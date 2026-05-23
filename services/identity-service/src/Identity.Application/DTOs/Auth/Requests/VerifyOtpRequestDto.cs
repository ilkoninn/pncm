public record VerifyOtpRequestDto(
    string Code,
    EOtpPurpose Purpose
);