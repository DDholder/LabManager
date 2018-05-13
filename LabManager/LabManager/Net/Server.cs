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
    /// <inheritdoc />
    /// <summary>
    /// 服务端类，提供服务端的发送、监听、接受功能
    /// </summary>
    public class Server : NetBase
    {

        /// <summary>
        /// 操作的信息事件委托
        /// </summary>
        /// <param name="message">操作的信息内容</param>
        /// <returns>操作的信息内容</returns>
        public delegate Code ExceptionHandle(Code message);
        /// <summary>
        /// 操作的信息事件
        /// </summary>
        public event ExceptionHandle Excepte;

        //private static TcpClient _serverTCP;
        private Thread _waitLinkThread;
        private bool _serverOpened;
        private readonly List<ClientMember> _clientSocket = new List<ClientMember>();
        private Permission _loginPermission;
        private readonly TCPMessage _tcpMessage = new TCPMessage();
        private readonly TcpListener _tcpListener;
        private readonly string _exceptionMessage = "null";
        private readonly string _secretKey = "5QsLSHkoS75/dipJu877+Q==";
        /// <summary>
        /// 客户端列表索引的字典
        /// </summary>
        public static Dictionary<EndPoint, ClientMember> Dict = new Dictionary<EndPoint, ClientMember>();
        /// <summary>
        /// 获取服务器IP地址
        /// </summary>
        public IPAddress IP => ThisIP;
        /// <summary>
        /// 获取或设置服务器端口
        /// </summary>
        public int Port { get => ThisPort; set => ThisPort = value; }
        /// <summary>
        /// 获取服务器是否打开
        /// </summary>
        public bool ServerOpened => _serverOpened; 
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
        /// <summary>
        /// (*待更改)
        /// </summary>
        public bool BMain { get; } = false;
        /// <summary>
        /// 服务器初始方法
        /// </summary>
        /// <param name="accessKey">登陆的MD5密钥</param>
        public Server(string accessKey)
        {
            try
            {
                ThisIP = Dns.GetHostEntry(Dns.GetHostName()).AddressList
                    .FirstOrDefault(a => a.AddressFamily.ToString().Equals("InterNetwork"));
                ThisPort = 6000;
                if (Md5Handler.GetMD5(accessKey) == _secretKey)
                {
                    _loginPermission = Permission.All;
                    _tcpListener = new TcpListener(IPAddress.Any, Port);
                }

                _loginPermission = Permission.Guest;
                DisConnected += Server_DisConnected;
                RecordOperateLog("服务器启动，地址：" + ThisIP+":"+ThisPort);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                RecordErrLog(ex.ToString());
            }
        }
        ~Server()
        {
            RecordOperateLog("exit");
        }
        /// <summary>
        /// 客户端离线的处理
        /// </summary>
        /// <param name="endPoint">离线客户端的远端地址</param>
        private void Server_DisConnected(EndPoint endPoint) 
        {
            try
            {
                ClientMember clientMember = Dict[endPoint]; 
                string name = clientMember.Name;
                _clientSocket.Remove(Dict[clientMember.Address]);
                clientMember.TCPClient.Close();
                RecordOperateLog(endPoint + "(" + name + ")" + "下线");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                RecordErrLog(ex.ToString());
            }
            //throw new NotImplementedException();
        }

        /// <summary>
        /// 打开TCP服务器
        /// </summary>
        /// <param name="listenNum">最大监听数量</param>
        /// <returns></returns>
        public void Open(int listenNum = 10)
        {
            if (_serverOpened)
            {
                Excepte?.Invoke(_tcpMessage.ServerOpened_OK);
                RecordOperateLog("打开成功，Permission="+_loginPermission);
                return;
            }
            if (_loginPermission == Permission.Guest)
            {
                Excepte?.Invoke(_tcpMessage.InsufficientPermission);
                RecordOperateLog("打开失败，Permission="+_loginPermission);
            }
            try
            {
                _tcpListener.Start();
                _waitLinkThread = new Thread(WaitLink)
                {
                    IsBackground = true
                };
                _waitLinkThread.Start();
                _serverOpened = true;

                Excepte?.Invoke(_tcpMessage.ServerOpened_OK);
                RecordOperateLog("打开成功，Permission="+_loginPermission);
            }
            catch (SocketException ex)
            {
                MessageBox.Show(ex.Message);
                Excepte?.Invoke(_tcpMessage.ConnectToServer_Failed);
                RecordErrLog(ex.ToString());
            }
        }
        /// <summary>
        /// 接受客户端的连接
        /// </summary>
        /// <param name="client">待连接的客户端</param>
        private void AcceptLink(object client)
        {
            try
            {
                var myClient = client as ClientMember;
                AcceptLinkForm acceptLinkForm = new AcceptLinkForm(myClient);
                acceptLinkForm.ShowDialog();
                myClient = acceptLinkForm.ThisClient;
                acceptLinkForm.Close();
                Dict.Add(myClient.Address, myClient);
                if (_clientSocket.Count == 0) _clientSocket.Add(myClient);
                else
                {
                    for (var i = 0; i < _clientSocket.Count; i++)
                    {
                        if (Equals((myClient.Address as IPEndPoint)?.Address,
                            (_clientSocket[i].Address as IPEndPoint)?.Address))
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
                Thread receiveThread = new Thread(Receive)
                {
                    IsBackground = true
                };
                receiveThread.Start(_clientSocket[_clientSocket.Count - 1].TCPClient);
                RecordOperateLog(_clientSocket[_clientSocket.Count - 1].Address+"("+_clientSocket[_clientSocket.Count - 1].Name+")"+"上线");
            }
            catch (SocketException ex)
            {
                MessageBox.Show(ex.ToString());
                RecordErrLog(ex.ToString());
            }
        }
        /// <summary>
        /// 监听客户端连接
        /// </summary>
        private void WaitLink()
        {
            while (_serverOpened)
            {
                try
                {
                    ClientMember client = new ClientMember(_tcpListener.AcceptTcpClient());
                    Thread thread = new Thread(AcceptLink)
                    {
                        IsBackground = true
                    };
                    thread.Start(client);
                }
                catch (SocketException ex)
                {
                    MessageBox.Show(ex.ToString());
                    RecordErrLog(ex.ToString());
                    break;
                }
            }
        }
        /// <summary>
        /// 向指定的客户端发送数据
        /// </summary>
        /// <param name="index">客户端在List中的索引</param>
        /// <param name="data">要发送的数据</param>
        /// <param name="size">要发送的长度</param>
        /// <param name="offset">从data的那个位置开始发送</param>
        public void Write(int index, byte[] data, int size, int offset = 0)
        {
            try
            {
                ThisNetworkStream = _clientSocket[index].NetworkStream;
                Write(data, size, offset);
                RecordOperateLog("发送数据:"+Encoding.Default.GetString(data));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                RecordErrLog(ex.ToString());
            }
        }
    }
}
