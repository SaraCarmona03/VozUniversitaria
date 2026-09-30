using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace VozUniversitaria.Services;

public class SmtpEmailSender : IEmailSender
{
    private readonly IConfiguration _config;

    public SmtpEmailSender(IConfiguration config) => _config = config;

    public async Task SendAsync(string to, string subject, string body)
    {
        var host = _config["Smtp:Host"]
            ?? throw new InvalidOperationException("Smtp:Host is not configured.");
        var port = int.Parse(_config["Smtp:Port"] ?? "25");
        var from = _config["Smtp:From"] ?? "no-reply@vozuniversitaria.local";

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("VozUniversitaria", from));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;
        message.Body = new TextPart("plain") { Text = body };

        using var client = new SmtpClient();
        await client.ConnectAsync(host, port, SecureSocketOptions.Auto);

        var user = _config["Smtp:User"];
        if (!string.IsNullOrEmpty(user))
         await client.AuthenticateAsync(user, _config["Smtp:Password"] ?? string.Empty);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}