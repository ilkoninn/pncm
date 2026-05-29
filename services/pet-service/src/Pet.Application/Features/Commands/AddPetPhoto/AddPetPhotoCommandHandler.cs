public sealed class AddPetPhotoCommandHandler(IPetRepository petRepository)
    : IRequestHandler<AddPetPhotoCommand, PetPhotoResponseDto>
{
    public async Task<PetPhotoResponseDto> Handle(AddPetPhotoCommand request, CancellationToken cancellationToken)
    {
        var pet = await petRepository.GetByIdAsync(request.PetId, cancellationToken);

        if (pet is null)
            throw new KeyNotFoundException("Heyvan tapılmadı.");

        if (pet.OwnerId != request.RequesterId)
            throw new UnauthorizedAccessException("Bu əməliyyat üçün icazəniz yoxdur.");

        var photo = new PetPhoto
        {
            PetId = request.PetId,
            MediaId = request.MediaId,
            IsPrimary = request.IsPrimary
        };

        await petRepository.AddPhotoAsync(photo, cancellationToken);

        return photo.Adapt<PetPhotoResponseDto>();
    }
}
