using bma.Application.Expenses.Dtos;
using bma.Application.Overtime.Dtos;
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
/// API controller for managing overtime requests.
/// </summary>
[Route("api/overtime")]
[ApiController]
[Authorize]
public class OvertimeController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;

    public OvertimeController(
        IUnitOfWork unitOfWork,
        UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    /// <summary>
    /// Retrieves all overtime requests for the current user or company.
    /// </summary>
    /// <param name="companyId">The ID of the company to retrieve overtime requests for.</param>
    /// <returns>A list of overtime requests for the user or company.</returns>
    /// <response code="200">Returns the list of overtime requests.</response>
    /// <response code="401">User is not authenticated or unauthorized to access the company's overtime requests.</response>
    /// <response code="404">Company not found.</response>
    /// <response code="500">An error occurred while retrieving overtime requests.</response>
    [HttpGet("{companyId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<OvertimeRequestResponseDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllOvertimeRequests([FromRoute] int companyId)
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

            IEnumerable<OvertimeRequest> overtime;

            if (company.OwnerId == user.Id)
            {
                overtime = await _unitOfWork.OvertimeRequests.GetAllOvertimeRequestsForCompanyAsync(companyId);
            }
            else
            {
                overtime = await _unitOfWork.OvertimeRequests.GetAllOvertimeRequestsForUserAsync(user.Id);
            }

            overtime = overtime.ToList();
            var overtimeDto = overtime.Select(OvertimeRequestProfile.ToOvertimeRequestResponseDto);

            return Ok(overtimeDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = ex.Message });
        }
    }

    /// <summary>
    /// Creates a new overtime request.
    /// </summary>
    /// <param name="createOvertimeRequest">The DTO containing overtime request details.</param>
    /// <param name="companyId">The ID of the company for which the overtime request is being made.</param>
    /// <param name="createOvertimeRequestDtoValidator">Validator for the overtime request DTO.</param>
    /// <returns>A confirmation message upon successful creation.</returns>
    /// <response code="201">Overtime request created successfully.</response>
    /// <response code="400">Validation errors occurred.</response>
    /// <response code="401">User is not authenticated or unauthorized to create an overtime request for the company.</response>
    /// <response code="500">An error occurred while creating the overtime request.</response>
    [HttpPost("{companyId}")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(object))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateOvertimeRequest([FromBody] CreateOvertimeRequestDto createOvertimeRequest,
        [FromRoute] int companyId,
        IValidator<CreateOvertimeRequestDto> createOvertimeRequestDtoValidator)
    {
        try
        {
            var validationResult = await createOvertimeRequestDtoValidator.ValidateAsync(createOvertimeRequest);
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

            var overtimeRequest = createOvertimeRequest.ToOvertimeRequest();

            overtimeRequest.UserId = user.Id;
            overtimeRequest.CompanyId = companyId;

            await _unitOfWork.OvertimeRequests.AddAsync(overtimeRequest);
            await _unitOfWork.SaveChangesAsync();

            return Created(string.Empty, new { Message = "The overtime request has been successfully created." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = ex.Message });
        }
    }
}
