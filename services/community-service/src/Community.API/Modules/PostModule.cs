public class PostModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/posts", Create);
        app.MapGet("/posts/{id:guid}", GetById);
        app.MapGet("/posts", GetAll);
    }

    private static async Task<IResult> Create(CreatePostRequestDto dto, ISender sender)
    {
        var command = new CreatePostCommand(dto.UserId, dto.PetId, dto.Content, dto.MediaIds);
        var result = await sender.Send(command);
        return Results.Created($"/posts/{result.Id}", result);
    }

    private static async Task<IResult> GetById(Guid id, ISender sender)
    {
        var result = await sender.Send(new GetPostByIdQuery(id));
        return Results.Ok(result);
    }

    private static async Task<IResult> GetAll(ISender sender)
    {
        var result = await sender.Send(new GetAllPostsQuery());
        return Results.Ok(result);
    }
}
