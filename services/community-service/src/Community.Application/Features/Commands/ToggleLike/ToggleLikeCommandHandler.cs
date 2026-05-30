public sealed class ToggleLikeCommandHandler(IPostRepository repository)
    : IRequestHandler<ToggleLikeCommand, ToggleLikeResponseDto>
{
    public async Task<ToggleLikeResponseDto> Handle(ToggleLikeCommand request, CancellationToken cancellationToken)
    {
        var post = await repository.GetByIdAsync(request.PostId, cancellationToken)
            ?? throw new KeyNotFoundException("Post tapılmadı.");

        var (likesCount, isLiked) = await repository.ToggleLikeAsync(post.Id, request.UserId, cancellationToken);
        return new ToggleLikeResponseDto(likesCount, isLiked);
    }
}
