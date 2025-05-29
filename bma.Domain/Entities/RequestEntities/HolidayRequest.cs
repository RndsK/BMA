using System.ComponentModel.DataAnnotations;

namespace bma.Domain.Entities.RequestEntities;
/// <summary>
/// A child class from Request to specify additional information about a holiday request
/// </summary>
public class HolidayRequest : Request
{
    /// <summary>
    /// Gets or sets the date at which the holiday will start
    /// </summary>
    [Required]
    public DateOnly StartDate { get; set; }

    /// <summary>
    /// Gets or sets the date at which the holiday will end
    /// </summary>
    [Required]
    public DateOnly EndDate { get; set; }
}