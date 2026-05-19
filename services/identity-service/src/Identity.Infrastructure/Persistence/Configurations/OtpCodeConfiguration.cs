public class OtpCodeConfiguration : IEntityTypeConfiguration<OtpCode>
{
    public void Configure(EntityTypeBuilder<OtpCode> builder)
    {
        builder.ToTable("otp_codes");

        builder.Property(o => o.Code).HasMaxLength(6).IsRequired();
        builder.Property(o => o.ExpiresAt).IsRequired();

        builder.HasIndex(o => o.UserId);
        
        builder.HasOne(o => o.User)
            .WithMany()
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}