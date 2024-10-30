namespace WeatherPdf.Utils;

public static class DateHelper
{
    public static (DateTime startDate, DateTime endDate) GetPreviousMonthDateRange()
    {
        var previousMonth = DateTime.UtcNow.AddMonths(-1);
        var startDateTime = DateTime.SpecifyKind(new DateTime(previousMonth.Year, previousMonth.Month, 1), DateTimeKind.Utc);
        var endDateTime = DateTime.SpecifyKind(new DateTime(previousMonth.Year, previousMonth.Month, DateTime.DaysInMonth(previousMonth.Year, previousMonth.Month)), DateTimeKind.Utc);
        return (startDateTime, endDateTime);
    }
}
