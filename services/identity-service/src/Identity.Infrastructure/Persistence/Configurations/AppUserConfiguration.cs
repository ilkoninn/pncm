public sealed class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.ToTable("users");

        builder.Property(u => u.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(u => u.LastName).HasMaxLength(100).IsRequired();
        builder.Property(u => u.AvatarMediaId);
        builder.Property(u => u.Bio).HasMaxLength(1000);
        builder.Property(u => u.City).HasMaxLength(100);
        
        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.PhoneNumber).IsUnique();
    }
}