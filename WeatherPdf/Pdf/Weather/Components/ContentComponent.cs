using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using WeatherPdf.Database.Entities;

namespace WeatherPdf.Pdf.Weather.Component;

public class ContentComponent : IComponent
{
    private List<WeatherData> Weather { get; } = null!;
    public ContentComponent(List<WeatherData> weather)
    {
        Weather = weather;
    }
    public void Compose(IContainer container)
    {
        container.PaddingVertical(25).PaddingHorizontal(25).Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(50);
                columns.RelativeColumn(50);
                columns.RelativeColumn(50);
                columns.RelativeColumn(50);
                columns.RelativeColumn(50);
            });

            table.Header(header =>
            {
                header.Cell().Text("Число").Underline();
                header.Cell().Text("Утро").Underline();
                header.Cell().Text("День").Underline();
                header.Cell().Text("Вечер").Underline();
                header.Cell().Text("Ночь").Underline();
            });

            for (int i = 0; i < Weather.Count; i += 4)
            {
                table.Cell().Text(Weather[i].SearchedTime.Date.ToString("%d"));
                table.Cell().Text(Weather[i].Temperature.ToString());
                table.Cell().Text(Weather[i + 1].Temperature.ToString());
                table.Cell().Text(Weather[i + 2].Temperature.ToString());
                table.Cell().Text(Weather[i + 3].Temperature.ToString());
            }
        });
    }
}
