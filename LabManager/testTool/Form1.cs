using System;
using System.Data.SqlClient;
using System.Net;
using System.Windows.Forms;
using ManagerNet;
using Secret;
namespace testTool//sss
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            server.DataFinished += Server_DataFinished;
            server.Excepte += Server_Excepte;
            client.DataFinished += Client_DataFinished;
            InitializeComponent();
        }

        private string constr =
            @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Deng\source\repos\trySQL\trySQL\student.mdf;Integrated Security=True";
        private Server.Message Server_Excepte(Server.Message message)
        {
            label2.Text = server.GetErrorMessage(message);
            return message;
            //throw new NotImplementedException();
        }

        private void Client_DataFinished()
        {
            void show()
            {
                label1.Text = client.DataString;
            }
            Invoke((Action)show);
        }
        private void Server_DataFinished()
        {
            void show()
            {
                label1.Text = server.DataString;
            }
            Invoke((Action) show);
            //throw new NotImplementedException();
        }

        private Server server = new Server("109109109");

        private Client client = new Client(IPAddress.Parse("10.1.51.69"), 6000);
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == @"djh") server.LoginPermission = Permission.All;
            server.Open();
        }
        
        private void button2_Click(object sender, EventArgs e)
        {
            client.ConnectServer();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            client.SendData("ok");
        }
        private void button4_Click(object sender, EventArgs e)
        {
            
            string strConnection = constr;
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter("select * from Id", strConnection);
                SqlConnection conn = new SqlConnection(strConnection);
                SqlCommand SCD=new SqlCommand("select Id, name from [Table] where name='fjj'", conn);
                adapter.SelectCommand = SCD;
                conn.Open();
                SqlDataReader reder = SCD.ExecuteReader();
                if (reder.Read())
                {
                    label1.Text = reder["Id"].ToString();
                }
                else
                    MessageBox.Show(@"no this message");
                conn.Close();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //label1.Text = Md5Handler.getMD5("109109109");
           
        }

        private void button6_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            for (int i = 0; i < server.ClientSocket.Count; i++)
            {
                listBox1.Items.Add(server.ClientSocket[i].socket.RemoteEndPoint+"   "+ server.ClientSocket[i].name);
            }
        }
    }
}
