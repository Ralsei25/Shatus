namespace Shatus.Vk.Extensions;

public static class DateTimeExtensions
{
    public static DateTime TrimSeconds(this DateTime dateTime)
        => dateTime.AddSeconds(-dateTime.Second);
    public static DateTime UtcToMoscow(this DateTime dateTime)
        => dateTime.AddHours(3);
    public static DateTime MoscowToUtc(this DateTime dateTime)
        => dateTime.AddHours(-3);


    public static bool IsNightTime(this DateTime dateTime)
        => !IsDayTime(dateTime);
    public static bool IsDayTime(this DateTime dateTime)
    {
        var start = new TimeSpan(10, 0, 0);
        var end = new TimeSpan(21, 0, 0);
        var current = dateTime.TimeOfDay;

        if (start < current && current < end)
            return true;
        return false;
    }
}
