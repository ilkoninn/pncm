public sealed class CreatePostCommandHandler(IPostRepository repository)
    : IRequestHandler<CreatePostCommand, PostResponseDto>
{
    public async Task<PostResponseDto> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        var post = new Post
        {
            UserId = request.UserId,
            PetId = request.PetId,
            Content = request.Content,
            MediaIds = request.MediaIds
        };

        await repository.CreateAsync(post, cancellationToken);

        return post.Adapt<PostResponseDto>();
    }
}
