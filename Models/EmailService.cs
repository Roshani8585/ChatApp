using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;

public class EmailService
{
    private readonly EmailSettings _settings;

    public EmailService(IOptions<EmailSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task SendInviteEmail(string toEmail, string inviteLink)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_settings.Email));
        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = "Chat App Invitation by roshani";

        email.Body = new TextPart("html")
        {
            Text = $"<h2>You are invited!</h2><p>Click below to join:</p><a href='{inviteLink}'>Join Chat</a>"
        };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_settings.Host, _settings.Port, MailKit.Security.SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_settings.Email, _settings.Password);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}