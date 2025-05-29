namespace bma.Application.Holidays.Dtos;

/// <summary>
/// DTO for creating a holiday request.
/// </summary>
public class CreateHolidayRequestDto
{
    /// <summary>
    /// Start date of the holiday.
    /// </summary>
    public DateOnly? StartDate { get; set; }

    /// <summary>
    /// End date of the holiday.
    /// </summary>
    public DateOnly? EndDate { get; set; }

    /// <summary>
    /// The reason for taking the holiday.
    /// </summary>
    public string? Description { get; set; } = string.Empty;
}