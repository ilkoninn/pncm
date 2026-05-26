public sealed class PetPhotoConfiguration : IEntityTypeConfiguration<PetPhoto>
{
    public void Configure(EntityTypeBuilder<PetPhoto> builder)
    {
        builder.HasKey(x => x.Id);

        builder.ToTable("pet_photos");
    }
}
