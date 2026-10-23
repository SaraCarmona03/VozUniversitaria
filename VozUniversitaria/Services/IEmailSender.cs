namespace VozUniversitaria.Services;

/// <summary>Abstracción para enviar mensajes de correo desde la aplicación.</summary>
public interface IEmailSender
{
    /// <summary>Envía un mensaje de texto al destinatario indicado.</summary>
    Task SendAsync(string to, string subject, string body);
}