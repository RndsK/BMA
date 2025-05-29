using bma.Application.Transfer.Finacial.Dtos;
using bma.Application.Transfer.SignOff.Dtos;
using bma.Domain.Constants;
using bma.Domain.Entities;
using bma.Domain.Entities.RequestEntities;
using bma.Domain.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace bma.Presentation.Controllers;

/// <summary>
/// Controller for managing transfer requests within a company.
/// </summary>
[Route("api/transfer/{companyId}")]
[ApiController]
[Authorize]
public class TransferController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IBlobStorageService _blobStorageService;

    public TransferController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IBlobStorageService blobStorageService)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _blobStorageService = blobStorageService;
    }

    /// <summary>
    /// Retrieves all transfer requests for the logged-in user or their company if they are an owner.
    /// </summary>
    /// <param name="companyId">The ID of the company.</param>
    /// <returns>A list of transfer requests relevant to the user or company.</returns>
    /// <response code="200">Returns the list of transfer requests.</response>
    /// <response code="401">User is not authenticated or unauthorized to access the transfer requests.</response>
    /// <response code="500">An error occurred while retrieving transfer requests.</response>
    [HttpGet("financial")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<FinancialRequestResponseDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllFinancialRequests([FromRoute] int companyId)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            var isUserInCompany = await _unitOfWork.Companies.GetAllByUserIdAsync(user.Id);
            if (user == null || !isUserInCompany.Contains(companyId))
            {
                return Unauthorized();
            }
            var company = await _unitOfWork.Companies.GetByIdAsync(companyId);
            if (company == null)
                return NotFound();

            IEnumerable<FinancialRequest> financials;

            if (company.OwnerId == user.Id)
            {
                financials = await _unitOfWork.FinancialRequests.GetAllFinancialRequestsForCompanyAsync(companyId);
            }
            else
            {
                financials = await _unitOfWork.FinancialRequests.GetAllFinancialRequestsForUserAsync(user.Id);
            }

            financials = financials.ToList();
            var financialsDtos = financials.Select(FinancialRequestProfile.ToFinancialRequestResponseDto);

            return Ok(financialsDtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Creates a new Financial transfer request for the logged-in user.
    /// </summary>
    /// <param name="createFinancialRequestDto">The DTO for creating the Financial transfer request.</param>
    /// <param name="companyId">The ID of the company where the Financial transfer request is being made.</param>
    /// <param name="supportingDocument">An optional file to support the Financial transfer request.</param>
    /// <param name="createFinancialRequestValidator">Validator for the Financial transfer request DTO.</param>
    /// <returns>A confirmation message upon successful creation.</returns>
    /// <response code="201">Financial transfer request created successfully.</response>
    /// <response code="400">Validation errors occurred or a specified user was not found.</response>
    /// <response code="401">User is not authenticated or unauthorized to create a Financial transfer request for the company.</response>
    /// <response code="500">An error occurred while creating the Financial transfer request.</response>
    [HttpPost("financial")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(object))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateFinancialRequest([FromForm] CreateFinancialRequestDto createFinancialRequestDto,
        [FromRoute] int companyId,
        IFormFile? supportingDocument,
        IValidator<CreateFinancialRequestDto> createFinancialRequestValidator)
    {
        try
        {
            var validationResult = await createFinancialRequestValidator.ValidateAsync(createFinancialRequestDto);
            if (!validationResult.IsValid)
            {
                var structuredErrors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        group => group.Key,
                        group => group.Select(e => new
                        {
                            key = e.CustomState?.GetType().GetProperty("Key")?.GetValue(e.CustomState)?.ToString(),
                            message = e.CustomState?.GetType().GetProperty("Message")?.GetValue(e.CustomState)?.ToString()
                        }).ToArray()
                    );

                return ValidationProblem(new ValidationProblemDetails
                {
                    Title = "One or more validation errors occurred.",
                    Status = StatusCodes.Status400BadRequest,
                    Extensions = { ["structuredErrors"] = structuredErrors }
                });

            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var isUserInCompany = await _unitOfWork.Companies.GetAllByUserIdAsync(user.Id);
            if (!isUserInCompany.Contains(companyId))
            {
                return Unauthorized();
            }

            string? blobUrl = null;

            if (supportingDocument != null)
            {
                var fileName = $"{Guid.NewGuid()}_{supportingDocument.FileName}";
                await using var stream = supportingDocument.OpenReadStream();
                blobUrl = await _blobStorageService.UploadFileAsync(stream, fileName);
            }

            var financialRequest = FinancialRequestProfile.ToFinancialRequest(createFinancialRequestDto);
            financialRequest.UserId = user.Id;
            financialRequest.CompanyId = companyId;
            financialRequest.SupportingDocument = blobUrl;

            await _unitOfWork.FinancialRequests.AddAsync(financialRequest);
            await _unitOfWork.SaveChangesAsync();

            var newSignOffParticipants = new List<string>();

            foreach (var participantEmail in createFinancialRequestDto.SignOffParticipants)
            {
                var participant = await _unitOfWork.Users.GetByEmailAsync(participantEmail);
                if (participant == null)
                {
                    return BadRequest(new { Message = $"User with email {participantEmail} not found." });
                }

                newSignOffParticipants.Add(participant.Email!);

                var signOffParticipant = new TransferRequestSignOffParticipant
                {
                    UserId = participant.Id,
                    RequestId = financialRequest.Id,
                    Status = StringDefinitions.SignOffStatusNotSigned
                };

                await _unitOfWork.FinancialRequests.CreateSignOffParticipantAsync(signOffParticipant);
                await _unitOfWork.SaveChangesAsync();
            }

            // Add all new participants to the financialRequest after processing.
            financialRequest.SignOffParticipants.AddRange(newSignOffParticipants);

            // Update the request to reflect new participants.
            _unitOfWork.FinancialRequests.Update(financialRequest);
            await _unitOfWork.SaveChangesAsync();

            return Created(string.Empty, new { Message = "The transfer request has been successfully created." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { ex.Message });
        }
    }


    /// <summary>
    /// Retrieves all sign-off transfer requests for the logged-in user or their company if they are an owner.
    /// </summary>
    /// <param name="companyId">The ID of the company.</param>
    /// <returns>A list of sign-off transfer requests relevant to the user or company.</returns>
    /// <response code="200">Returns the list of sign-off transfer requests.</response>
    /// <response code="401">User is not authenticated or unauthorized to access the sign-off transfer requests.</response>
    /// <response code="500">An error occurred while retrieving sign-off transfer requests.</response>
    [HttpGet("signOff")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<FinancialRequestResponseDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllSignOffRequests([FromRoute] int companyId)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            var isUserInCompany = await _unitOfWork.Companies.GetAllByUserIdAsync(user.Id);
            if (user == null || !isUserInCompany.Contains(companyId))
            {
                return Unauthorized();
            }
            var company = await _unitOfWork.Companies.GetByIdAsync(companyId);
            if (company == null)
                return NotFound();

            IEnumerable<SignOffRequest> signOffs;

            if (company.OwnerId == user.Id)
            {
                signOffs = await _unitOfWork.SignOffRequests.GetAllSignOffRequestsForCompanyAsync(companyId);
            }
            else
            {
                signOffs = await _unitOfWork.SignOffRequests.GetAllSignOffRequestsForUserAsync(user.Id);
            }

            signOffs = signOffs.ToList();
            var financialsDtos = signOffs.Select(SignOffRequestProfile.ToSignOffRequestResponseDto);

            return Ok(financialsDtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { ex.Message });
        }
    }

    /// <summary>
    /// Creates a new sign-off  transfer request for the logged-in user.
    /// </summary>
    /// <param name="createSignOffRequestDto">The DTO for creating the sign-off  transfer request.</param>
    /// <param name="companyId">The ID of the company where the sign-off transfer request is being made.</param>
    /// <param name="supportingDocument">An optional file to support the sign-off transfer request.</param>
    /// <param name="createSignOffRequestValidator">Validator for the sign-off transfer request DTO.</param>
    /// <returns>A confirmation message upon successful creation.</returns>
    /// <response code="201">Sign-off transfer request created successfully.</response>
    /// <response code="400">Validation errors occurred or a specified user was not found.</response>
    /// <response code="401">User is not authenticated or unauthorized to create a sign-off  transfer request for the company.</response>
    /// <response code="500">An error occurred while creating the sign-off transfer request.</response>
    [HttpPost("signOff")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(object))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateSignOffRequest([FromForm] CreateSignOffRequestDto createSignOffRequestDto,
        [FromRoute] int companyId,
        IFormFile? supportingDocument,
        IValidator<CreateSignOffRequestDto> createSignOffRequestValidator)
    {
        try
        {
            var validationResult = await createSignOffRequestValidator.ValidateAsync(createSignOffRequestDto);
            if (!validationResult.IsValid)
            {
                var structuredErrors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        group => group.Key,
                        group => group.Select(e => new
                        {
                            key = e.CustomState?.GetType().GetProperty("Key")?.GetValue(e.CustomState)?.ToString(),
                            message = e.CustomState?.GetType().GetProperty("Message")?.GetValue(e.CustomState)?.ToString()
                        }).ToArray()
                    );

                return ValidationProblem(new ValidationProblemDetails
                {
                    Title = "One or more validation errors occurred.",
                    Status = StatusCodes.Status400BadRequest,
                    Extensions = { ["structuredErrors"] = structuredErrors }
                });

            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var isUserInCompany = await _unitOfWork.Companies.GetAllByUserIdAsync(user.Id);
            if (!isUserInCompany.Contains(companyId))
            {
                return Unauthorized();
            }

            string? blobUrl = null;

            if (supportingDocument != null)
            {
                var fileName = $"{Guid.NewGuid()}_{supportingDocument.FileName}";
                await using var stream = supportingDocument.OpenReadStream();
                blobUrl = await _blobStorageService.UploadFileAsync(stream, fileName);
            }

            var signOffRequest = SignOffRequestProfile.ToSignOffRequest(createSignOffRequestDto);
            signOffRequest.UserId = user.Id;
            signOffRequest.CompanyId = companyId;
            signOffRequest.SupportingDocument = blobUrl;

            await _unitOfWork.SignOffRequests.AddAsync(signOffRequest);
            await _unitOfWork.SaveChangesAsync();

            var newSignOffParticipants = new List<string>();

            foreach (var participantEmail in createSignOffRequestDto.SignOffParticipants)
            {
                var participant = await _unitOfWork.Users.GetByEmailAsync(participantEmail);
                if (participant == null)
                {
                    return BadRequest(new { Message = $"User with email {participantEmail} not found." });
                }

                newSignOffParticipants.Add(participant.Email!);

                var signOffParticipant = new TransferRequestSignOffParticipant
                {
                    UserId = participant.Id,
                    RequestId = signOffRequest.Id,
                    Status = StringDefinitions.SignOffStatusNotSigned
                };

                await _unitOfWork.FinancialRequests.CreateSignOffParticipantAsync(signOffParticipant);
                await _unitOfWork.SaveChangesAsync();
            }

            // Add all new participants to the financialRequest after processing.
            signOffRequest.SignOffParticipants.AddRange(newSignOffParticipants);

            // Update the request to reflect new participants.
            _unitOfWork.SignOffRequests.Update(signOffRequest);
            await _unitOfWork.SaveChangesAsync();

            return Created(string.Empty, new { Message = "The transfer request has been successfully created." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { ex.Message });
        }
    }
}
