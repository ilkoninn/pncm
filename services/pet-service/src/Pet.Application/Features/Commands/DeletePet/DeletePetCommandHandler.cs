public sealed class DeletePetCommandHandler(IPetRepository petRepository)
    : IRequestHandler<DeletePetCommand>
{
    public async Task Handle(DeletePetCommand request, CancellationToken cancellationToken)
    {
        var pet = await petRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Heyvan tapılmadı.");

        await petRepository.DeleteAsync(pet.Id, cancellationToken);
    }
}
