using bma.Application.Requests.Dtos;

namespace bma.Application.Holidays.Dtos;
/// <summary>
/// DTO for a holiday.
/// </summary>
public class HolidayRequestResponseDto : RequestDto
{
    /// <summary>
    /// The date at which the holiday will start.
    /// </summary>
    public DateOnly StartDate { get; set; }

    /// <summary>
    /// The date at which the holiday will end.
    /// </summary>
    public DateOnly EndDate { get; set; }
}