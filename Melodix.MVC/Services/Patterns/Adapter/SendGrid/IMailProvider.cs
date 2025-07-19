namespace Melodix.MVC.Services.Patterns.Adapter.SendGrid;

public interface IMailProvider
{
    Task SendEmailAsync(string to, string subject, string htmlMessage);
}