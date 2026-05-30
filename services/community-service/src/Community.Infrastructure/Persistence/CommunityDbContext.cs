public class CommunityDbContext(DbContextOptions<CommunityDbContext> options) : DbContext(options)
{
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<PostLike> PostLikes => Set<PostLike>();
    public DbSet<Contest> Contests => Set<Contest>();
    public DbSet<ContestEntry> ContestEntries => Set<ContestEntry>();
    public DbSet<Invite> Invites => Set<Invite>();
    public DbSet<ScoreEvent> ScoreEvents => Set<ScoreEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CommunityDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
