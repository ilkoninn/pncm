public sealed class AdoptionRequestConfiguration : IEntityTypeConfiguration<AdoptionRequest>
{
    public void Configure(EntityTypeBuilder<AdoptionRequest> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Message)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.ContactPhone)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.Status).HasConversion<int>();

        builder.HasIndex(x => x.PetId);
        builder.HasIndex(x => x.AdopterId);

        builder.ToTable("adoption_requests");
    }
}
