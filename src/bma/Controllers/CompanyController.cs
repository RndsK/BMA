using bma.Application.Companies.Dtos;
using bma.Application.JoinRequests.Dtos;
using bma.Domain.Constants;
using bma.Domain.Entities;
using bma.Domain.Exceptions;
using bma.Domain.Interfaces;
using bma.Infrastructure.Storage;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace bma.Presentation.Controllers;

[Route("api/company")]
[ApiController]
[Authorize]
public class CompanyController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IBlobStorageService _blobStorageService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CompanyController"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work for managing repositories.</param>
    /// <param name="userManager">The user manager for managing users.</param>
    public CompanyController(IUnitOfWork unitOfWork,
        UserManager<ApplicationUser> userManager,
        IBlobStorageService blobStorageService)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _blobStorageService = blobStorageService;
    }

    /// <summary>
    /// Creates a new company and assigns the logged-in user as the owner.
    /// </summary>
    /// <param name="createCompanyRequest">The company creation data transfer object.</param>
    /// <param name="createCompanyDtoValidator">The validator for company creation DTO.</param>
    /// <returns>The created company.</returns>
    /// <response code="201">Company created successfully.</response>
    /// <response code="400">Validation errors occurred.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="500">An error occurred while creating the company.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(object))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateCompany([FromBody] CreateCompanyDto createCompanyRequest,
        IValidator<CreateCompanyDto> createCompanyDtoValidator)
    {
        try
        {
            var validationResult = await createCompanyDtoValidator.ValidateAsync(createCompanyRequest);
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
                return Unauthorized();

            var company = CompanyProfile.ToCompany(createCompanyRequest);

            await _unitOfWork.Companies.CreateCompanyAsync(user.Id, company);

            return Created(string.Empty, new { Message = "The company has been successfully created." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while creating the company: {ex.Message}");
        }
    }

    /// <summary>
    /// Retrieves a list of call companies.
    /// </summary>
    /// <returns>A paginated response containing the list of companies and metadata.</returns>
    /// <response code="200">Returns a list of companies.</response>
    /// <response code="500">An error occurred while retrieving companies.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CompanyDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllCompanies()
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();
            var companies = await _unitOfWork.Companies.GetAllCompaniesAsync();

            var companyDtos = companies.Select(CompanyProfile.ToCompanyDto);

            return Ok(companyDtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while retrieving companies: {ex.Message}");
        }
    }



    /// <summary>
    /// Creates a new join request and informs the owner of the company about the request.
    /// </summary>
    /// <param name="companyId">The id of the company that the request is for.</param>
    /// <returns>Created status code.</returns>
    /// <response code="201">Join request created successfully.</response>
    /// <response code="400">Join request already exists or validation errors occurred.</response>
    /// <response code="404">Company not found.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="500">An error occurred while creating the join request.</response>
    [HttpPost("join/{companyId}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateJoinRequest([FromRoute] int companyId)
    {
        try
        {
            var company = await _unitOfWork.Companies.GetByIdAsync(companyId);
            if (company == null)
                return NotFound();
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            await _unitOfWork.JoinRequests.CreateJoinRequestAsync(user.Id, companyId);
            return Created(string.Empty, new { Message = "The join request has been successfully created." });
        }
        catch (DuplicateJoinRequestException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while creating the JoinRequest: {ex.Message}");
        }
    }

    /// <summary>
    /// Retrieves a list of pending join requests for a company.
    /// </summary>
    /// <param name="companyId">The id of the company that the request is for.</param>
    /// <returns>A response containing the list of join requests.</returns>
    /// <response code="200">Returns a list of join requests for the company.</response>
    /// <response code="500">An error occurred while retrieving join requests.</response>
    [HttpGet("{companyId}/joinRequests")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<JoinRequest>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCompanyJoinRequests([FromRoute] int companyId)
    {
        try
        {
            var company = await _unitOfWork.Companies.GetByIdAsync(companyId);
            if (company == null)
                return NotFound(new { Message = "Company not found." });
            var user = await _userManager.GetUserAsync(User);

            if (user == null || company.OwnerId != user.Id)
            {
                return Unauthorized();
            }
            var joinRequests = await _unitOfWork.JoinRequests.GetAllJoinRequestsByCompanyIdAsync(companyId);
            var joinRequestDto = joinRequests.Select(JoinRequestProfile.ToJoinRequestDto);
            return Ok(joinRequestDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while retrieving companies: {ex.Message}");
        }
    }

    /// <summary>
    /// Retrieves a list of join requests for a user.
    /// </summary>
    /// <returns>A response containing the list of join requests.</returns>
    /// <response code="200">Returns a list of join requests for the user in the company.</response>
    /// <response code="500">An error occurred while retrieving join requests.</response>
    [HttpGet("{companyId}/myJoinRequests")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<JoinRequest>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUserJoinRequests([FromRoute] int companyId)
    {
        try
        {
            var company = await _unitOfWork.Companies.GetByIdAsync(companyId);
            if (company == null)
                return NotFound(new { Message = "Company not found." });
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
            
            var joinRequests = await _unitOfWork.JoinRequests.GetAllJoinRequestsByUserAndCompanyAsync(companyId, user.Id);
            var joinRequestDto = joinRequests.Select(JoinRequestProfile.ToJoinRequestDto);
            return Ok(joinRequestDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while retrieving companies: {ex.Message}");
        }
    }

    /// <summary>
    /// Marks a request as rejected.
    /// </summary>
    /// <param name="requestId">The ID of the request to reject.</param>
    /// <returns>An IActionResult indicating the result of the operation.</returns>
    /// <response code="200">Request rejected successfully.</response>
    /// <response code="401">User is not authorized to reject requests.</response>
    /// <response code="404">Request not found.</response>
    /// <response code="500">An error occurred while rejecting the request.</response>
    [HttpPut("{joinRequestId}/reject")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RejectJoinRequest([FromRoute] int joinRequestId)
    {
        try
        {
            var joinRequest = await _unitOfWork.JoinRequests.GetByIdAsync(joinRequestId);
            var company = await _unitOfWork.Companies.GetByIdAsync(joinRequest.CompanyId);
            if (company == null)
                return NotFound(new { Message = "Company not found." });
            var user = await _userManager.GetUserAsync(User);

            if (user == null || company.OwnerId != user.Id || joinRequest.Status != StringDefinitions.RequestStatusPending)
            {
                return Unauthorized();
            }
            var request = await _unitOfWork.JoinRequests.GetByIdAsync(joinRequestId);
            if (request == null)
            {
                return NotFound(new { Message = "Join request not found." });
            }

            request.Status = StringDefinitions.RequestStatusRejected;

            _unitOfWork.JoinRequests.Update(request);

            await _unitOfWork.Approvals.CreateApprovalForRequest(request.Id, user.Email!, StringDefinitions.RequestStatusRejected);

            await _unitOfWork.SaveChangesAsync();

            return Ok(new { Message = "The join request has been successfully rejected." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = $"An error occurred: {ex.Message}" });
        }
    }

    /// <summary>
    /// Marks a request as approved.
    /// </summary>
    /// <param name="requestId">The ID of the request to approve.</param>
    /// <returns>An IActionResult indicating the result of the operation.</returns>
    /// <response code="200">Request approved successfully.</response>
    /// <response code="401">User is not authorized to approve requests.</response>
    /// <response code="404">Request not found.</response>
    /// <response code="500">An error occurred while approving the request.</response>
    [HttpPut("{joinRequestId}/approve")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ApproveJoinRequest([FromRoute] int joinRequestId)
    {
        try
        {
            var joinRequest = await _unitOfWork.JoinRequests.GetByIdAsync(joinRequestId);
            if (joinRequest == null)
            {
                return NotFound(new { Message = "Join request not found." });
            }
            var company = await _unitOfWork.Companies.GetByIdAsync(joinRequest.CompanyId);
            if (company == null)
                return NotFound(new { Message = "Company not found." });

            var user = await _userManager.GetUserAsync(User);
            if (user == null || company.OwnerId != user.Id || joinRequest.Status != StringDefinitions.RequestStatusPending)
            {
                return Unauthorized();
            }

            joinRequest.Status = StringDefinitions.RequestStatusApproved;
            joinRequest.AcceptanceDate = DateOnly.FromDateTime(DateTime.UtcNow);

            _unitOfWork.JoinRequests.Update(joinRequest);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.Approvals.CreateApprovalForJoinRequest(joinRequest.Id, user.Email, StringDefinitions.RequestStatusApproved);

            await _unitOfWork.RolesInCompanies.AddRoleInCompanyAsync(joinRequest.UserId, company.Id, StringDefinitions.User);


            return Ok(new { Message = "The join request has been successfully approved and the user has been added to the company." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = $"An error occurred: {ex.Message}" });
        }
    }

    /// <summary>
    /// Uploads a logo for the specified company.
    /// </summary>
    /// <param name="companyId">The ID of the company.</param>
    /// <param name="logo">The logo file to upload.</param>
    /// <returns>A confirmation message upon successful upload.</returns>
    /// <response code="200">Logo uploaded successfully.</response>
    /// <response code="400">Validation errors occurred.</response>
    /// <response code="401">User is not authenticated or unauthorized to update the company.</response>
    /// <response code="404">Company not found.</response>
    /// <response code="500">An error occurred while uploading the logo.</response>
    [HttpPut("{companyId}/logo")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UploadCompanyLogo(
        [FromRoute] int companyId,
        IFormFile logo)
    {
        try
        {
            if (logo == null || logo.Length == 0)
                return BadRequest(new { Message = "Invalid logo file." });

            var company = await _unitOfWork.Companies.GetByIdAsync(companyId);
            if (company == null)
                return NotFound(new { Message = "Company not found." });

            var user = await _userManager.GetUserAsync(User);
            if (user == null || company.OwnerId != user.Id)
                return Unauthorized();

            var fileName = $"{Guid.NewGuid()}_{logo.FileName}";
            await using var stream = logo.OpenReadStream();
            var logoUrl = await _blobStorageService.UploadFileAsync(stream, fileName);

            company.Logo = logoUrl;
            _unitOfWork.Companies.Update(company);
            await _unitOfWork.SaveChangesAsync();

            return Ok(new { Message = "Company logo uploaded successfully.", LogoUrl = logoUrl });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = $"An error occurred: {ex.Message}" });
        }
    }

}
