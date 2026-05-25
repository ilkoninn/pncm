public class StoreModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/stores/{id:guid}", GetById);
        app.MapGet("/stores/nearby", GetNearby);
        app.MapGet("/stores", GetAll);
        app.MapPost("/stores", Create);
        app.MapPut("/stores/{id:guid}", Update);
        app.MapDelete("/stores/{id:guid}", Delete);
        app.MapPatch("/stores/{id:guid}/logo", UpdateLogo);
    }

    private static async Task<IResult> GetById(Guid id, IMediator mediator)
    {
        var result = await mediator.Send(new GetStoreByIdQuery(id));
        return Results.Ok(result);
    }

    private static async Task<IResult> GetNearby(
        decimal lat, decimal lng, double radiusKm, IMediator mediator)
    {
        var result = await mediator.Send(new GetNearbyStoresQuery(lat, lng, radiusKm));
        return Results.Ok(result);
    }

    private static async Task<IResult> GetAll(IMediator mediator)
    {
        var result = await mediator.Send(new GetAllStoresQuery());
        return Results.Ok(result);
    }

    private static async Task<IResult> Create(
        CreateStoreCommand command, IMediator mediator)
    {
        var result = await mediator.Send(command);
        return Results.Created($"/stores/{result.Id}", result);
    }

    private static async Task<IResult> Update(
        Guid id, UpdateStoreCommand command, IMediator mediator)
    {
        var result = await mediator.Send(command with { Id = id });
        return Results.Ok(result);
    }

    private static async Task<IResult> Delete(Guid id, IMediator mediator)
    {
        await mediator.Send(new DeleteStoreCommand(id));
        return Results.NoContent();
    }

    private static async Task<IResult> UpdateLogo(
        Guid id, UpdateStoreLogoCommand command, IMediator mediator)
    {
        await mediator.Send(command with { Id = id });
        return Results.NoContent();
    }
}