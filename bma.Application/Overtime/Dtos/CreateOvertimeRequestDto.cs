namespace bma.Application.Overtime.Dtos;

/// <summary>
/// DTO for creating an overtime request.
/// </summary>
public class CreateOvertimeRequestDto
{
    /// <summary>
    /// The date for which the overtime is requested.
    /// </summary>
    public DateOnly? StartDate { get; set; }

    /// <summary>
    /// The amount of days the overtime will last.
    /// </summary>
    public int? Length { get; set; }

    /// <summary>
    /// The reason for overtime
    /// </summary>
    public string Description { get; set; } = string.Empty;
}