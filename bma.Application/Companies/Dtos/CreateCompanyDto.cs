using System.ComponentModel.DataAnnotations;

namespace bma.Application.Companies.Dtos;

/// <summary>
/// DTO for creating a company
/// </summary>
public class CreateCompanyDto
{
    /// <summary>
    /// Name of the company.
    /// </summary>
    public string? Name { get; set; } = string.Empty;

    /// <summary>
    /// The country the company is located.
    /// </summary>
    public string? Country { get; set; } = string.Empty;

    /// <summary>
    /// The companies industry.
    /// </summary>
    public string? Industry { get; set; } = string.Empty;

    /// <summary>
    /// Description for the company owner.
    /// </summary>
    public string? Description { get; set; } = string.Empty;

    /// <summary>
    /// The official company logo.
    /// </summary>
    public string? Logo { get; set; } = string.Empty;
}
