public sealed class UpdatePetCommandValidator : AbstractValidator<UpdatePetCommand>
{
    public UpdatePetCommandValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("Ad 100 simvoldan çox ola bilməz.")
            .When(x => x.Name is not null);

        RuleFor(x => x.City)
            .MaximumLength(100).WithMessage("Şəhər 100 simvoldan çox ola bilməz.")
            .When(x => x.City is not null);
    }
}
