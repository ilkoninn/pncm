public sealed class UpdateAdoptionStatusCommandValidator : AbstractValidator<UpdateAdoptionStatusCommand>
{
    public UpdateAdoptionStatusCommandValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Yanlış status dəyəri.");
    }
}
