namespace bma.Domain.Interfaces;

/// <summary>
/// Interface for blob storage operations.
/// </summary>
public interface IBlobStorageService
{
    /// <summary>
    /// Uploads a file to the specified blob storage container.
    /// </summary>
    /// <param name="data">The file data as a stream.</param>
    /// <param name="fileName">The name of the file to be stored in the blob container.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the URL of the uploaded file.</returns>
    Task<string> UploadFileAsync(Stream data, string fileName);

    /// <summary>
    /// Deletes a file from the specified blob storage container.
    /// </summary>
    /// <param name="blobName">The name of the blob to be deleted.</param>
    /// <param name="containerName">The name of the blob container from which the file will be deleted.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="Exception"></exception>
    Task DeleteFileAsync(string blobName, string containerName);
}
