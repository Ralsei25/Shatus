using Shatus.Vk;
using Shatus.YouTube;

namespace ShatusBot.WinForms;

public partial class ShatusControlForm : Form
{
    private readonly string _videoFolderPath;
    private readonly ShatusVkPublisher _vkPublisher;
    private readonly IVideoDownloader _videoDownloader;

    public ShatusControlForm(ShatusVkPublisher vkPublisher, IVideoDownloader videoDownloader)
    {
        InitializeComponent();
        _vkPublisher = vkPublisher;
        _videoDownloader = videoDownloader;
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

                await _videoDownloader.DownloadVideoAsync(linkTB.Text, _videoFolderPath, titleTB.Text.ToLower() + ".mp4");
                await _vkPublisher.ScheduleVideo(titleTB.Text.ToLower().Trim(), _videoFolderPath + titleTB.Text + ".mp4");

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

    private async void submit1_Click(object sender, EventArgs e)
    {
        try
        {
            await SubmitVideo(videoTitle1, videoLink1, submit1);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private async void submit2_Click(object sender, EventArgs e)
    {

        try
        {
            await SubmitVideo(videoTitle2, videoLink2, submit2);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private async void submit3_Click(object sender, EventArgs e)
    {

        try
        {
            await SubmitVideo(videoTitle3, videoLink3, submit3);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private async void submit4_Click(object sender, EventArgs e)
    {

        try
        {
            await SubmitVideo(videoTitle4, videoLink4, submit4);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private async void submit5_Click(object sender, EventArgs e)
    {

        try
        {
            await SubmitVideo(videoTitle5, videoLink5, submit5);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
}