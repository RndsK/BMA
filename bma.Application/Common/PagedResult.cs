namespace bma.Application.Common;

/// <summary>
/// Represents a paginated response containing a subset of data and metadata.
/// </summary>
public class PagedResult<T>
{
    /// <summary>
    /// Gets or sets the data items for the current page.
    /// </summary>
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();

    /// <summary>
    /// Gets or sets the total count of all matching items.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Gets or sets the current page number.
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Gets or sets the number of items per page.
    /// </summary>
    public int PageSize { get; set; }
}
