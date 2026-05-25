public sealed class MinioStorageService(
    [FromKeyedServices("internal")] IMinioClient internalClient,
    [FromKeyedServices("public")] IMinioClient publicClient) : IStorageService
{
    public async Task<string> UploadAsync(
        Stream fileStream, string objectKey, string contentType,
        string bucketName, CancellationToken cancellationToken = default)
    {
        var bucketExists = await internalClient.BucketExistsAsync(
            new BucketExistsArgs().WithBucket(bucketName), cancellationToken);

        if (!bucketExists)
            await internalClient.MakeBucketAsync(
                new MakeBucketArgs().WithBucket(bucketName), cancellationToken);

        await internalClient.PutObjectAsync(new PutObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectKey)
            .WithStreamData(fileStream)
            .WithObjectSize(fileStream.Length)
            .WithContentType(contentType), cancellationToken);

        return objectKey;
    }

    public async Task DeleteAsync(
        string objectKey, string bucketName, CancellationToken cancellationToken = default)
    {
        await internalClient.RemoveObjectAsync(new RemoveObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectKey), cancellationToken);
    }

    public async Task<string> GetPresignedUrlAsync(
        string objectKey, string bucketName, int expirySeconds = 3600, CancellationToken cancellationToken = default)
    {
        return await publicClient.PresignedGetObjectAsync(new PresignedGetObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectKey)
            .WithExpiry(expirySeconds));
    }
}