using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace LabManager.Net
{
    /// <inheritdoc />
    /// <summary>
    /// 网络系统的基类
    /// </summary>
    public class NetBase : SystemBase
    {
        /// <summary>
        /// TCP收到数据事件委托
        /// </summary>
        /// <param name="endPoint">受到数据的目标远程地址</param>
        public delegate void DataFinishedHandle(EndPoint endPoint);
        /// <summary>
        /// TCP收到数据事件
        /// </summary>
        public event DataFinishedHandle DataFinished;
        /// <summary>
        /// 连接断开的事件委托
        /// </summary>
        /// <param name="endPoint"></param>
        public delegate void DisConnectHandle(EndPoint endPoint);
        /// <summary>
        /// 连接断开的事件
        /// </summary>
        public event DisConnectHandle DisConnected;

        /// <summary>
        /// 收到的数据
        /// </summary>
        public byte[] ReceiveData { get; private set; }
        /// <summary>
        /// 收到数据的长度
        /// </summary>
        public int? ReceiveLength { get; private set; }

        /// <summary>
        /// 此组建的网络数据流
        /// </summary>
        protected NetworkStream ThisNetworkStream { get; set; }
        /// <summary>
        /// 向数据流中写入数据
        /// </summary>
        /// <param name="data">要写入的数据</param>
        /// <param name="size">要写入的数量</param>
        /// <param name="offset">从data的指定位置开始写入</param>
        public void Write(byte[] data, int size, int offset = 0)
        {
            ThisNetworkStream.Write(data, offset, size);
            ThisNetworkStream.Flush();
        }
        /// <summary>
        /// 接收数据，如收到数据则触发DataFinished事件，断开则触发DisConnected事件
        /// </summary>
        /// <param name="client">要接受数据的TcpClient</param>
        protected void Receive(object client)
        {
            var myClient = client as TcpClient;
            while (true)
            {
                try
                {
                    var result = new byte[4096];
                    ReceiveLength = myClient?.Client.Receive(result);
                    ReceiveData = new byte[ReceiveLength ?? 0];
                    Array.Copy(result, ReceiveData, ReceiveLength ?? 0);
                    if (ReceiveLength == 0)
                    {
                        DisConnected?.Invoke(myClient?.Client.RemoteEndPoint);
                        return;
                    }
                    DataFinished?.Invoke(myClient?.Client.RemoteEndPoint);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    DisConnected?.Invoke(myClient?.Client.RemoteEndPoint);
                    RecordErrLog(ex.ToString());
                    break;
                }
            }
        }
        /// <summary>
        /// 此组件的IP地址
        /// </summary>
        protected IPAddress ThisIP = Dns.GetHostEntry(Dns.GetHostName()).AddressList
            .FirstOrDefault(a => a.AddressFamily.ToString().Equals("InterNetwork"));
        /// <summary>
        /// 此组件的端口
        /// </summary>
        protected int ThisPort = 6000;
        protected NetBase()
        {
        }
    }
}
