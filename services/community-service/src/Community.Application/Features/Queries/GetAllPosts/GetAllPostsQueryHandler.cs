public sealed class GetAllPostsQueryHandler(IPostRepository repository)
    : IRequestHandler<GetAllPostsQuery, IEnumerable<PostResponseDto>>
{
    public async Task<IEnumerable<PostResponseDto>> Handle(GetAllPostsQuery request, CancellationToken cancellationToken)
    {
        var posts = await repository.GetAllAsync(cancellationToken);

        return posts.Adapt<IEnumerable<PostResponseDto>>();
    }
}
