public sealed class InviteConfiguration : IEntityTypeConfiguration<Invite>
{
    public void Configure(EntityTypeBuilder<Invite> builder)
    {
        builder.ToTable("invites");

        builder.Property(i => i.Token)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(i => i.Token)
            .IsUnique();
    }
}
