using bma.Application.Approvals.Dtos;
using bma.Domain.Entities;
using bma.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace bma.Presentation.Controllers;

/// <summary>
/// Controller for managing approvals.
/// </summary>
[Route("api/approvals")]
[ApiController]
[Authorize]
public class ApprovalController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApprovalController"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work for managing repositories.</param>
    /// <param name="userManager">The user manager for managing users.</param>
    public ApprovalController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    /// <summary>
    /// Retrieves approvals by the user ID.
    /// </summary>
    /// <returns>A list of approvals for the specified user.</returns>
    /// <response code="200">Returns a list of approvals.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="500">An error occurred while retrieving approvals.</response>
    [HttpGet("currentUser")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Approval>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetApprovalsByUserId()
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var approvals = await _unitOfWork.Approvals.GetByUserIdAsync(user.Id);
            var approvalDto = approvals.Select(ApprovalProfile.ToApprovalDto);
            return Ok(approvalDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while retrieving approvals: {ex.Message}");
        }
    }

    /// <summary>
    /// Retrieves approvals by the company ID.
    /// </summary>
    /// <param name="companyId">The ID of the company.</param>
    /// <returns>A list of approvals for the specified company.</returns>
    /// <response code="200">Returns a list of approvals.</response>
    /// <response code="401">User is not authorized.</response>
    /// <response code="500">An error occurred while retrieving approvals.</response>
    [HttpGet("company/{companyId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Approval>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetApprovalsByCompanyId([FromRoute] int companyId)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var company = await _unitOfWork.Companies.GetByIdAsync(companyId);
            if (company == null)
            {
                return NotFound(new { Message = "Company not found." });
            }

            if (company.OwnerId != user.Id)
            {
                return Unauthorized(new { Message = "You are not authorized to access approvals for this company." });
            }

            var approvals = await _unitOfWork.Approvals.GetByCompanyIdAsync(companyId);
            var approvalDto = approvals.Select(ApprovalProfile.ToApprovalDto);
            return Ok(approvalDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while retrieving approvals: {ex.Message}");
        }
    }
}
