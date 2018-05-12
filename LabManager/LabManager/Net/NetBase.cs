using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using LabManager.Message;
using LabManager.Secret;
namespace LabManager.Net
{
    public class NetBase : SystemBase
    {
        /// <summary>
        /// TCP收到数据事件委托
        /// </summary>
        public delegate void DataFinishedHandle();
        /// <summary>
        /// TCP收到数据事件
        /// </summary>
        public event DataFinishedHandle DataFinished;

        public delegate void DisConnectHandle(EndPoint endPoint);
        public event DisConnectHandle DisConnected;
        protected IPAddress ThisIP = Dns.GetHostEntry(Dns.GetHostName()).AddressList
            .FirstOrDefault(a => a.AddressFamily.ToString().Equals("InterNetwork"));
        protected int ThisPort = 6000;

        protected NetBase()
        {
        }

        private byte[] _receiveData = null;
        public byte[] ReceiveData => _receiveData;
        protected NetworkStream ThisNetworkStream { get; set; }

        public void Write(byte[] data, int size, int offset = 0)
        {
            ThisNetworkStream.Write(data, offset, size);
        }
        
        public void TrySendClass()
        {
            Communicate.tryclass t = new Communicate.tryclass { num = 132456, id = 222 };
            byte[] data =Communicate.StructToBytes(t);
            ThisNetworkStream.Write(data, 0, data.Length);
        }
        protected void Receive(object client)
        {
            var myClient = client as TcpClient;
            while (true)
            {
                try
                {
                    var result = new byte[4096];
                    int receiveNumber = myClient.Client.Receive(result);
                    _receiveData = result;
                    if (receiveNumber == 0)
                    {
                        DisConnected?.Invoke(myClient.Client.RemoteEndPoint);
                        return;
                    }
                    DataFinished?.Invoke();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    DisConnected?.Invoke(myClient?.Client.RemoteEndPoint);
                    RecordErrLog(ex.ToString());
                    break;
                }
            }
        }

    }
}
