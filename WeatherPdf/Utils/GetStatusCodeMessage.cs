namespace WeatherPdf.Utils;

public static class GetStatusCode
{
    public static string Message(int status) => status switch
    {
        401 => "Wrong api key",
        404 => "Wrong specified city name",
        429 => "Too many requests. Try again later",
        _ => "Something wrong happened. Try again later"
    };
}
