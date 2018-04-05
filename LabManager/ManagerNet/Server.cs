using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Secret;
namespace ManagerNet
{
    public class Server
    {
        public struct Client
        {
            public Socket socket;
            public string name;
        }
        public delegate void DataFinishedHandle();
        public event DataFinishedHandle DataFinished;
        private readonly IPAddress myIP = Dns.GetHostEntry(Dns.GetHostName()).AddressList
            .FirstOrDefault(a => a.AddressFamily.ToString().Equals("InterNetwork"));
        public delegate Message ExceptionHandle(Message message);
        public event ExceptionHandle Excepte;

        private string dataString;
        private int myport = 6000;
        static Socket serverSocket;
        private Thread WaitLinkThread;
        private bool serverOpened;
        private readonly List<Client> clientSocket = new List<Client>();
        private Permission loginPermission;
        /// <summary>
        /// 获取服务器IP地址
        /// </summary>
        public IPAddress MyIP => myIP;
        /// <summary>
        /// 获取或设置服务器端口
        /// </summary>
        public int MyPort { get => myport; set => myport = value; }
        /// <summary>
        /// 获取服务器是否打开
        /// </summary>
        public bool ServerOpened => serverOpened;
        /// <summary>
        /// 获取TCP收到的数据
        /// </summary>
        public string DataString => enCreate ? dataString : "No permission to get";
        private string exceptionMessage = "null";
        /// <summary>
        /// 获取或设置权限
        /// </summary>
        public Permission LoginPermission { get => loginPermission; set => loginPermission = enCreate ? value : Permission.Guest; }
        /// <summary>
        /// 获取错误信息
        /// </summary>
        public string ExceptionMessage => exceptionMessage;
        /// <summary>
        /// 获取或设置数据库连接字符串
        /// </summary>
        public string Constr { get => constr; set => constr = value; }
        /// <summary>
        /// 获取客户端列表
        /// </summary>
        public List<Client> ClientSocket => clientSocket;

        private string constr =
            @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Deng\source\repos\trySQL\trySQL\student.mdf;Integrated Security=True";

        private readonly string secretKey = "5QsLSHkoS75/dipJu877+Q==";
        /// <summary>
        /// 消息枚举
        /// </summary>
        public enum Message
        {
            False = 0,
            OK = 1,
            AlreadyOpened = 2,
            InsufficientPermission = 3
        }

        private readonly bool enCreate;
        /// <summary>
        /// 通过错误代码获取错误信息
        /// </summary>
        /// <param name="message">错误代码</param>
        /// <returns></returns>
        public string GetErrorMessage(Message message)
        {
            switch (message)
            {
                case Message.InsufficientPermission:
                    exceptionMessage = "无权限";
                    break;
                case Message.AlreadyOpened:
                    exceptionMessage = "服务器已打开";
                    break;
                case Message.False:
                    exceptionMessage = "服务器打开失败";
                    break;
                case Message.OK:
                    exceptionMessage = "服务器打开成功";
                    break;
                default:
                    exceptionMessage = "未知信息";
                    break;
            }
            return exceptionMessage;
        }
        public Server(string accessKey)
        {
            if (Md5Handler.getMD5(accessKey) == secretKey)
                enCreate = true;
            else
                MessageBox.Show(@"illegal licence");
            loginPermission = Permission.Guest;
        }
        /// <summary>显示具有指定文本的消息框。</summary>
        /// <param name="listenNum">要在消息框中显示的文本。</param>
        public Message Open(int listenNum = 10)
        {
            if (serverOpened)
            {
                return Excepte(Message.AlreadyOpened);
            }
            if (loginPermission == Permission.Guest)
            {
                return Excepte(Message.InsufficientPermission);
            }
            try
            {
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                serverSocket.Bind(new IPEndPoint(myIP, myport)); //绑定IP地址：端口
                serverSocket.Listen(listenNum); //设定最多10个排队连接请求
                WaitLinkThread = new Thread(WaitLink);
                WaitLinkThread.Start();
                serverOpened = true;

                return Excepte(Message.OK);
            }
            catch (SocketException ex)
            {
                MessageBox.Show(ex.Message);
                return Excepte(Message.False);
            }
        }

        private void AcceptLink(object client)
        {
            try
            {
                Client myClient = (Client)client;
                int i = 1;
                AcceptLinkForm acceptLinkForm = new AcceptLinkForm(myClient);
                acceptLinkForm.ShowDialog();
                myClient = acceptLinkForm.thisClient;
                acceptLinkForm.Close();
                if (clientSocket.Count == 0) clientSocket.Add(myClient);
                else
                    for (i = 0; i < clientSocket.Count; i++)
                    {
                        if (Equals((myClient.socket.RemoteEndPoint as IPEndPoint)?.Address, (clientSocket[i].socket.RemoteEndPoint as IPEndPoint)?.Address))
                        {
                            clientSocket[i].socket.Shutdown(SocketShutdown.Both);
                            clientSocket[i].socket.Close();
                            clientSocket[i] = myClient;
                            i++;
                            break;
                        }
                    }
                Thread receiveThread = new Thread(Receive);
                receiveThread.Start(clientSocket[i - 1].socket);
            }
            catch (SocketException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void WaitLink()
        {
            while (serverOpened)
            {
                try
                {
                    Client client = new Client
                    {
                        socket = serverSocket.Accept()
                    };
                    Thread thread = new Thread(AcceptLink);
                    thread.Start(client);
                }
                catch (SocketException ex)
                {
                    MessageBox.Show(ex.Message);
                    break;
                }
            }
        }

        private void Receive(object socket)
        {
            Socket myClientSocket = (Socket)socket;
            while (true)
            {
                try
                {
                    var result = new byte[4096];
                    int receiveNumber = myClientSocket.Receive(result);
                    dataString = Encoding.Default.GetString(result);
                    if (receiveNumber == 0) return;
                    DataFinished?.Invoke();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    myClientSocket.Shutdown(SocketShutdown.Both);
                    myClientSocket.Close();
                    break;
                }
            }
        }

        public void Send(Socket socket,string data)
        {
            socket.Send(Encoding.Default.GetBytes(data));
        }
    }
}
