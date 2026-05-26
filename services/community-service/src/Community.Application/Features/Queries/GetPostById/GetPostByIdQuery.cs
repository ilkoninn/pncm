public sealed record GetPostByIdQuery(Guid Id) : IRequest<PostResponseDto>;
