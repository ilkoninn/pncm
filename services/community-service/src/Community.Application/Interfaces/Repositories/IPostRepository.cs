public interface IPostRepository
{
    Task<Post> CreateAsync(Post post, CancellationToken cancellationToken = default);
    Task<Post?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Post>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<(int LikesCount, bool IsLiked)> ToggleLikeAsync(Guid postId, Guid userId, CancellationToken cancellationToken = default);
    Task<HashSet<Guid>> GetLikedPostIdsAsync(Guid userId, IEnumerable<Guid> postIds, CancellationToken cancellationToken = default);
}
