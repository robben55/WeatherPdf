using Microsoft.AspNetCore.Components.Web;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using WeatherPdf.Pdf.Weather.ContentModels;

namespace WeatherPdf.Pdf.Weather.Components;

public class HeaderComponent : IComponent
{
    private HeaderContent Content { get; }
    public HeaderComponent(HeaderContent content)
    {
        Content = content;
    }
    public void Compose(IContainer container)
    {
        container.Row(row =>
        {
            row.ConstantItem(100).Image("./Pdf/Weather/Images/pic.png");
            row.RelativeItem().Column(column =>
            {
                column.Item().Text("Weather Report").AlignCenter().FontSize(30)
                        .FontColor(Colors.Orange.Medium)
                        .SemiBold();

                column.Item().Row(row =>
                {
                    row.ConstantItem(20).Column(column =>
                    {
                        row.Spacing(10);
                        column.Item().Image("./Pdf/Weather/Images/geo.png");
                    });

                    row.RelativeItem().Column(column =>
                    {
                        column.Item().Text("Tashkent").FontColor(Colors.Orange.Accent4);
                    });
                });
                row.Spacing(10);
                column.Item().Row(row =>
                {
                    row.Spacing(10);
                    row.ConstantItem(20).Column(column =>
                    {
                        column.Item().Image("./Pdf/Weather/Images/period.png");
                    });

                    row.RelativeItem().Column(column =>
                    {
                        column.Item().Text($"{Content.From}-{Content.To} {Content.Month}").FontColor(Colors.Orange.Accent4);
                    });
                });
            });
        });
    }
}
