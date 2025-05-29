using Azure.Storage.Blobs;
using bma.Domain.Interfaces;
using bma.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace bma.Infrastructure.Storage;

/// <summary>
/// Service for handling Azure Blob Storage operations.
/// </summary>
public class BlobStorageService : IBlobStorageService
{
    private readonly BlobStorageSettings _blobStorageSettings;
    private readonly BlobServiceClient _blobServiceClient;

    public BlobStorageService(IOptions<BlobStorageSettings> blobStorageSettingsOptions)
    {
        _blobStorageSettings = blobStorageSettingsOptions.Value;

        _blobServiceClient = new BlobServiceClient(_blobStorageSettings.ConnectionString);
    }

    /// <exception cref="Exception"></exception>
    public async Task DeleteFileAsync(string blobName, string containerName)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            var blobClient = containerClient.GetBlobClient(blobName);

            await blobClient.DeleteIfExistsAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to delete file: {ex.Message}", ex);
        }
    }

    public async Task<string> UploadFileAsync(Stream data, string fileName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_blobStorageSettings.AttachmentContainerName);

        var blobClient = containerClient.GetBlobClient(fileName);

        await blobClient.UploadAsync(data);

        var blobUrl = blobClient.Uri.ToString();

        return blobUrl;
    }
}

