public sealed class PetStoreConfiguration : IEntityTypeConfiguration<PetStore>
{
    public void Configure(EntityTypeBuilder<PetStore> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Address)
            .IsRequired();

        builder.Property(x => x.City)
            .IsRequired();

        builder.Property(x => x.Latitude)
            .HasPrecision(9, 6);

        builder.Property(x => x.Longitude)
            .HasPrecision(9, 6);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.PhoneNumber)
            .HasMaxLength(20);

        builder.HasIndex(x => x.City);
        builder.HasIndex(x => x.OwnerId);
        builder.HasIndex(x => new { x.Latitude, x.Longitude });

        builder.ToTable("stores");
    }
}