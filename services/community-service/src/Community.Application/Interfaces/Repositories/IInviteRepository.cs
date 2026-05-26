public interface IInviteRepository
{
    Task<Invite> CreateAsync(Invite invite, CancellationToken cancellationToken = default);
    Task<Invite?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
}
