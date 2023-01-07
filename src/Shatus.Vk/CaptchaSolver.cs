using VkNet.Utils.AntiCaptcha;

namespace Shatus.Vk;

public class ConsoleCaptchaSolver : ICaptchaSolver
{
    public void CaptchaIsFalse()
    {
        throw new NotImplementedException();
    }

    public string Solve(string url)
    {
        Console.WriteLine(url);
        return Console.ReadLine() ?? "";
    }
}