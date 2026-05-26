public sealed class NotificationConfiguration : IEntityTypeConfiguration<NotificationDomain.Notification>
{
    public void Configure(EntityTypeBuilder<NotificationDomain.Notification> builder)
    {
        builder.ToTable("notifications");

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Body)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.Type).HasConversion<int>();

        builder.HasIndex(x => x.UserId);
    }
}
