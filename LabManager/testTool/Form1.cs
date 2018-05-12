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
using  System.Text;
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

        private void Client_DataFinished()
        {
           // object ob = Communicate.BytesToStruct(client.ReceiveData, new Communicate.tryclass().GetType());
            //Communicate.tryclass t2 = ob as Communicate.tryclass? ?? new Communicate.tryclass();
            void show()
            {
                label4.Text = Encoding.Default.GetString(client.ReceiveData);
            }
            Invoke((Action)show);
        }
        private void Server_DataFinished()
        {
            var bdata = server.ReceiveData;
            //object ob = Communicate.BytesToStruct(bdata, new Communicate.tryclass().GetType());
            //Communicate.tryclass t2 = ob as Communicate.tryclass? ?? new Communicate.tryclass();
            void show()
            {
                label5.Text = Encoding.Default.GetString(bdata);
            }
            Invoke((Action)show);
            //throw new NotImplementedException();
        }

        private Server server = new Server("109109109");

        private Client client = new Client(IPAddress.Parse("10.1.51.63"), 6000);
        private void Button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == @"djh") server.LoginPermission = Permission.All;
            server.Open();
        }
        private void Button2_Click(object sender, EventArgs e)
        {
            //client.ConnectServer();
            client.ConnectServer(IPAddress.Parse(textBox2.Text), 6000);
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            //client.SendData("ok");
            var sdata = Encoding.Default.GetBytes("Hello,Server!");  
            client.Write(sdata, sdata.Length);
            //client.TrySendClass();
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
            server.Write(listBox1.SelectedIndex,data,data.Length);
            //server.TrySendClass();
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            foreach (var eachClientSocket in server.ClientSocket)
            {
                listBox1.Items.Add(eachClientSocket.TCPClient.Client.RemoteEndPoint + "   " + eachClientSocket.name);
            }
        }
        public DataSet data = new DataSet();
        CMDData cmddata;
        private void Button7_Click(object sender, EventArgs e)
        {
            //dataBaseManager.SetValue(0, 1, 1234);
            dataBaseManager.UpdateData();
            //label2.Text = cmddata.DataStr;
        }

        private void button8_Click(object sender, EventArgs e)
        {
           // tryname(func);
            //tryinfo();
            string s = "尝试";
            var b = Encoding.Default.GetBytes(s);
            label7.Text=s+Encoding.Default.GetString(b);
        }
        delegate void fund(int i);
        void tryname(fund fun)
        {
            label7.Text = "ok";
            fun(1);
            label8.Text = fun.Method.Name;
            System.Reflection.ParameterInfo[] infos=fun.Method.GetParameters();
            
        }

        void func(int i)
        {
            label7.Text += "  ok";
        }

        void tryinfo()
        {
            StackTrace ss = new StackTrace(true);  
            Type t = ss.GetFrame(1).GetMethod().ReflectedType;  
            String sName = ss.GetFrame(1).GetMethod().Name;  
            MessageBox.Show(DateTime.Now+  "typeName:"+t.FullName + " sName: " + sName);  
        }
    }
}
