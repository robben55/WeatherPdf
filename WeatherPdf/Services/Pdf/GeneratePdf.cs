using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using WeatherPdf.Database.Entities;
using WeatherPdf.Pdf.Weather.Component;
using WeatherPdf.Pdf.Weather.Components;
using WeatherPdf.Pdf.Weather.ContentModels;
using WeatherPdf.Pdf.Weather.Elements;

namespace WeatherPdf.Services.Pf;

public class GeneratePdf : IGeneratePdf
{
    public Document CreatePdf(List<WeatherData> weather, HeaderContent content)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.DefaultTextStyle(x => x.FontSize(16));
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(16));

                page.Header().ShowOnce().Component(new HeaderComponent(content));

                page.Content().Component(new ContentComponent(weather));

                page.Footer()
                    .Element(FooterElements.Footer);
            });
        });
    }
}
