namespace bma.Infrastructure.Configuration;

/// <summary>
/// Configuration settings for Azure Blob Storage.
/// </summary>
public class BlobStorageSettings
{
    /// <summary>
    /// Gets or sets the connection string for the Azure Blob Storage account.
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the name of the container where attachments are stored.
    /// </summary>
    public string AttachmentContainerName { get; set; } = string.Empty;
}
