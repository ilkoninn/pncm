public interface IStorageService
{
    Task<string> UploadAsync(Stream fileStream, string fileName, string contentType, string bucketName, CancellationToken cancellationToken = default);
    Task DeleteAsync(string objectKey, string bucketName, CancellationToken cancellationToken = default);
    Task<string> GetPresignedUrlAsync(string objectKey, string bucketName, int expirySeconds = 3600, CancellationToken cancellationToken = default);
}