public sealed class ScoreEventConfiguration : IEntityTypeConfiguration<ScoreEvent>
{
    public void Configure(EntityTypeBuilder<ScoreEvent> builder)
    {
        builder.ToTable("score_events");

        builder.HasIndex(e => new { e.ContestEntryId, e.GivenByUserId })
            .IsUnique();

        builder.HasOne<ContestEntry>()
            .WithMany()
            .HasForeignKey(e => e.ContestEntryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
