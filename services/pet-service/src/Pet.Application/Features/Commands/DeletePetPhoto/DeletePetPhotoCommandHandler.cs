public sealed class DeletePetPhotoCommandHandler(IPetRepository petRepository)
    : IRequestHandler<DeletePetPhotoCommand>
{
    public async Task Handle(DeletePetPhotoCommand request, CancellationToken cancellationToken)
    {
        var pet = await petRepository.GetByIdAsync(request.PetId, cancellationToken)
            ?? throw new KeyNotFoundException("Heyvan tapılmadı.");

        if (pet.OwnerId != request.RequesterId)
            throw new UnauthorizedAccessException("Bu əməliyyat üçün icazəniz yoxdur.");

        var photo = pet.Photos.FirstOrDefault(p => p.Id == request.PhotoId)
            ?? throw new KeyNotFoundException("Foto tapılmadı.");

        await petRepository.DeletePhotoAsync(photo.Id, cancellationToken);
    }
}
