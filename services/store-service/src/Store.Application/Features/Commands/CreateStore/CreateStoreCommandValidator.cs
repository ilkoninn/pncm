public sealed class CreateStoreCommandValidator : AbstractValidator<CreateStoreCommand>
{
    public CreateStoreCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Mağaza adı boş ola bilməz.")
            .MaximumLength(100).WithMessage("Mağaza adı 100 simvoldan çox ola bilməz.");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Ünvan boş ola bilməz.");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("Şəhər boş ola bilməz.");

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90).WithMessage("Enlik -90 ilə 90 arasında olmalıdır.");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180).WithMessage("Uzunluq -180 ilə 180 arasında olmalıdır.");

        RuleFor(x => x.OwnerId)
            .NotEmpty().WithMessage("Mağaza sahibi boş ola bilməz.");
    }
}