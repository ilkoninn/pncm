public sealed record GetAllPostsQuery(Guid? UserId = null) : IRequest<IEnumerable<PostResponseDto>>;
