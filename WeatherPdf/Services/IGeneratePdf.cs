using QuestPDF.Fluent;
using WeatherPdf.Database.Entities;
using WeatherPdf.Pdf.Weather.ContentModels;

namespace WeatherPdf.Services;

public interface IGeneratePdf
{
    Document CreatePdf(List<WeatherData> weather, HeaderContent content);
}
