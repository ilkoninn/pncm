public sealed class UploadMediaBatchEndpoint(IMediator mediator)
    : Endpoint<UploadMediaBatchRequestDto, IEnumerable<MediaFileResponseDto>>
{
    public override void Configure()
    {
        Post("/media/upload/batch");
        AllowFileUploads();
    }

    public override async Task HandleAsync(UploadMediaBatchRequestDto req, CancellationToken ct)
    {
        if (Files.Count == 0)
        {
            AddError("files", "Heç bir fayl seçilməyib.");
            await Send.ErrorsAsync(400, ct);
            return;
        }

        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            AddError("auth", "İstifadəçi məlumatı tapılmadı.");
            await Send.ErrorsAsync(401, ct);
            return;
        }

        var ownerId = req.OwnerType == EOwnerType.User
            ? userId
            : req.OwnerId ?? userId;

        var results = new List<MediaFileResponseDto>();

        foreach (var file in Files)
        {
            var result = await mediator.Send(new UploadMediaCommand(
                file.OpenReadStream(),
                file.FileName,
                file.ContentType,
                file.Length,
                ownerId,
                req.OwnerType), ct);

            results.Add(result);
        }

        await Send.OkAsync(results, ct);
    }
}
