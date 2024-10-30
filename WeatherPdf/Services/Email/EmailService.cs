
using FluentEmail.Core;
using FluentEmail.Core.Models;

namespace WeatherPdf.Services.Email;

public class EmailService(IFluentEmail email) : IEmailService
{
    private readonly IFluentEmail _email = email;

    public async Task<bool> SendPdfReport(string email, Stream content, int month)
    {
        var task = await _email.To(email).Subject("Weather report").Body("Tashkent weather report for previous month")
            .Attach(new Attachment
            {
                ContentType = "application/pdf",
                Data = content,
                Filename = $"Report for {month}th month"

            }).SendAsync();

        return task.Successful ? true: false;
    }

    public async Task<bool> SendEmail(EmailMetadata data)
    {
        var task = await _email.To(data.ToAddress).Subject(data.Subject).Body(data.Body).SendAsync();

        return task.Successful ? true: false;
    }
}
