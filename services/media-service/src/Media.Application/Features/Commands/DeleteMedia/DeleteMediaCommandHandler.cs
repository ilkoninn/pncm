public sealed class DeleteMediaCommandHandler(
    IStorageService storageService,
    IMediaRepository mediaRepository
) : IRequestHandler<DeleteMediaCommand, Unit>
{
    public async Task<Unit> Handle(
        DeleteMediaCommand request, CancellationToken cancellationToken)
    {
        var mediaFile = await mediaRepository.GetByIdAsync(request.Id, cancellationToken);

        if (mediaFile is null)
            throw new KeyNotFoundException("Media faylı tapılmadı.");

        await storageService.DeleteAsync(mediaFile.ObjectKey, mediaFile.BucketName, cancellationToken);
        await mediaRepository.DeleteAsync(request.Id, cancellationToken);

        return Unit.Value;
    }
}