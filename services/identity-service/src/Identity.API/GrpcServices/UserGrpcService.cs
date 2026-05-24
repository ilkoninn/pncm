public sealed class UserGrpcService(IUserRepository userRepository) 
    : Identity.API.Protos.UserGrpcService.UserGrpcServiceBase
{
    public override async Task<UserExistsResponse> UserExists(
        UserExistsRequest request, ServerCallContext context)
    {
        var userId = Guid.Parse(request.UserId);
        var user = await userRepository.GetByIdAsync(userId);
        return new UserExistsResponse { Exists = user is not null };
    }
}