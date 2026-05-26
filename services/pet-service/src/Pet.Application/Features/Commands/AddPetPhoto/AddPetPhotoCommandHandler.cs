public sealed class AddPetPhotoCommandHandler(IPetRepository petRepository)
    : IRequestHandler<AddPetPhotoCommand, PetPhotoResponseDto>
{
    public async Task<PetPhotoResponseDto> Handle(AddPetPhotoCommand request, CancellationToken cancellationToken)
    {
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
