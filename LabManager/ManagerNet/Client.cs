using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Secret;
namespace ManagerNet
{
   public class Client
    {
        public delegate void DataFinishedHandle();
        public event DataFinishedHandle DataFinished;
        public IPAddress serverIP;
        public int serverPort=6000;
        private string dataString;
        readonly Socket socket= new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        public string DataString => dataString;

        public Client(IPAddress serverAddress,int port)
        {
            serverIP = serverAddress;
            serverPort = port;
        }
        public Client()
        {

        }

        public void ConnectServer()
        {
            socket.Connect(serverIP, serverPort);
            Thread receiveThread = new Thread(Receive);
            receiveThread.Start(socket);
        }

        public void SendData(string data)
        {
            socket.Send(Encoding.Default.GetBytes(data));
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
    }
}
