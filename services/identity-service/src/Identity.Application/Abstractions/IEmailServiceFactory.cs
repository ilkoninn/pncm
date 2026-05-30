public interface IEmailServiceFactory
{
    IEmailService Create(EEmailProvider provider);
}
