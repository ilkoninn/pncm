public interface IUserGrpcClient
{
    Task<bool> UserExistsAsync(Guid userId);
}