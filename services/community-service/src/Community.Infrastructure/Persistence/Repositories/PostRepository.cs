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
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<(int LikesCount, bool IsLiked)> ToggleLikeAsync(Guid postId, Guid userId, CancellationToken cancellationToken = default)
    {
        var existing = await context.PostLikes
            .FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId, cancellationToken);

        if (existing is not null)
        {
            context.PostLikes.Remove(existing);
            await context.Posts
                .Where(p => p.Id == postId)
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.LikesCount, p => p.LikesCount - 1), cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            var count = await context.Posts.Where(p => p.Id == postId).Select(p => p.LikesCount).FirstOrDefaultAsync(cancellationToken);
            return (count, false);
        }
        else
        {
            await context.PostLikes.AddAsync(new PostLike { PostId = postId, UserId = userId }, cancellationToken);
            await context.Posts
                .Where(p => p.Id == postId)
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.LikesCount, p => p.LikesCount + 1), cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            var count = await context.Posts.Where(p => p.Id == postId).Select(p => p.LikesCount).FirstOrDefaultAsync(cancellationToken);
            return (count, true);
        }
    }

    public async Task<HashSet<Guid>> GetLikedPostIdsAsync(Guid userId, IEnumerable<Guid> postIds, CancellationToken cancellationToken = default)
        => [.. await context.PostLikes
            .Where(l => l.UserId == userId && postIds.Contains(l.PostId))
            .Select(l => l.PostId)
            .ToListAsync(cancellationToken)];
}
