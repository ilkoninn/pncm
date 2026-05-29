public class PetModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/pets", GetAll);
        app.MapGet("/pets/{id:guid}", GetById);
        app.MapGet("/pets/slug/{slug}", GetBySlug);
        app.MapGet("/pets/nearby", GetNearby);
        app.MapGet("/pets/owner", GetByOwner);
        app.MapPost("/pets", Create);
        app.MapPut("/pets/{id:guid}", Update);
        app.MapDelete("/pets/{id:guid}", Delete);
        app.MapPatch("/pets/{id:guid}/status", UpdateStatus);
        app.MapPost("/pets/{id:guid}/photos", AddPhoto);
    }

    private static async Task<IResult> GetAll(
        ClaimsPrincipal user,
        IMediator mediator,
        string? city = null,
        int? species = null,
        int? gender = null,
        int? size = null,
        bool? isVaccinated = null,
        bool? isNeutered = null)
    {
        Guid? excludeOwnerId = null;
        var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(userIdClaim, out var userId))
            excludeOwnerId = userId;

        var result = await mediator.Send(new GetAllPetsQuery(
            city,
            species.HasValue ? (ESpecies)species.Value : null,
            gender.HasValue  ? (EGender)gender.Value   : null,
            size.HasValue    ? (EPetSize)size.Value     : null,
            isVaccinated,
            isNeutered,
            excludeOwnerId));
        return Results.Ok(result);
    }

    private static async Task<IResult> GetById(Guid id, IMediator mediator)
    {
        var result = await mediator.Send(new GetPetByIdQuery(id));
        return Results.Ok(result);
    }

    private static async Task<IResult> GetBySlug(string slug, IMediator mediator)
    {
        var result = await mediator.Send(new GetPetBySlugQuery(slug));
        return Results.Ok(result);
    }

    private static async Task<IResult> GetNearby(decimal lat, decimal lng, double radiusKm, IMediator mediator)
    {
        var result = await mediator.Send(new GetNearbyPetsQuery(lat, lng, radiusKm));
        return Results.Ok(result);
    }

    private static async Task<IResult> GetByOwner(ClaimsPrincipal user, IMediator mediator, string? type = null)
    {
        var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdClaim, out var userId))
            return Results.Unauthorized();
        var result = await mediator.Send(new GetPetsByOwnerQuery(userId, type));
        return Results.Ok(result);
    }

    private static async Task<IResult> Create(ClaimsPrincipal user, CreatePetRequestDto dto, IMediator mediator)
    {
        var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdClaim, out var userId))
            return Results.Unauthorized();
        var firstName = user.FindFirstValue(ClaimTypes.GivenName);
        var lastName  = user.FindFirstValue(ClaimTypes.Surname);
        var result = await mediator.Send(new CreatePetCommand(
            dto.Name, dto.Species, dto.Breed, dto.AgeMonths,
            dto.Gender, dto.Size, dto.Color, dto.Description,
            dto.IsVaccinated, dto.IsNeutered, userId,
            dto.City, dto.Latitude, dto.Longitude, dto.Status,
            firstName, lastName));
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

    private static async Task<IResult> AddPhoto(Guid id, ClaimsPrincipal user, AddPetPhotoRequestDto dto, IMediator mediator)
    {
        var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdClaim, out var userId))
            return Results.Unauthorized();
        var result = await mediator.Send(new AddPetPhotoCommand(id, userId, dto.MediaId, dto.IsPrimary));
        return Results.Created($"/pets/{id}/photos/{result.Id}", result);
    }
}
