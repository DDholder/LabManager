using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace LabManager.Net
{
   
    public class Client : NetBase
    {
        private TcpClient _tcpClient;

        public Client(IPAddress serverIP, int port)
        {
            ThisIP = Dns.GetHostEntry(Dns.GetHostName()).AddressList
                .FirstOrDefault(a => a.AddressFamily.ToString().Equals("InterNetwork"));
            ThisPort = 5050;

        }
        ~Client()
        {
            _tcpClient?.Close();
        }
        public void ConnectServer(IPAddress serverIP, int port)
        {
            _tcpClient = new TcpClient(serverIP.ToString(), port);
            ThisIP = ((IPEndPoint)_tcpClient.Client.LocalEndPoint).Address;
            ThisPort = ((IPEndPoint)_tcpClient.Client.LocalEndPoint).Port;
            ThisNetworkStream = _tcpClient.GetStream();
            Thread receiveThread = new Thread(Receive)
            {
                IsBackground = true
            };
            receiveThread.Start(_tcpClient);
        }
    }
}
