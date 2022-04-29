using System.Net;
using System.Net.Mail;
using ApiAuth.Common;
using Microsoft.Extensions.Options;

namespace ApiAuth.Services;

public interface IMailService
{
    void Send(string to, string subject, string body);
    Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken);
}

public class MailService: IMailService
{
    private readonly SmtpClient _client;
    private readonly AppSettings _appSettings;

    public MailService(IOptions<AppSettings> appSettingsOptions)
    {
        _appSettings = appSettingsOptions.Value;
        _client = new SmtpClient(_appSettings.EmailService.SmtpHost, _appSettings.EmailService.SmtpPort)
        {
            Credentials = new NetworkCredential(_appSettings.EmailService.Username,_appSettings.EmailService.Password),
            EnableSsl = _appSettings.EmailService.EnableSsl
        };
    }
    public void Send(string to, string subject, string body)
    {
        var message = new MailMessage(_appSettings.EmailService.SenderEmail, to, subject, body);
        _client.Send(message);
    }

    public Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken)
    {
        var message = new MailMessage(_appSettings.EmailService.SenderEmail, to, subject, body);
        return _client.SendMailAsync(message, cancellationToken);
    }
}