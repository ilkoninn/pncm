public sealed class InviteRepository(CommunityDbContext context) : IInviteRepository
{
    public async Task<Invite> CreateAsync(Invite invite, CancellationToken cancellationToken = default)
    {
        await context.Invites.AddAsync(invite, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return invite;
    }

    public async Task<Invite?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
        => await context.Invites.FirstOrDefaultAsync(i => i.Token == token, cancellationToken);
}
