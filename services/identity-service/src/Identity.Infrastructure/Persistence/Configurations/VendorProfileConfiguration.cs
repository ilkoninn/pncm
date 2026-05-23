public sealed class VendorProfileConfiguration : IEntityTypeConfiguration<VendorProfile>
{
    public void Configure(EntityTypeBuilder<VendorProfile> builder)
    {
        builder.ToTable("vendor_profiles");

        builder.Property(v => v.StoreName).HasMaxLength(200).IsRequired();
        builder.Property(v => v.Description).HasMaxLength(1000);
        builder.Property(v => v.Address).HasMaxLength(500);
        builder.Property(v => v.Latitude).HasPrecision(18, 6);
        builder.Property(v => v.Longitude).HasPrecision(18, 6);

        builder.HasIndex(v => v.UserId).IsUnique();
        builder.HasIndex(v => v.StoreName);
        builder.HasIndex(v => new { v.Latitude, v.Longitude });

        builder.HasOne(v => v.User)
            .WithOne(u => u.VendorProfile)
            .HasForeignKey<VendorProfile>(v => v.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}