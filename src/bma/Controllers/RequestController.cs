using bma.Application.Common;
using bma.Application.Requests.Dtos;
using bma.Domain.Constants;
using bma.Domain.Entities;
using bma.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace bma.Presentation.Controllers;

/// <summary>
/// Controller for managing transfer requests within a company.
/// </summary>
[Route("api/requests")]
[ApiController]
[Authorize]
public class RequestController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;

    public RequestController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
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
    [HttpPut("{requestId}/reject")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RejectRequest([FromRoute] int requestId)
    {
        try
        {
            var request = await _unitOfWork.Requests.GetByIdAsync(requestId);
            if (request == null)
            {
                return NotFound(new { Message = "Request not found." });
            }

            var company = await _unitOfWork.Companies.GetByIdAsync(request.CompanyId);
            var user = await _userManager.GetUserAsync(User);
            if (user == null || company.OwnerId != user.Id)
            {
                return Unauthorized();
            }

            request.Status = StringDefinitions.RequestStatusRejected;

            _unitOfWork.Requests.Update(request);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.Approvals.CreateApprovalForRequest(request.Id, user.Email, StringDefinitions.RequestStatusRejected);

            return Ok(new { Message = "The request has been successfully rejected." });
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
    [HttpPut("{requestId}/approve")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ApproveRequest([FromRoute] int requestId)
    {
        try
        {
            var request = await _unitOfWork.Requests.GetByIdAsync(requestId);
            if (request == null)
            {
                return NotFound(new { Message = "Request not found." });
            }

            var company = await _unitOfWork.Companies.GetByIdAsync(request.CompanyId);
            if (company == null)
                return NotFound();
            var user = await _userManager.GetUserAsync(User);
            if (user == null || company.OwnerId != user.Id || request.Status != StringDefinitions.RequestStatusPending)
            {
                return Unauthorized();
            }
            request.Status = StringDefinitions.RequestStatusApproved;

            _unitOfWork.Requests.Update(request);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.Approvals.CreateApprovalForRequest(request.Id, user.Email, StringDefinitions.RequestStatusApproved);

            return Ok(new { Message = "The request has been successfully approved." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = $"An error occurred: {ex.Message}" });
        }
    }

    /// <summary>
    /// Marks a request as cancelled.
    /// </summary>
    /// <param name="requestId">The ID of the request to cancel.</param>
    /// <returns>An IActionResult indicating the result of the operation.</returns>
    /// <response code="200">Request cancelled successfully.</response>
    /// <response code="401">User is not authorized to cancel requests.</response>
    /// <response code="404">Request not found.</response>
    /// <response code="500">An error occurred while cancelling the request.</response>
    [HttpPut("{requestId}/cancel")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CancelRequest([FromRoute] int requestId)
    {
        try
        {
            var request = await _unitOfWork.Requests.GetByIdAsync(requestId);
            if (request == null)
            {
                return NotFound(new { Message = "Request not found." });
            }

            var company = await _unitOfWork.Companies.GetByIdAsync(request.CompanyId);
            var user = await _userManager.GetUserAsync(User);
            if (user == null || request.UserId != user.Id)
            {
                return Unauthorized();
            }

            request.Status = StringDefinitions.RequestStatusCancelled;

            _unitOfWork.Requests.Update(request);
            await _unitOfWork.SaveChangesAsync();

            return Ok(new { Message = "The request has been successfully rejected." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = $"An error occurred: {ex.Message}" });
        }
    }

    /// <summary>
    /// Retrieves a paginated and optionally filtered list of requests for a specific company.
    /// </summary>
    /// <param name="companyId">The ID of the company to retrieve requests for.</param>
    /// <param name="searchQuery">The search term to filter requests by type (optional).</param>
    /// <param name="pageNumber">The page number for pagination (default is 1).</param>
    /// <param name="pageSize">The number of items per page (default is 10).</param>
    /// <returns>A paginated response containing the list of requests and metadata.</returns>
    /// <response code="200">Returns the list of requests for the specified company.</response>
    /// <response code="400">Invalid pagination parameters.</response>
    /// <response code="401">User is not authorized to access company requests.</response>
    /// <response code="500">An error occurred while retrieving the requests.</response>
    [HttpGet("company/{companyId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResult<object>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPendingCompanyRequests([FromRoute] int companyId,
        [FromQuery] string? searchQuery, [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest("PageNumber and PageSize must be greater than 0.");
            }

            var company = await _unitOfWork.Companies.GetByIdAsync(companyId);
            if (company == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null || company.OwnerId != user.Id)
            {
                return Unauthorized();
            }

            var (requests, totalCount) = await _unitOfWork.Requests.GetPagedAndFilteredPendingRequestsAsync(
                companyId, searchQuery, pageNumber, pageSize);

            var requestDtos = requests.Select(r => RequestProfile.MapToDto(r.Request, r.Discriminator)).ToList();

            var response = new PagedResult<object>
            {
                Items = requestDtos,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = $"An error occurred: {ex.Message}" });
        }
    }
}
