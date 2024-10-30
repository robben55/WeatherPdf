using FluentEmail.Core.Models;

namespace WeatherPdf.Services.Email;

public class EmailMetadata
{
    public string ToAddress { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string? Body { get; set; }
    public Attachment? Attachment { get; set; } 

}
