public sealed class CreateAdoptionCommandValidator : AbstractValidator<CreateAdoptionCommand>
{
    public CreateAdoptionCommandValidator()
    {
        RuleFor(x => x.PetId)
            .NotEmpty().WithMessage("Heyvan seçilməlidir.");

        RuleFor(x => x.AdopterId)
            .NotEmpty().WithMessage("Götürən şəxs seçilməlidir.");

        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Mesaj boş ola bilməz.")
            .MaximumLength(1000).WithMessage("Mesaj 1000 simvoldan çox ola bilməz.");

        RuleFor(x => x.ContactPhone)
            .NotEmpty().WithMessage("Əlaqə nömrəsi boş ola bilməz.")
            .MaximumLength(20).WithMessage("Əlaqə nömrəsi 20 simvoldan çox ola bilməz.");
    }
}
