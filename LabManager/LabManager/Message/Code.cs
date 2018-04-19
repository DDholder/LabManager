using System.Windows.Forms;

namespace LabManager.Message
{
    public class Code
    {
        public string Message { get; }

        public Code(string showedText)
        {
            Message = showedText;
        }

        void Show()
        {
            MessageBox.Show(Message);
        }
    }
}
