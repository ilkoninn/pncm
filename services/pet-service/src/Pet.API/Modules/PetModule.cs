public class PetModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/pets", GetAll);
        app.MapGet("/pets/{id:guid}", GetById);
        app.MapGet("/pets/nearby", GetNearby);
        app.MapGet("/pets/owner", GetByOwner);
        app.MapPost("/pets", Create);
        app.MapPut("/pets/{id:guid}", Update);
        app.MapDelete("/pets/{id:guid}", Delete);
        app.MapPatch("/pets/{id:guid}/status", UpdateStatus);
        app.MapPost("/pets/{id:guid}/photos", AddPhoto);
    }

    private static async Task<IResult> GetAll(IMediator mediator)
    {
        var result = await mediator.Send(new GetAllPetsQuery());
        return Results.Ok(result);
    }

    private static async Task<IResult> GetById(Guid id, IMediator mediator)
    {
        var result = await mediator.Send(new GetPetByIdQuery(id));
        return Results.Ok(result);
    }

    private static async Task<IResult> GetNearby(decimal lat, decimal lng, double radiusKm, IMediator mediator)
    {
        var result = await mediator.Send(new GetNearbyPetsQuery(lat, lng, radiusKm));
        return Results.Ok(result);
    }

    private static async Task<IResult> GetByOwner(Guid ownerId, EOwnerType ownerType, IMediator mediator)
    {
        var result = await mediator.Send(new GetPetsByOwnerQuery(ownerId, ownerType));
        return Results.Ok(result);
    }

    private static async Task<IResult> Create(CreatePetRequestDto dto, IMediator mediator)
    {
        var result = await mediator.Send(new CreatePetCommand(
            dto.Name, dto.Species, dto.Breed, dto.AgeMonths,
            dto.Gender, dto.Size, dto.Color, dto.Description,
            dto.IsVaccinated, dto.IsNeutered, dto.OwnerId,
            dto.OwnerType, dto.City, dto.Latitude, dto.Longitude));
        return Results.Created($"/pets/{result.Id}", result);
    }

    private static async Task<IResult> Update(Guid id, UpdatePetRequestDto dto, IMediator mediator)
    {
        var result = await mediator.Send(new UpdatePetCommand(
            id, dto.Name, dto.Breed, dto.AgeMonths, dto.Color, dto.Description,
            dto.IsVaccinated, dto.IsNeutered, dto.City, dto.Latitude, dto.Longitude));
        return Results.Ok(result);
    }

    private static async Task<IResult> Delete(Guid id, IMediator mediator)
    {
        await mediator.Send(new DeletePetCommand(id));
        return Results.NoContent();
    }

    private static async Task<IResult> UpdateStatus(Guid id, UpdatePetStatusRequestDto dto, IMediator mediator)
    {
        var result = await mediator.Send(new UpdatePetStatusCommand(id, dto.Status));
        return Results.Ok(result);
    }

    private static async Task<IResult> AddPhoto(Guid id, AddPetPhotoRequestDto dto, IMediator mediator)
    {
        var result = await mediator.Send(new AddPetPhotoCommand(id, dto.MediaId, dto.IsPrimary));
        return Results.Created($"/pets/{id}/photos/{result.Id}", result);
    }
}
