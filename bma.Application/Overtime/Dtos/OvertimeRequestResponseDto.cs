using bma.Application.Requests.Dtos;

namespace bma.Application.Overtime.Dtos;
/// <summary>
/// DTO for overtime request.
/// </summary>
public class OvertimeRequestResponseDto : RequestDto
{
    /// <summary>
    /// The date for which the overtime is requested.
    /// </summary>
    public DateOnly StartDate { get; set; }

    /// <summary>
    /// The amount of days the overtime will last.
    /// </summary>
    public int Length { get; set; }
}