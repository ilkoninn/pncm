public sealed class UpdatePetStatusCommandValidator : AbstractValidator<UpdatePetStatusCommand>
{
    public UpdatePetStatusCommandValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Yanlış status dəyəri.");
    }
}
