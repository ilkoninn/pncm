public sealed class PostLikeConfiguration : IEntityTypeConfiguration<PostLike>
{
    public void Configure(EntityTypeBuilder<PostLike> builder)
    {
        builder.ToTable("post_likes");
        builder.HasIndex(p => new { p.PostId, p.UserId }).IsUnique();
        builder.HasIndex(p => p.PostId);
    }
}
