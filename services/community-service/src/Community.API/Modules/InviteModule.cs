public class InviteModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/invites", Create);
        app.MapGet("/invites/{token}", GetByToken);
    }

    private static async Task<IResult> Create(CreateInviteRequestDto dto, ISender sender)
    {
        var command = new CreateInviteCommand(dto.ContestId, dto.InviterId);
        var result = await sender.Send(command);
        return Results.Created($"/invites/{result.Token}", result);
    }

    private static async Task<IResult> GetByToken(string token, ISender sender)
    {
        var result = await sender.Send(new GetInviteByTokenQuery(token));
        return Results.Ok(result);
    }
}
