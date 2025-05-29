using System.ComponentModel.DataAnnotations;

namespace bma.Application.Requests.Dtos;
/// <summary>
/// Base DTO for requests.
/// </summary>
public class RequestDto
{
    /// <summary>
    /// The type of request it is.
    /// </summary>
    public string RequestType { get; set; } = string.Empty;
    /// <summary>
    /// The ID of the request.
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// The current status of the request.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// The name of the user who made the request.
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// A description for the request, will include different information
    /// about the request depending on what kind of request it is
    /// </summary>
    public string Description { get; set; } = string.Empty;
}