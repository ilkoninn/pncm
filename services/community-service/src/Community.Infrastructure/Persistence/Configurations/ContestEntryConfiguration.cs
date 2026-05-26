public sealed class ContestEntryConfiguration : IEntityTypeConfiguration<ContestEntry>
{
    public void Configure(EntityTypeBuilder<ContestEntry> builder)
    {
        builder.ToTable("contest_entries");

        builder.HasIndex(e => e.ContestId);
        builder.HasIndex(e => e.UserId);

        builder.HasOne(e => e.Contest)
            .WithMany(c => c.Entries)
            .HasForeignKey(e => e.ContestId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
