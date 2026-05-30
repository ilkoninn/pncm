public sealed class EmailServiceFactory(IServiceProvider sp) : IEmailServiceFactory
{
    public IEmailService Create(EEmailProvider provider) =>
        sp.GetRequiredKeyedService<IEmailService>(provider);
}
