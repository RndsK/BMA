using bma.Application.Expenses.Dtos;
using bma.Domain.Entities;
using bma.Domain.Entities.RequestEntities;
using bma.Domain.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace bma.Presentation.Controllers;

/// <summary>
/// API controller for managing expenses.
/// </summary>
[Route("api/expenses")]
[ApiController]
[Authorize]
public class ExpensesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IBlobStorageService _blobStorageService;

    public ExpensesController(
        IUnitOfWork unitOfWork,
        UserManager<ApplicationUser> userManager,
        IBlobStorageService blobStorageService)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _blobStorageService = blobStorageService;
    }

    /// <summary>
    /// Retrieves all expenses for the current user or company.
    /// </summary>
    /// <param name="companyId">The ID of the company.</param>
    /// <returns>A list of expenses associated with the user or company.</returns>
    /// <response code="200">Returns the list of expenses.</response>
    /// <response code="401">User is not authenticated or unauthorized to access the company's expenses.</response>
    /// <response code="404">Company not found.</response>
    /// <response code="500">An error occurred while retrieving expenses.</response>
    [HttpGet("{companyId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ExpenseRequestResponseDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllExpenses([FromRoute] int companyId)
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

            IEnumerable<ExpensesRequest> expenses;

            if (company.OwnerId == user.Id)
            {
                expenses = await _unitOfWork.ExpensesRequests.GetAllExpenseRequestsForCompanyAsync(companyId);
            }
            else
            {
                expenses = await _unitOfWork.ExpensesRequests.GetAllExpenseRequestsForUserAsync(user.Id);
            }

            expenses = expenses.ToList();
            var expensesDtos = expenses.Select(ExpensesRequestProfile.ToExpensesRequestResponseDto);

            return Ok(expensesDtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = ex.Message });
        }
    }

    /// <summary>
    /// Creates a new expense request.
    /// </summary>
    /// <param name="createExpenseRequest">The DTO for creating an expense request.</param>
    /// <param name="companyId">The ID of the company.</param>
    /// <param name="receipt">Optional receipt file.</param>
    /// <param name="createExpenseRequestDtoValidator">Validator for the expense request DTO.</param>
    /// <returns>A confirmation message upon successful creation.</returns>
    /// <response code="201">Expense request created successfully.</response>
    /// <response code="400">Validation errors occurred.</response>
    /// <response code="401">User is not authenticated or unauthorized to create an expense request for the company.</response>
    /// <response code="500">An error occurred while creating the expense request.</response>
    [HttpPost("{companyId}")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(object))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateExpenseRequest([FromForm] CreateExpenseRequestDto createExpenseRequest,
        [FromRoute] int companyId,
        IFormFile? receipt,
        IValidator<CreateExpenseRequestDto> createExpenseRequestDtoValidator)
    {
        try
        {
            var validationResult = await createExpenseRequestDtoValidator.ValidateAsync(createExpenseRequest);
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

            if (receipt != null)
            {
                var fileName = $"{Guid.NewGuid()}_{receipt.FileName}";
                await using var stream = receipt.OpenReadStream();
                blobUrl = await _blobStorageService.UploadFileAsync(stream, fileName);
            }

            var expenseRequest = ExpensesRequestProfile.ToExpensesRequest(createExpenseRequest);

            expenseRequest.UserId = user.Id;
            expenseRequest.User = user;
            expenseRequest.CompanyId = companyId;
            expenseRequest.Attachment = blobUrl;

            await _unitOfWork.ExpensesRequests.AddAsync(expenseRequest);
            await _unitOfWork.SaveChangesAsync();

            return Created(string.Empty, new { Message = "The expense request has been successfully created." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = ex.Message });
        }
    }
}
