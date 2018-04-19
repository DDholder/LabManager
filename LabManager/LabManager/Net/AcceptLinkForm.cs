using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LabManager.Net
{
    public partial class AcceptLinkForm : Form
    {
        public AcceptLinkForm()
        {
            InitializeComponent();
        }
        public LabManager.Net.Server.ClientMember thisClient;
        public AcceptLinkForm(LabManager.Net.Server.ClientMember client)
        {
            thisClient = client;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            thisClient.name = textBox1.Text;
            Hide(); 
        }

        private void AcceptLinkForm_Load(object sender, EventArgs e)
        {
            label2.Text = thisClient.TCPClient.Client.RemoteEndPoint.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
