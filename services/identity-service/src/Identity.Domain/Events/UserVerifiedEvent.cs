public class UserVerifiedEvent : BaseEvent
{
    public Guid UserId { get; }
    public string Email { get; }

    public UserVerifiedEvent(Guid userId, string email)
    {
        UserId = userId;
        Email = email;
    }
}