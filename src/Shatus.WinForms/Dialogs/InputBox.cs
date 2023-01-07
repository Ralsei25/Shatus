namespace ShatusBot.WinForms.Dialogs
{
    public partial class InputBox : Form
    {
        public string Input { get; private set; }
        public InputBox(string message, string caption)
        {
            InitializeComponent();
            messageLabel.Text = message;
            Text = caption;
        }

        private void submitBtn_Click(object sender, EventArgs e)
        {
            Input = inputTextBox.Text;
            DialogResult = DialogResult.OK;
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void submitBtn_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                submitBtn_Click(this, new EventArgs());
            }
        }

        private void InputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }

        public static string ReadInput(string text, string caption)
        {
            var inputBox = new InputBox(text, caption);
            return inputBox.ShowDialog() == DialogResult.OK ? inputBox.Input : string.Empty;
        }
    }
}
