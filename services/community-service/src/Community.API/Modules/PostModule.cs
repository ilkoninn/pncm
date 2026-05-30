public class PostModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/posts", Create).RequireAuthorization();
        app.MapGet("/posts/{id:guid}", GetById);
        app.MapGet("/posts", GetAll);
        app.MapPost("/posts/{id:guid}/like", ToggleLike).RequireAuthorization();
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

    private static async Task<IResult> GetAll(ISender sender, HttpContext httpContext)
    {
        Guid? userId = null;
        if (httpContext.User.Identity?.IsAuthenticated == true)
            userId = Guid.Parse(httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await sender.Send(new GetAllPostsQuery(userId));
        return Results.Ok(result);
    }

    private static async Task<IResult> ToggleLike(Guid id, ISender sender, HttpContext httpContext)
    {
        var userId = Guid.Parse(httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await sender.Send(new ToggleLikeCommand(id, userId));
        return Results.Ok(result);
    }
}
