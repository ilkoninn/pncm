public interface IPostRepository
{
    Task<Post> CreateAsync(Post post, CancellationToken cancellationToken = default);
    Task<Post?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Post>> GetAllAsync(CancellationToken cancellationToken = default);
}
