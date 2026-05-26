public sealed class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.ToTable("posts");

        builder.Property(p => p.Content)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(p => p.MediaIds)
            .HasColumnType("uuid[]");

        builder.HasIndex(p => p.UserId);
    }
}
