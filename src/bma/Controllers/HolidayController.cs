using bma.Application.Expenses.Dtos;
using bma.Application.Holidays.Dtos;
using bma.Domain.Entities;
using bma.Domain.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace bma.Presentation.Controllers;

[Route("api/holidays")]
[ApiController]
public class HolidayController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;

    public HolidayController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    /// <summary>
    /// Creates a new holiday request.
    /// </summary>
    /// <param name="createHolidayRequest">Holiday request DTO.</param>
    /// <param name="companyId">ID of the company.</param>
    /// <param name="createHolidayRequestDtoValidator">Validator for the holiday request DTO.</param>
    /// <returns>A confirmation message upon successful creation.</returns>
    /// <response code="201">Holiday request created successfully.</response>
    /// <response code="400">Validation errors occurred.</response>
    /// <response code="401">User is not authenticated or unauthorized.</response>
    /// <response code="404">Company not found.</response>
    /// <response code="500">An error occurred while creating the holiday request.</response>
    [HttpPost("request/{companyId}")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(object))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateHolidayRequest([FromBody] CreateHolidayRequestDto createHolidayRequest,
        [FromRoute] int companyId,
        IValidator<CreateHolidayRequestDto> createHolidayRequestDtoValidator)
    {
        try
        {
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

            var validationResult = await createHolidayRequestDtoValidator.ValidateAsync(createHolidayRequest);
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

            var company = await _unitOfWork.Companies.GetByIdAsync(companyId);
            if (company == null)
                return NotFound();

            var holidayRequest = createHolidayRequest.ToHolidayRequest();

            await _unitOfWork.Holidays.CreateHolidayRequestAsync(user.Id, company.Id, holidayRequest);
            return Created(string.Empty, new { Message = "The holiday request has been successfully created." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while creating the Holiday request: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets the bank holidays for a country using an external API.
    /// </summary>
    /// <param name="countryCode">Country code for the country whose holidays need to be displayed.</param>
    /// <returns>A list of bank holiday dates.</returns>
    /// <response code="200">Returns the list of bank holidays.</response>
    /// <response code="400">Invalid country code.</response>
    /// <response code="404">No bank holidays found for the specified country.</response>
    /// <response code="500">An error occurred while retrieving bank holidays.</response>
    [HttpGet("bank-holidays/{countryCode}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<DateOnly>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetBankHolidaysByCountryCode(string countryCode)
    {
        try
        {
            if (string.IsNullOrEmpty(countryCode))
            {
                return BadRequest(new { Errors = new[] { "Country code cannot be null or empty." } });
            }

            var bankHolidayDates = await _unitOfWork.Holidays.GetBankHolidayDatesAsync(countryCode);

            if (bankHolidayDates == null || !bankHolidayDates.Any())
            {
                return NotFound(new { Message = "No bank holidays found for the specified country." });
            }

            return Ok(bankHolidayDates);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = $"An error occurred while retrieving bank holidays: {ex.Message}" });
        }
    }

    /// <summary>
    /// Gets the current holiday balance for the user.
    /// </summary>
    /// <param name="companyId">The ID of the company.</param>
    /// <returns>The current holiday balance, upcoming holidays, and balance after upcoming holidays.</returns>
    /// <response code="200">Returns the holiday balance.</response>
    /// <response code="400">User is not part of the company.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="500">An error occurred while retrieving the holiday balance.</response>
    [HttpGet("balance/{companyId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(HolidayBalance))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetHolidayBalanceForUser([FromRoute] int companyId)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var joinRequest = await _unitOfWork.JoinRequests.GetJoinRequestForUserByCompanyAsync(user.Id, companyId);
            if (joinRequest == null)
                return BadRequest("User is not part of this company.");

            var holidayBalance = await _unitOfWork.Holidays.CalculateHolidayBalanceAsync(user.Id, companyId, joinRequest.AcceptanceDate);

            return Ok(holidayBalance);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = $"An error occurred while retrieving holiday balance: {ex.Message}" });
        }
    }

    /// <summary>
    /// Gets a user's upcoming holidays.
    /// </summary>
    /// <param name="companyId">The ID of the company.</param>
    /// <returns>A list of upcoming holidays.</returns>
    /// <response code="200">Returns the upcoming holidays.</response>
    /// <response code="401">User is not authenticated or unauthorized.</response>
    /// <response code="500">An error occurred while retrieving upcoming holidays.</response>
    [HttpGet("upcoming/{companyId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<HolidayRequestResponseDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUpcomingHolidaysForUser([FromRoute] int companyId)
    {
        try
        {
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

            var holidays = await _unitOfWork.Holidays.GetApprovedUpcomingHolidaysByUserIdAsync(user.Id, companyId);
            var holidayDto = holidays.Select(HolidayRequestProfile.ToHolidayRequestResponseDto);
            return Ok(holidayDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = $"An error occurred while retrieving upcoming holidays: {ex.Message}" });
        }
    }

    /// <summary>
    /// Gets all holidays for a company or just for a specific month.
    /// </summary>
    /// <param name="searchMonth">The month for which holidays should be displayed (optional).</param>
    /// <param name="companyId">The ID of the company.</param>
    /// <returns>A list of holidays for the company.</returns>
    /// <response code="200">Returns the company holidays.</response>
    /// <response code="400">Invalid month value.</response>
    /// <response code="401">User is not authenticated or unauthorized.</response>
    /// <response code="500">An error occurred while retrieving holidays.</response>
    [HttpGet("company/{companyId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<HolidayRequestResponseDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCompanyHolidays([FromQuery] int? searchMonth, [FromRoute] int companyId)
    {
        try
        {
            if (searchMonth.HasValue && (searchMonth < 1 || searchMonth > 12))
            {
                return BadRequest("The month value must be between 1 and 12.");
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

            var holidays = await _unitOfWork.Holidays.GetHolidaysByCompanyAsync(companyId, searchMonth);
            var holidayDto = holidays.Select(HolidayRequestProfile.ToHolidayRequestResponseDto);
            return Ok(holidayDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = $"An error occurred while retrieving holidays: {ex.Message}" });
        }
    }

    /// <summary>
    /// Gets all holidays for a user or just for a company.
    /// </summary>
    /// <param name="companyId">The ID of the company.</param>
    /// <returns>A list of holidays for the user.</returns>
    /// <response code="200">Returns the company holidays.</response>
    /// <response code="400">Invalid month value.</response>
    /// <response code="401">User is not authenticated or unauthorized.</response>
    /// <response code="500">An error occurred while retrieving holidays.</response>
    [HttpGet("user/{companyId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<HolidayRequestResponseDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUserHolidays([FromRoute] int companyId)
    {
        try
        {
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

            var holidays = await _unitOfWork.Holidays.GetHolidaysByUserAsync(user.Id, companyId);
            var holidayDto = holidays.Select(HolidayRequestProfile.ToHolidayRequestResponseDto);
            return Ok(holidayDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = $"An error occurred while retrieving holidays: {ex.Message}" });
        }
    }
}
