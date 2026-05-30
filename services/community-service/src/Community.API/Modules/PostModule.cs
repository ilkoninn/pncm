public class PostModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/posts", Create).RequireAuthorization();
        app.MapGet("/posts/{id:guid}", GetById);
        app.MapGet("/posts", GetAll);
    }

    private static async Task<IResult> Create(CreatePostRequestDto dto, ISender sender, HttpContext httpContext)
    {
        var userId = Guid.Parse(httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var firstName = httpContext.User.FindFirstValue(ClaimTypes.GivenName) ?? "";
        var lastName = httpContext.User.FindFirstValue(ClaimTypes.Surname) ?? "";
        var authorName = $"{firstName} {lastName}".Trim();

        var command = new CreatePostCommand(userId, dto.PetId, dto.Content, dto.MediaIds, authorName, dto.AuthorAvatarUrl);
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
