using Melodix.MVC.Services.Patterns.Adapter.SendGrid;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

public class SendGridAdapter:IMailProvider, IEmailSender
{
    private readonly string _apiKey;
    private readonly string _fromEmail;
    private readonly string _fromName;

    public SendGridAdapter(IConfiguration configuration)
    {
        _apiKey = Melodix.Keys.Keys.SendGridApiKey; // Usa la key del proyecto Melodix.Keys

        if(string.IsNullOrWhiteSpace(_apiKey))
            throw new Exception("No se encontró la clave SendGrid.");

        _fromEmail = configuration["SendGrid:FromEmail"] ?? "no-reply@tudominio.com";
        _fromName = configuration["SendGrid:FromName"] ?? "Melodix";
    }

    public async Task SendEmailAsync(string to, string subject, string htmlMessage)
    {
        var client = new SendGridClient(_apiKey);
        var from = new EmailAddress(_fromEmail, _fromName);
        var toEmail = new EmailAddress(to);
        var msg = MailHelper.CreateSingleEmail(
            from,
            toEmail,
            subject,
            plainTextContent: "Este correo requiere un cliente compatible con HTML.",
            htmlContent: htmlMessage
        );

        var response = await client.SendEmailAsync(msg);

        if(!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Body.ReadAsStringAsync();
            throw new Exception($"Error enviando correo: {response.StatusCode} - {errorBody}");
        }
    }

    async Task IEmailSender.SendEmailAsync(string email, string subject, string htmlMessage)
    {
        await SendEmailAsync(email, subject, htmlMessage);
    }
}