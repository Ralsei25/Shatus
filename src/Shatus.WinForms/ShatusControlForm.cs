using Shatus.Vk;
using Shatus.WinForms.Configs;
using Shatus.YouTube;

namespace ShatusBot.WinForms;

public partial class ShatusControlForm : Form
{
    private readonly ShatusVkPublisher _vkPublisher;
    private readonly IVideoDownloader _videoDownloader;
    private readonly IWritableOptions<WinAppConfigs> _configs;

    public ShatusControlForm(ShatusVkPublisher vkPublisher, IVideoDownloader videoDownloader, IWritableOptions<WinAppConfigs> configs)
    {
        InitializeComponent();
        _vkPublisher = vkPublisher;
        _videoDownloader = videoDownloader;
        _configs = configs;

        FillTextBoxesFromConfigs();
    }
    private void FillTextBoxesFromConfigs()
    {
        if (!string.IsNullOrEmpty(_configs.Value.VideoTitle1))
            videoTitle1.Text = _configs.Value.VideoTitle1;
        if (!string.IsNullOrEmpty(_configs.Value.VideoTitle2))
            videoTitle2.Text = _configs.Value.VideoTitle2;
        if (!string.IsNullOrEmpty(_configs.Value.VideoTitle3))
            videoTitle3.Text = _configs.Value.VideoTitle3;
        if (!string.IsNullOrEmpty(_configs.Value.VideoTitle4))
            videoTitle4.Text = _configs.Value.VideoTitle4;
        if (!string.IsNullOrEmpty(_configs.Value.VideoTitle5))
            videoTitle5.Text = _configs.Value.VideoTitle5;
    }
    private async Task SubmitVideo(TextBox titleTB, TextBox linkTB, Button btn)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(titleTB.Text) && !string.IsNullOrWhiteSpace(linkTB.Text))
            {
                titleTB.Enabled = false;
                linkTB.Enabled = false;
                btn.Enabled = false;

                await _videoDownloader.DownloadVideoAsync(linkTB.Text, _configs.Value.VideoFolderPath, titleTB.Text.ToLower() + ".mp4");
                await _vkPublisher.ScheduleVideo(titleTB.Text.ToLower().Trim(), Path.Combine(_configs.Value.VideoFolderPath, titleTB.Text + ".mp4"));

                linkTB.Clear();
            }
        }
        finally
        {

            titleTB.Enabled = true;
            linkTB.Enabled = true;
            btn.Enabled = true;
        }
    }

    private async Task GeneralSubmit(Button btn)
    {
        try
        {
            switch (btn.Name)
            {
                case "submit1":
                    await SubmitVideo(videoTitle1, videoLink1, submit1);
                    await _configs.UpdateAsync(c => c.VideoTitle1 = videoTitle1.Text);
                    break;
                case "submit2":
                    await SubmitVideo(videoTitle2, videoLink2, submit2);
                    await _configs.UpdateAsync(c => c.VideoTitle2 = videoTitle2.Text);
                    break;
                case "submit3":
                    await SubmitVideo(videoTitle3, videoLink3, submit3);
                    await _configs.UpdateAsync(c => c.VideoTitle3 = videoTitle3.Text);
                    break;
                case "submit4":
                    await SubmitVideo(videoTitle4, videoLink4, submit4);
                    await _configs.UpdateAsync(c => c.VideoTitle4 = videoTitle4.Text);
                    break;
                case "submit5":
                    await SubmitVideo(videoTitle5, videoLink5, submit5);
                    await _configs.UpdateAsync(c => c.VideoTitle5 = videoTitle5.Text);
                    break;
                default:
                    break;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
    private async void submit_Click(object sender, EventArgs e)
    {
        await GeneralSubmit((Button)sender);
    }

    private async void video1_KeyPress(object sender, KeyPressEventArgs e)
    {
        if (e.KeyChar == (char)Keys.Enter)
            await GeneralSubmit(submit1);
    }

    private async void video2_KeyPress(object sender, KeyPressEventArgs e)
    {
        if (e.KeyChar == (char)Keys.Enter)
            await GeneralSubmit(submit2);
    }

    private async void video3_KeyPress(object sender, KeyPressEventArgs e)
    {
        if (e.KeyChar == (char)Keys.Enter)
            await GeneralSubmit(submit3);
    }

    private async void video4_KeyPress(object sender, KeyPressEventArgs e)
    {
        if (e.KeyChar == (char)Keys.Enter)
            await GeneralSubmit(submit4);
    }

    private async void video5_KeyPress(object sender, KeyPressEventArgs e)
    {
        if (e.KeyChar == (char)Keys.Enter)
            await GeneralSubmit(submit5);
    }
}