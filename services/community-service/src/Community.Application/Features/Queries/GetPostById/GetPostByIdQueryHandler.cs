public sealed class GetPostByIdQueryHandler(IPostRepository repository, IMediaGrpcClient mediaGrpcClient)
    : IRequestHandler<GetPostByIdQuery, PostResponseDto>
{
    public async Task<PostResponseDto> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
    {
        var post = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Post tapılmadı.");

        var dto = post.Adapt<PostResponseDto>();

        if (post.MediaIds.Count == 0)
            return dto;

        List<string> urls = [];
        try { urls = await mediaGrpcClient.GetPhotoUrlsByOwnerAsync(post.Id, ownerType: 3, cancellationToken); }
        catch { }

        return dto with
        {
            MediaUrls = urls,
            PrimaryPhotoUrl = urls.Count > 0 ? urls[0] : null,
        };
    }
}
