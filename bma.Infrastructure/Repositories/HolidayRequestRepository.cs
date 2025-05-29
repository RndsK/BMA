using System.Globalization;
using System.Text.Json;
using bma.Domain.Constants;
using bma.Domain.Entities;
using bma.Domain.Entities.RequestEntities;
using bma.Domain.Interfaces;
using bma.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace bma.Infrastructure.Repositories;

public class HolidayRequestRepository(
    BmaDbContext context,
    HttpClient httpClient)
    : Repository<HolidayRequest>(context), IHolidayRequestRepository
{
    private readonly BmaDbContext _context = context;

    /// <summary>
    /// Retrieves the bank holiday dates for a specified country.
    /// </summary>
    /// <param name="countryCode">The country code (e.g., "US", "GB") for which to retrieve bank holidays.</param>
    /// <returns>A task representing the asynchronous operation, containing a collection of bank holiday dates.</returns>
    /// <exception cref="HttpRequestException"></exception>
    /// <exception cref="TaskCanceledException"></exception>
    /// <exception cref="UriFormatException"></exception>
    /// <exception cref="JsonException"></exception>
    public async Task<IEnumerable<DateTime>> GetBankHolidayDatesAsync(string countryCode)
    {
        var year = DateTime.UtcNow.Year;
        var url = $"https://date.nager.at/api/v3/publicholidays/{year}/{countryCode}";
        var response = await httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            return Enumerable.Empty<DateTime>();
        }

        var content = await response.Content.ReadAsStringAsync();
        JsonDocument document = JsonDocument.Parse(content);
        var holidays = document.RootElement
            .EnumerateArray()
            .Select(element => DateTime.Parse(element.GetProperty("date").GetString()
                ?? throw new InvalidOperationException("Date property is missing"), CultureInfo.InvariantCulture))
            .ToList();
        document.Dispose();
        return holidays;
    }

    /// <summary>
    /// Retrieves the approved upcoming holidays for a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user for whom to retrieve upcoming holidays.</param>
    /// <returns>A task representing the asynchronous operation, containing a collection of approved upcoming holiday requests.</returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    public async Task<IEnumerable<HolidayRequest>> GetApprovedUpcomingHolidaysByUserIdAsync(string userId, int companyId)
    {
        if (string.IsNullOrEmpty(userId))
            throw new ArgumentException("User ID cannot be null or empty", nameof(userId));

        var currentDate = DateOnly.FromDateTime(DateTime.UtcNow);

        return await _context.Set<HolidayRequest>()
            .Where(hr => hr.UserId == userId && hr.StartDate > currentDate && hr.Status.Equals(StringDefinitions.RequestStatusApproved) && hr.CompanyId == companyId)
            .Include(hr => hr.Company)
            .Include(hr => hr.User)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves holiday requests for a specific company, optionally filtered by month.
    /// </summary>
    /// <param name="companyId">The ID of the company whose holidays should be retrieved.</param>
    /// <param name="searchMonth">An optional parameter to filter the holidays by month (1-12).</param>
    /// <returns>A task representing the asynchronous operation, containing a collection of holiday requests for the specified company and month if specified.</returns>
    /// <exception cref="OperationCanceledException"></exception>
    public async Task<IEnumerable<HolidayRequest>> GetHolidaysByCompanyAsync(int companyId, int? searchMonth)
    {
        return await _context.Set<HolidayRequest>()
            .Where(hr => hr.CompanyId == companyId && (!searchMonth.HasValue || hr.StartDate.Month == searchMonth.Value))
            .Include(hr => hr.Company)
            .Include(hr => hr.User)
            .ToListAsync();
    }

    /// <summary>
    /// Creates a new holiday request for a user and associates it with a specific company.
    /// </summary>
    /// <param name="userId">The ID of the user creating the holiday request.</param>
    /// <param name="companyId">The ID of the company associated with the holiday request.</param>
    /// <param name="createHolidayRequest">The holiday request entity containing the holiday details.</param>
    /// <returns>A task representing the asynchronous operation of creating the holiday request.</returns>
    /// <exception cref="DbUpdateException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    public async Task CreateHolidayRequestAsync(string userId, int companyId, HolidayRequest createHolidayRequest)
    {
        createHolidayRequest.CompanyId = companyId;
        createHolidayRequest.UserId = userId;

        await _context.HolidayRequests.AddAsync(createHolidayRequest);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Retrieves holiday requests based on user ID.
    /// </summary>
    /// <param name="userId">The user whose requests to return.</param>
    /// <returns>A task representing the asynchronous operation, containing a collection of holiday requests that match the specified expression.</returns>
    /// <exception cref="OperationCanceledException"></exception>
    public async Task<IEnumerable<HolidayRequest>> GetHolidaysByUserAsync(string userId, int companyId)
    {
        return await _context.Set<HolidayRequest>()
            .Where(e => e.UserId == userId && e.CompanyId == companyId)
            .Include(e => e.User)
            .AsNoTracking()
            .ToListAsync();
    }

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
    public async Task<HolidayBalance> CalculateHolidayBalanceAsync(string userId, int companyId, DateOnly? startWorkDate)
    {
        if (startWorkDate == null)
            throw new ArgumentNullException(nameof(startWorkDate), "Start work date is required.");

        var currentDate = DateTime.Now;
        var totalDays = (currentDate - startWorkDate.Value.ToDateTime(TimeOnly.MinValue)).Days;
        var totalEarnedHolidays = (int)Math.Ceiling(totalDays * (25 / 365f));

        var holidays = await _context.HolidayRequests
            .Where(h => h.UserId == userId && h.CompanyId == companyId)
            .ToListAsync();

        var usedHolidays = holidays.Sum(holiday =>
            (holiday.EndDate.ToDateTime(TimeOnly.MinValue) - holiday.StartDate.ToDateTime(TimeOnly.MinValue)).Days + 1);

        var overtimeRequests = await _context.OvertimeRequests
            .Where(o => o.UserId == userId)
            .ToListAsync();

        var overtimeDays = overtimeRequests.Sum(o => o.Length);

        var upcomingHolidays = holidays
            .Where(h => h.StartDate > DateOnly.FromDateTime(currentDate))
            .Sum(h => (h.EndDate.ToDateTime(TimeOnly.MinValue) - h.StartDate.ToDateTime(TimeOnly.MinValue)).Days + 1);

        var balance = totalEarnedHolidays - usedHolidays + overtimeDays;
        var balanceAfter = balance - upcomingHolidays;

        return new HolidayBalance
        {
            Balance = balance,
            UpcomingHolidays = upcomingHolidays,
            BalanceAfter = balanceAfter
        };
    }

}