public sealed class GetPostByIdQueryHandler(IPostRepository repository)
    : IRequestHandler<GetPostByIdQuery, PostResponseDto>
{
    public async Task<PostResponseDto> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
    {
        var post = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Post tapılmadı.");

        return post.Adapt<PostResponseDto>();
    }
}
