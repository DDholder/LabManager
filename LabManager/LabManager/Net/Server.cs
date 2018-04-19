using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using LabManager.Message;
using LabManager.Secret;
namespace LabManager.Net
{
    public partial class Server
    {
        /// <summary>
        /// TCP收到数据事件委托
        /// </summary>
        public delegate void DataFinishedHandle();
        /// <summary>
        /// TCP收到数据事件
        /// </summary>
        public event DataFinishedHandle DataFinished;
        /// <summary>
        /// 主机IP
        /// </summary>
        private readonly IPAddress _myIp = Dns.GetHostEntry(Dns.GetHostName()).AddressList
            .FirstOrDefault(a => a.AddressFamily.ToString().Equals("InterNetwork"));
        /// <summary>
        /// 操作的信息事件委托
        /// </summary>
        /// <param name="message">操作的信息内容</param>
        /// <returns>操作的信息内容</returns>
        public delegate Code ExceptionHandle(Code message);
        public event ExceptionHandle Excepte;

        private string _dataString;
        private int _myport = 6000;
        private static TcpClient _serverTCP;
        private Thread _waitLinkThread;
        private bool _serverOpened;
        private readonly List<ClientMember> _clientSocket = new List<ClientMember>();
        private Permission _loginPermission;
        private readonly TCPMessage _tcpMessage = new TCPMessage();
        private TcpListener tcpListener;
        /// <summary>
        /// 获取服务器IP地址
        /// </summary>
        public IPAddress MyIP => _myIp;
        /// <summary>
        /// 获取或设置服务器端口
        /// </summary>
        public int MyPort { get => _myport; set => _myport = value; }
        /// <summary>
        /// 获取服务器是否打开
        /// </summary>
        public bool ServerOpened => _serverOpened;
        /// <summary>
        /// 获取TCP收到的数据
        /// </summary>
        public string DataString => _dataString;
        private string _exceptionMessage = "null";
        /// <summary>
        /// 获取或设置权限
        /// </summary>
        public Permission LoginPermission { get => _loginPermission; set => _loginPermission = value; }
        /// <summary>
        /// 获取错误信息
        /// </summary>
        public string ExceptionMessage => _exceptionMessage;
        /// <summary>
        /// 获取客户端列表
        /// </summary>
        public List<ClientMember> ClientSocket => _clientSocket;

        public bool BMain { get; } = false;

        private readonly string _secretKey = "5QsLSHkoS75/dipJu877+Q==";
        /// <summary>
        /// 消息枚举
        /// </summary>

        public Server(string accessKey)
        {
            if (Md5Handler.GetMD5(accessKey) == _secretKey)
            {
                _loginPermission = Permission.All;
                tcpListener = new TcpListener(IPAddress.Any, _myport);
            }
            _loginPermission = Permission.Guest;

        }
        /// <summary>
        /// 打开TCP服务器
        /// </summary>
        /// <param name="listenNum">最大监听数量</param>
        /// <returns></returns>
        public Code Open(int listenNum = 10)
        {
            if (_serverOpened)
            {
                return Excepte?.Invoke(_tcpMessage.ServerOpened_OK);
            }
            if (_loginPermission == Permission.Guest)
            {
                return Excepte?.Invoke(_tcpMessage.InsufficientPermission);
            }
            try
            {
                _serverTCP = new TcpClient(new IPEndPoint(_myIp, _myport));
               // tcpListener.Server.Listen(10);
                tcpListener.Start();
                _waitLinkThread = new Thread(WaitLink);
                _waitLinkThread.Start();
                _serverOpened = true;
                
                return Excepte?.Invoke(_tcpMessage.ServerOpened_OK);
            }
            catch (SocketException ex)
            {
                MessageBox.Show(ex.ToString());
                return Excepte?.Invoke(_tcpMessage.ConnectToServer_Failed);
            }
        }

        private void AcceptLink(object client)
        {
            try
            {
                var myClient = client as ClientMember;
                AcceptLinkForm acceptLinkForm = new AcceptLinkForm(myClient);
                acceptLinkForm.ShowDialog();
                myClient = acceptLinkForm.thisClient;
                acceptLinkForm.Close();
                if (_clientSocket.Count == 0) _clientSocket.Add(myClient);
                else
                {
                    for (var i = 0; i < _clientSocket.Count; i++)
                    {
                        if (Equals((myClient.TCPClient.Client.RemoteEndPoint as IPEndPoint)?.Address,
                            (_clientSocket[i].TCPClient.Client.RemoteEndPoint as IPEndPoint)?.Address))
                        {
                            _clientSocket[i].TCPClient.Close();
                            _clientSocket[i] = myClient;
                            break;
                        }
                        if (i == _clientSocket.Count - 1)
                        {
                            _clientSocket.Add(myClient);
                            i++;
                        }
                    }
                }
                Thread receiveThread = new Thread(Receive);
                receiveThread.Start(_clientSocket[_clientSocket.Count - 1].TCPClient);
            }
            catch (SocketException ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void WaitLink()
        {
            while (_serverOpened)
            {
                try
                {                   
                    ClientMember client = new ClientMember(tcpListener.AcceptTcpClient());
                    Thread thread = new Thread(AcceptLink);
                    thread.Start(client);
                }
                catch (SocketException ex)
                {
                    MessageBox.Show(ex.ToString());
                    break;
                }
            }
        }

        private void Receive(object client)
        {
            TcpClient myClient = client as TcpClient;
            while (true)
            {
                try
                {
                    var result = new byte[4096];
                    int receiveNumber = myClient.Client.Receive(result);
                    _dataString = Encoding.Default.GetString(result);
                    if (receiveNumber == 0) return;
                    DataFinished?.Invoke();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    myClient.Close();
                    break;
                }
            }
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="socket">发送目标的套接字</param>
        /// <param name="data">要发送的数据</param>
        public void Send(Socket socket, string data)
        {
            socket.Send(Encoding.Default.GetBytes(data));
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="socket">发送目标的套接字</param>
        /// <param name="data">要发送的数据</param>
        public void Send(Socket socket, byte[] data)
        {
            socket.Send(data);
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="index">发送目标在客户端列表中的索引</param>
        /// <param name="data">要发送的数据</param>
        public void Send(int index, string data)
        {
            _clientSocket[index].Send(Encoding.Default.GetBytes(data));
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="index">发送目标在客户端列表中的索引</param>
        /// <param name="data">要发送的数据</param>
        public void Send(int index, byte[] data)
        {
            _clientSocket[index].Send(data);
        }
    }
}
