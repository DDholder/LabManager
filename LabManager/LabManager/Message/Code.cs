using System.Windows.Forms;

namespace LabManager.Message
{
    /// <summary>
    /// (*待修改)
    /// </summary>
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
