using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace WeatherPdf.Pdf.Weather.Elements;

public static class FooterElements
{
    public static void Footer(IContainer container)
    {
        container
            .AlignCenter()
            .Text(x =>
            {
                x.CurrentPageNumber();
            });
    }
}
