using System.ComponentModel.DataAnnotations;

namespace bma.Domain.Entities.RequestEntities;
/// <summary>
/// A child class from Request to specify additional information about an overtime request
/// </summary>
public class OvertimeRequest : Request
{
    /// <summary>
    /// Gets or sets the date at which the overtime will start
    /// </summary>
    [Required]
    public DateOnly StartDate { get; set; }

    /// <summary>
    /// Gets or sets the amount of days overtime will last
    /// </summary>
    [Required]
    public int Length { get; set; } = 1;
}