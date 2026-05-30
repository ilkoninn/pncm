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

        HashSet<Guid> likedPostIds = [];
        if (request.UserId.HasValue)
        {
            likedPostIds = await repository.GetLikedPostIdsAsync(
                request.UserId.Value,
                postList.Select(p => p.Id),
                cancellationToken);
        }

        return postList.Select(p =>
        {
            var urls = p.MediaIds
                .Where(id => id != Guid.Empty && mediaUrlMap.ContainsKey(id))
                .Select(id => mediaUrlMap[id])
                .ToList();

            return new PostResponseDto(
                p.Id,
                p.UserId,
                p.PetId,
                p.Content,
                p.MediaIds,
                p.CreatedAt,
                p.AuthorName,
                p.AuthorAvatarUrl,
                PrimaryPhotoUrl: urls.FirstOrDefault(),
                MediaUrls: urls.Count > 0 ? urls : null,
                LikesCount: p.LikesCount,
                IsLiked: likedPostIds.Contains(p.Id)
            );
        });
    }
}
