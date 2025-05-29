namespace bma.Domain.Entities;

public class HolidayBalance
{
    /// <summary>
    /// The current holiday balance.
    /// </summary>
    public int Balance { get; set; }

    /// <summary>
    /// The total number of upcoming holiday days.
    /// </summary>
    public int UpcomingHolidays { get; set; }

    /// <summary>
    /// The holiday balance after upcoming holidays are deducted.
    /// </summary>
    public int BalanceAfter { get; set; }
}
