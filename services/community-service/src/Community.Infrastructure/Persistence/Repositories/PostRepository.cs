public sealed class PostRepository(CommunityDbContext context) : IPostRepository
{
    public async Task<Post> CreateAsync(Post post, CancellationToken cancellationToken = default)
    {
        await context.Posts.AddAsync(post, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return post;
    }

    public async Task<Post?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await context.Posts.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<IEnumerable<Post>> GetAllAsync(CancellationToken cancellationToken = default)
        => await context.Posts
            .Where(p => p.IsActive && !p.IsDeleted)
            .ToListAsync(cancellationToken);
}
