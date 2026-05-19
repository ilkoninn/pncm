public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens");

        builder.Property(rt => rt.Token).HasMaxLength(256).IsRequired();
        builder.Property(rt => rt.ExpiresAt).IsRequired();

        builder.HasIndex(rt => rt.UserId);
        
        builder.HasOne(rt => rt.User)
            .WithMany()
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}