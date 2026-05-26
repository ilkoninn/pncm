public class ContestModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/contests", Create);
        app.MapGet("/contests/{id:guid}", GetById);
        app.MapGet("/contests", GetAll);
        app.MapGet("/contests/{id:guid}/leaderboard", GetLeaderboard);
        app.MapPatch("/contests/{id:guid}/end", EndContest);
    }

    private static async Task<IResult> Create(CreateContestRequestDto dto, ISender sender)
    {
        var command = new CreateContestCommand(dto.Title, dto.Description, dto.StartDate, dto.EndDate, dto.Prize);
        var result = await sender.Send(command);
        return Results.Created($"/contests/{result.Id}", result);
    }

    private static async Task<IResult> GetById(Guid id, ISender sender)
    {
        var result = await sender.Send(new GetContestByIdQuery(id));
        return Results.Ok(result);
    }

    private static async Task<IResult> GetAll(ISender sender)
    {
        var result = await sender.Send(new GetAllContestsQuery());
        return Results.Ok(result);
    }

    private static async Task<IResult> GetLeaderboard(Guid id, ISender sender)
    {
        var result = await sender.Send(new GetLeaderboardQuery(id));
        return Results.Ok(result);
    }

    private static async Task<IResult> EndContest(Guid id, ISender sender)
    {
        var result = await sender.Send(new EndContestCommand(id));
        return Results.Ok(result);
    }
}
