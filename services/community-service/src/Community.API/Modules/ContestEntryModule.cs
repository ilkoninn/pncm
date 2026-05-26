public class ContestEntryModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/contest-entries", Create);
        app.MapPost("/contest-entries/{id:guid}/score", GiveScore);
    }

    private static async Task<IResult> Create(CreateContestEntryRequestDto dto, ISender sender)
    {
        var command = new CreateContestEntryCommand(dto.ContestId, dto.PostId, dto.UserId);
        var result = await sender.Send(command);
        return Results.Created($"/contest-entries/{result.Id}", result);
    }

    private static async Task<IResult> GiveScore(Guid id, GiveScoreRequestDto dto, ISender sender)
    {
        var command = new GiveScoreCommand(id, dto.GivenByUserId);
        var result = await sender.Send(command);
        return Results.Ok(result);
    }
}
