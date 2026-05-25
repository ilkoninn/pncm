public sealed class MediaFileConfiguration : IEntityTypeConfiguration<MediaFile>
{
    public void Configure(EntityTypeBuilder<MediaFile> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.FileName)
            .IsRequired();

        builder.Property(x => x.OriginalFileName)
            .IsRequired();

        builder.Property(x => x.ContentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Url)
            .IsRequired();

        builder.Property(x => x.BucketName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ObjectKey)
            .IsRequired();

        builder.HasIndex(x => x.OwnerId);
        builder.HasIndex(x => new { x.OwnerId, x.OwnerType });

        builder.ToTable("media_files");
    }
}