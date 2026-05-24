public sealed class UserGrpcClient(IConfiguration configuration) : IUserGrpcClient
{
    public async Task<bool> UserExistsAsync(Guid userId)
    {
        var address = configuration["GrpcServices:IdentityService"]!;

        var channel = GrpcChannel.ForAddress(address);

        var client = new UserGrpcService.UserGrpcServiceClient(channel);

        var response = await client.UserExistsAsync(new UserExistsRequest
        {
            UserId = userId.ToString()
        });

        return response.Exists;
    }
}