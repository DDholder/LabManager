using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace LabManager.Net
{
    /// <inheritdoc />
    /// <summary>
    /// 客户端类，提供客户端的发送、接受功能
    /// </summary>
    public class Client : NetBase
    {
        private TcpClient _tcpClient;

        public Client()
        {
            ThisIP = Dns.GetHostEntry(Dns.GetHostName()).AddressList
                .FirstOrDefault(a => a.AddressFamily.ToString().Equals("InterNetwork"));
            ThisPort = 5050;
        }
        ~Client()
        {
            _tcpClient?.Close();
            RecordOperateLog("exit");
        }
        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="serverIP">远程服务器的IP地址</param>
        /// <param name="port">远程服务器的端口</param>
        public void ConnectServer(IPAddress serverIP, int port)
        {
            try
            {
                _tcpClient = new TcpClient(serverIP.ToString(), port);
                ThisIP = ((IPEndPoint) _tcpClient.Client.LocalEndPoint).Address;
                ThisPort = ((IPEndPoint) _tcpClient.Client.LocalEndPoint).Port;
                ThisNetworkStream = _tcpClient.GetStream();
                Thread receiveThread = new Thread(Receive)
                {
                    IsBackground = true
                };
                receiveThread.Start(_tcpClient);
                RecordOperateLog("服务器连接成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                RecordErrLog(ex.ToString());
            }
        }
    }
}
