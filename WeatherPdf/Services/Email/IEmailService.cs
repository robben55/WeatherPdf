using FluentEmail.Core.Models;

namespace WeatherPdf.Services.Email;

public interface IEmailService
{
    Task<bool> SendPdfReport(string email, Stream content, int month);

    Task<bool> SendEmail(EmailMetadata data);
}
