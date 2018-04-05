using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ManagerNet
{
    public partial class AcceptLinkForm : Form
    {
        public AcceptLinkForm()
        {
            InitializeComponent();
        }
        public Server.Client thisClient;
        public AcceptLinkForm(Server.Client client)
        {
            thisClient = client;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            thisClient.name = textBox1.Text;
            Hide();
        }
    }
}
