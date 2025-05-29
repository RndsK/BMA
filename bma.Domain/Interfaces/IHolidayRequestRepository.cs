using System.Text.Json;
using bma.Domain.Entities;
using bma.Domain.Entities.RequestEntities;
using Microsoft.EntityFrameworkCore;

namespace bma.Domain.Interfaces;

public interface IHolidayRequestRepository : IRepository<HolidayRequest>
{
    /// <summary>
    /// Retrieves the bank holiday dates for a specified country.
    /// </summary>
    /// <param name="countryCode">The country code (e.g., "US", "GB") for which to retrieve bank holidays.</param>
    /// <returns>A task representing the asynchronous operation, containing a collection of bank holiday dates.</returns>
    /// <exception cref="HttpRequestException"></exception>
    /// <exception cref="TaskCanceledException"></exception>
    /// <exception cref="UriFormatException"></exception>
    /// <exception cref="JsonException"></exception>
    Task<IEnumerable<DateTime>> GetBankHolidayDatesAsync(string countryCode);

    /// <summary>
    /// Retrieves the approved upcoming holidays for a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user for whom to retrieve upcoming holidays.</param>
    /// <returns>A task representing the asynchronous operation, containing a collection of approved upcoming holiday requests.</returns>
    ///<exception cref="ArgumentException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task<IEnumerable<HolidayRequest>> GetApprovedUpcomingHolidaysByUserIdAsync(string userId, int companyId);

    /// <summary>
    /// Retrieves holiday requests for a specific company, optionally filtered by month.
    /// </summary>
    /// <param name="companyId">The ID of the company whose holidays should be retrieved.</param>
    /// <param name="searchMonth">An optional parameter to filter the holidays by month (1-12).</param>
    /// <returns>A task representing the asynchronous operation, containing a collection of holiday requests for the specified company and month.</returns>
    /// <exception cref="OperationCanceledException"></exception>
    Task<IEnumerable<HolidayRequest>> GetHolidaysByCompanyAsync(int companyId, int? searchMonth);

    /// <summary>
    /// Retrieves holiday requests based on user ID.
    /// </summary>
    /// <param name="userId">The user whose requests to return.</param>
    /// <returns>A task representing the asynchronous operation, containing a collection of holiday requests that match the specified expression.</returns>
    /// <exception cref="OperationCanceledException"></exception>
    Task<IEnumerable<HolidayRequest>> GetHolidaysByUserAsync(string userId, int companyId);

    /// <summary>
    /// Creates a new holiday request for a user and associates it with a specific company.
    /// </summary>
    /// <param name="userId">The ID of the user creating the holiday request.</param>
    /// <param name="companyId">The ID of the company associated with the holiday request.</param>
    /// <param name="createHolidayRequest">The holiday request entity containing the holiday details.</param>
    /// <returns>A task representing the asynchronous operation of creating the holiday request.</returns>
    /// <exception cref="OperationCanceledException"></exception>
    /// /// <exception cref="DbUpdateException"></exception>
    Task CreateHolidayRequestAsync(string userId, int companyId, HolidayRequest createHolidayRequest);

    /// <summary>
    /// Calculates the holiday balance for a user in a specific company.
    /// </summary>
    /// <param name="userId">The ID of the user whose holiday balance is being calculated.</param>
    /// <param name="companyId">The ID of the company associated with the user.</param>
    /// <param name="startWorkDate">The start date of the user's employment in the company.</param>
    /// <returns>A <see cref="HolidayBalanceDto"/> object containing the current holiday balance, 
    /// upcoming holidays, and the balance after upcoming holidays.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="startWorkDate"/> is null.</exception>
    /// <exception cref="OperationCanceledException">Thrown if the asynchronous operation is canceled.</exception>
    Task<HolidayBalance> CalculateHolidayBalanceAsync(string userId, int companyId, DateOnly? startWorkDate);
}