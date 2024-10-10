namespace WeatherPdf.Utils;

public static class DateHelper
{
    public static (DateTime startDate, DateTime endDate) GetPreviousMonthDateRange()
    {
        var currentYear = DateTime.Now.Year;
        var month = DateTime.Now.Month - 1;
        if (month == 0)
        {
            currentYear--;
            month = 12;
        }

        var end = DateTime.DaysInMonth(currentYear, month);
        var startDateTime = new DateTime(currentYear, month, 1);
        var endDateTime = new DateTime(currentYear, month, end);
        return (startDateTime, endDateTime);
    }
}
