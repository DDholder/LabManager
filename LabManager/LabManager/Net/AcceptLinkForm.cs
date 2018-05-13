using System;
using System.Windows.Forms;

namespace LabManager.Net
{
    /// <inheritdoc />
    /// <summary>
    /// 用于接受客户端连接的窗口
    /// </summary>
    public partial class AcceptLinkForm : Form
    {
        public ClientMember ThisClient;
        public AcceptLinkForm(ClientMember client)
        {
            ThisClient = client;
            InitializeComponent();
        }

        private void Confirm_Click(object sender, EventArgs e)
        {
            ThisClient.Name = textBox1.Text;
            Hide(); 
        }

        private void AcceptLinkForm_Load(object sender, EventArgs e)
        {
            label2.Text = ThisClient.TCPClient.Client.RemoteEndPoint.ToString();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {

        }
    }
}
