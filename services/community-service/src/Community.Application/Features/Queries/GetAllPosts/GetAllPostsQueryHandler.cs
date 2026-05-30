public sealed class GetAllPostsQueryHandler(IPostRepository repository, IMediaGrpcClient mediaGrpcClient)
    : IRequestHandler<GetAllPostsQuery, IEnumerable<PostResponseDto>>
{
    public async Task<IEnumerable<PostResponseDto>> Handle(GetAllPostsQuery request, CancellationToken cancellationToken)
    {
        var posts = await repository.GetAllAsync(cancellationToken);
        var postList = posts.ToList();

        if (postList.Count == 0)
            return [];

        var postsWithMedia = postList.Where(p => p.MediaIds.Count > 0).ToList();
        var mediaUrlMap = new Dictionary<Guid, string>();

        if (postsWithMedia.Count > 0)
        {
            var userIds = postsWithMedia.Select(p => p.UserId).Distinct().ToList();
            var allNeededMediaIds = postsWithMedia.SelectMany(p => p.MediaIds).ToHashSet();

            foreach (var userId in userIds)
            {
                try
                {
                    var photos = await mediaGrpcClient.GetPhotoItemsByOwnerAsync(userId, 0, cancellationToken);
                    foreach (var (mediaId, url) in photos.Where(p => allNeededMediaIds.Contains(p.MediaId)))
                        mediaUrlMap[mediaId] = url;
                }
                catch { }
            }
        }

        return postList.Select(p =>
        {
            var dto = p.Adapt<PostResponseDto>();
            var primaryMediaId = p.MediaIds.FirstOrDefault();
            var primaryUrl = primaryMediaId != Guid.Empty && mediaUrlMap.TryGetValue(primaryMediaId, out var u) ? u : null;
            return dto with { PrimaryPhotoUrl = primaryUrl };
        });
    }
}
