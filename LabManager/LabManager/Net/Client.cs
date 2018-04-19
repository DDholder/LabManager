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
    public class Client
    {
        public delegate void DataFinishedHandle();
        public event DataFinishedHandle DataFinished;
        Socket thisSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private bool isConnected;
        private int serverPort = 6000;
        public string DataString { get; private set; }

        public IPAddress ServerIp { get; set; }

        public int ServerPort { get=>serverPort; set=>serverPort=value; }
        public bool IsConnected { get=>isConnected; private set=>isConnected=value; }

        public Socket ThisSocket
        {
            get
            {
                if (thisSocket == null)
                {
                    thisSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                }

                if (thisSocket.Connected)
                    return thisSocket;
                thisSocket.Close();
                thisSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                return thisSocket;
            }
        }

        public Client(IPAddress serverAddress, int port)
        {
            ServerIp = serverAddress;
            ServerPort = port;
            isConnected = false;
        }

        public void ConnectServer(IPAddress serverAddress, int port)
        {
            ServerIp = serverAddress;
            ServerPort = port;
            Thread connectThread = new Thread(ConnectServerAsync);
            connectThread.Start();
        }

        public void ConnectServer()
        {
            Thread connectThread = new Thread(ConnectServerAsync);
            connectThread.Start();
        }

        private void ConnectServerAsync()
        {
            try
            {
                ThisSocket.Connect(ServerIp, ServerPort);
                Thread receiveThread = new Thread(Receive);
                receiveThread.Start();
                isConnected = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void SendData(string data)
        {
            try
            {
                ThisSocket.Send(Encoding.Default.GetBytes(data));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private void Receive()
        {
            while (true)
            {
                try
                {
                    var result = new byte[4096];
                    int receiveNumber = ThisSocket.Receive(result);
                    DataString = Encoding.Default.GetString(result);
                    if (receiveNumber == 0)
                    {
                        isConnected = false;
                        thisSocket.Close();
                        return;
                    }
                    DataFinished?.Invoke();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    ThisSocket.Shutdown(SocketShutdown.Both);
                    ThisSocket.Close();
                    break;
                }
            }
        }
    }
}
