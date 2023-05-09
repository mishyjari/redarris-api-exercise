namespace RedArrisApi;

public static class DateTimeExtensions
{
    public static string? NormalizeDateString(this string inputString)
    {
        return DateTime.TryParse(inputString, out var date)
            ? date.ToString("yyyy-MM-dd")
            : null;
    }

    public static string GetFirstDayOfYearString()
    {
        return new DateTime(DateTime.Now.Year, 1, 1)
            .ToString("yyyy-MM-dd");
    }
}