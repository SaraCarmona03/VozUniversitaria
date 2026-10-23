using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace VozUniversitaria.Services;

/// <summary>Envía correos de texto usando los parámetros SMTP de la configuración.</summary>
public class SmtpEmailSender : IEmailSender
{
    private readonly IConfiguration _config;

    /// <summary>Recibe la configuración de host, puerto y credenciales SMTP.</summary>
    public SmtpEmailSender(IConfiguration config) => _config = config;

    /// <summary>Construye el mensaje, se autentica cuando hay usuario y lo envía.</summary>
    public async Task SendAsync(string to, string subject, string body)
    {
        var host = _config["Smtp:Host"]
            ?? throw new InvalidOperationException("Smtp:Host is not configured.");
        if (!int.TryParse(_config["Smtp:Port"], out var port))
        {
            throw new InvalidOperationException("Smtp:Port must be configured as a valid port number.");
        }

        var from = _config["Smtp:From"] ?? "no-reply@vozuniversitaria.local";

        // El correo se compone en texto plano para evitar interpretar contenido HTML.
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("VozUniversitaria", from));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;
        message.Body = new TextPart("plain") { Text = body };

        using var client = new SmtpClient();
        await client.ConnectAsync(host, port, SecureSocketOptions.Auto);

        var user = _config["Smtp:User"];
        if (!string.IsNullOrEmpty(user))
        {
            var password = _config["Smtp:Password"]
                ?? throw new InvalidOperationException("Smtp:Password is required when Smtp:User is configured.");
            await client.AuthenticateAsync(user, password);
        }

        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}