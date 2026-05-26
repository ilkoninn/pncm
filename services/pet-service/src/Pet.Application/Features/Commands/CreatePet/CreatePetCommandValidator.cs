public sealed class CreatePetCommandValidator : AbstractValidator<CreatePetCommand>
{
    public CreatePetCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Ad boş ola bilməz.")
            .MaximumLength(100).WithMessage("Ad 100 simvoldan çox ola bilməz.");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("Şəhər boş ola bilməz.")
            .MaximumLength(100).WithMessage("Şəhər 100 simvoldan çox ola bilməz.");

        RuleFor(x => x.OwnerId)
            .NotEmpty().WithMessage("Sahib seçilməlidir.");
    }
}
