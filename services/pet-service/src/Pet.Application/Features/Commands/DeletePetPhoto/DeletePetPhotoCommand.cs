public sealed record DeletePetPhotoCommand(Guid PetId, Guid PhotoId, Guid RequesterId) : IRequest;
