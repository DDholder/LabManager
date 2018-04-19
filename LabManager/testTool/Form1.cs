using System;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Windows.Forms;
using LabManager.Message;
using LabManager.Secret;
using LabManager.Net;
using LabManager.DataBase;
namespace testTool//ssss
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
        private DataBaseManager dataBaseManager = new DataBaseManager(constr,"Table");
        private static string constr =
            @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Deng\source\repos\trySQL\trySQL\student.mdf;Integrated Security=True";
        private Code Server_Excepte(Code message)
        {
            label2.Text = message.Message;
            return message;
            //throw new NotImplementedException();
        }

        private void Client_DataFinished()
        {
            void show()
            {
                label4.Text = client.DataString;
            }
            Invoke((Action)show);
        }
        private void Server_DataFinished()
        {
            void show()
            {
                label5.Text = server.DataString;
            }
            Invoke((Action) show);
            //throw new NotImplementedException();
        }

        private Server server = new Server("109109109");

        private Client client = new Client(IPAddress.Parse("10.5.76.7"), 6000);
        private void Button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == @"djh") server.LoginPermission = Permission.All;
            server.Open();
        }
        
        private void Button2_Click(object sender, EventArgs e)
        {
            client.ConnectServer();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            client.SendData("ok");
        }
        
        private void Button4_Click(object sender, EventArgs e)
        {
            
            //string strConnection = constr;
            //try
            //{
            //    SqlDataAdapter adapter = new SqlDataAdapter("select * from Table", strConnection);
            //    SqlConnection conn = new SqlConnection(strConnection);
            //    SqlCommand SCD=new SqlCommand("select Id, name from [Table] where name='fjj'", conn);
            //    adapter.SelectCommand = SCD;
            //    conn.Open();
            //    SqlDataReader reder = SCD.ExecuteReader();
            //    if (reder.Read())
            //    {
            //        label1.Text = reder["Id"].ToString();
            //    }
            //    else
            //        MessageBox.Show(@"no this message");
            //    conn.Close();
            //}
            //catch (SqlException ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}


            dataBaseManager.SetDataSet();

            dataGridView1.DataSource = dataBaseManager.DataSet.Tables[0];
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            //server.Send(listBox1.SelectedIndex, "ok");
            //server.Send(listBox1.SelectedIndex,);
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            foreach (var eachClientSocket in server.ClientSocket)
            {
                listBox1.Items.Add(eachClientSocket.TCPClient.Client.RemoteEndPoint+"   "+ eachClientSocket.name);
            }
        }
       public DataSet data = new DataSet();
        private void Button7_Click(object sender, EventArgs e)
        {
            dataBaseManager.SetValue(0, 1, 1234);
            dataBaseManager.UpdateData();
        }
    }
}
