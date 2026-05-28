public sealed class UploadMediaEndpoint(IMediator mediator) : Endpoint<UploadMediaRequestDto, MediaFileResponseDto>
{
    public override void Configure()
    {
        Post("/media/upload");
        AllowFileUploads();
    }

    public override async Task HandleAsync(UploadMediaRequestDto req, CancellationToken ct)
    {
        var file = Files.FirstOrDefault();

        if (file is null)
        {
            AddError("file", "Fayl seçilməyib.");
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

        var result = await mediator.Send(new UploadMediaCommand(
            file.OpenReadStream(),
            file.FileName,
            file.ContentType,
            file.Length,
            userId,
            req.OwnerType
        ), ct);

        await Send.CreatedAtAsync<GetMediaByIdEndpoint>(
            new { id = result.Id }, result, cancellation: ct);
    }
}