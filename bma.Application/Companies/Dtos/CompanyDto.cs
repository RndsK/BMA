namespace bma.Application.Companies.Dtos;
/// <summary>
/// DTO for a company
/// </summary>
public class CompanyDto
{
    /// <summary>
    /// Primary key for the company
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Name of the company.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Name of the company owner.
    /// </summary>
    public string OwnerName { get; set; } = string.Empty;

    /// <summary>
    /// Description for the company owner.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The country the company is located.
    /// </summary>
    public string Country { get; set; } = string.Empty;

    /// <summary>
    /// Companies industry.
    /// </summary>
    public string Industry { get; set; } = string.Empty;

    /// <summary>
    /// The official company logo.
    /// </summary>
    public string? Logo { get; set; } = string.Empty;
}
