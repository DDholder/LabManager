using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using LabManager.Message;
using LabManager.Secret;
using LabManager.Net;
using LabManager.DataBase;
using LabManager;
using System.Text;
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

        private DataBaseManager dataBaseManager = new DataBaseManager(constr, "Table");
        private static string constr =
            @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Deng\source\repos\trySQL\trySQL\student.mdf;Integrated Security=True";
        private Code Server_Excepte(Code message)
        {
            label2.Text = message.Message;
            return message;
            //throw new NotImplementedException();
        }

        private void Client_DataFinished(EndPoint endPoint)
        {
            object ob=Communicate.Bytes2Object(client.ReceiveData);
            Type type = ob.GetType();
            if (type == typeof(Communicate.DataBaseCmd))
            {
                Communicate.DataBaseCmd ddd = (Communicate.DataBaseCmd) ob;
            }
        }

        private void Server_DataFinished(EndPoint endPoint)
        {
            void show()
            {
                label5.Text = @"got it";
            }
            Invoke((Action)show);
            //throw new NotImplementedException();

            object ob=Communicate.Bytes2Object(server.ReceiveData);
            Type type = ob.GetType();
            if (type == typeof(Communicate.DataBaseCmd))
            {
                Communicate.DataBaseCmd ddd = (Communicate.DataBaseCmd) ob;
                object value = dataBaseManager.DataSet.Tables[0].Rows[ddd.Row][ddd.Columns];
                var bbbyte = Communicate.Object2Bytes(value);
                server.Write(server.Dict[endPoint], bbbyte, bbbyte.Length);
                //server.Write()
            }
        }

        private Server server = new Server("109109109");

        private Client client = new Client();
        private void Button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == @"djh") server.LoginPermission = Permission.All;
            server.Open();
        }
        private void Button2_Click(object sender, EventArgs e)
        {
            client.ConnectServer(IPAddress.Parse(textBox2.Text), 6000);
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            var sdata = Encoding.Default.GetBytes("Hello,Server!");
            Communicate.DataBaseCmd dataBaseCmd = new Communicate.DataBaseCmd(Communicate.CmdType.Read, 1, 1);
            var bData = Communicate.Object2Bytes(dataBaseCmd);
            client.Write(bData, bData.Length);
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
            //server.SendCMD(listBox1.SelectedIndex,cmddata);
            var data = Encoding.Default.GetBytes("Hello,Client!");
            //server.Write(listBox1.SelectedIndex,data,data.Length);
            //server.TrySendClass();
            Communicate.DataBaseCmd dataBaseCmd = new Communicate.DataBaseCmd(Communicate.CmdType.Read, 1, 1);
            var bData = Communicate.Object2Bytes(dataBaseCmd);
            server.Write(listBox1.SelectedIndex, bData, bData.Length);
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            foreach (var eachClientSocket in server.ClientSocket)
            {
                listBox1.Items.Add(eachClientSocket.TCPClient.Client.RemoteEndPoint + "   " + eachClientSocket.Name);
            }
        }
        public DataSet data = new DataSet();
        private void Button7_Click(object sender, EventArgs e)
        {
            //dataBaseManager.SetValue(0, 1, 1234);
            DataRow dataRow = dataBaseManager.DataSet.Tables[0].NewRow();
            dataRow[0] = 3957;
            dataRow[1] = 551;
            dataBaseManager += dataRow;
            dataBaseManager.UpdateData();
            //label2.Text = cmddata.DataStr;
        }
        byte[] bbb;
       
        private void button8_Click(object sender, EventArgs e)
        {
            byte[] buffer = {0, 1};
            label1.Text = buffer[0].ToString();
            label7.Text = buffer[1].ToString();
            tryop(buffer);
            label2.Text = buffer[0].ToString();
            label8.Text = buffer[1].ToString();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dataBaseManager.SetDataSet();
        }

        void tryop(byte[] buffer)
        {
            buffer[0]++;
            buffer[1]++;
        }
    }
}
