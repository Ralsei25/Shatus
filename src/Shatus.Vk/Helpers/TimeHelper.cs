namespace Shatus.Vk.Helpers;

public static class TimeHelper
{
    public static DateTime MoscowNow
        => DateTime.UtcNow.AddHours(3);
}