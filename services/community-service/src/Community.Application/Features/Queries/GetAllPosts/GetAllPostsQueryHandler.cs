public sealed class GetAllPostsQueryHandler(IPostRepository repository, IMediaGrpcClient mediaGrpcClient)
    : IRequestHandler<GetAllPostsQuery, IEnumerable<PostResponseDto>>
{
    public async Task<IEnumerable<PostResponseDto>> Handle(GetAllPostsQuery request, CancellationToken cancellationToken)
    {
        var posts = await repository.GetAllAsync(cancellationToken);
        var postList = posts.ToList();

        if (postList.Count == 0)
            return [];

        var postIdsWithMedia = postList
            .Where(p => p.MediaIds.Count > 0)
            .Select(p => p.Id)
            .ToList();

        Dictionary<Guid, string> photoMap = [];
        if (postIdsWithMedia.Count > 0)
        {
            try { photoMap = await mediaGrpcClient.GetPrimaryPhotosAsync(postIdsWithMedia, ownerType: 3, cancellationToken); }
            catch { }
        }

        return postList.Select(p =>
        {
            var dto = p.Adapt<PostResponseDto>();
            photoMap.TryGetValue(p.Id, out var url);
            return dto with { PrimaryPhotoUrl = url };
        });
    }
}
