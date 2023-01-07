using ShatusBot.WinForms.Dialogs;
using VkNet.Utils.AntiCaptcha;

namespace ShatusBot.WinForms;

public class WinFormCaptchaSolver : ICaptchaSolver
{
    public static SynchronizationContext? SynchronizationContext { get; set; }

    public void CaptchaIsFalse()
    {
        MessageBox.Show("Captcha solving failed");
    }

    public string Solve(string url)
    {
        SynchronizationContext?.Send((s) => Clipboard.SetText(url), null);

        var solve = InputBox.ReadInput($"Solve captcha: {url}", "Solve captcha");
        return solve;
    }
}