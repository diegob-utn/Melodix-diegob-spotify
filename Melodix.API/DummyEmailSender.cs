using Microsoft.AspNetCore.Identity.UI.Services;

public class DummyEmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        // Aqu� puedes registrar en consola, log, etc.
        return Task.CompletedTask;
    }
}