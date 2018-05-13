using System.Net;
using System.Net.Sockets;

namespace LabManager.Net
{
    /// <summary>
    /// 用于为服务器提供与客户成员连接的TcpClient
    /// </summary>
    public class ClientMember
    {
        private readonly TcpClient _tcpclient;
        private string _name;
        private readonly NetworkStream _networkStream;

        /// <summary>
        /// 成员的TcpClient
        /// </summary>
        public TcpClient TCPClient => _tcpclient;
        /// <summary>
        /// 用于传输数据的数据流
        /// </summary>
        public NetworkStream NetworkStream => _networkStream;
        /// <summary>
        /// 成员的远端服务地址，即与服务器相连的客户端的地址与端口
        /// </summary>
        public EndPoint Address => TCPClient.Client.RemoteEndPoint;
        /// <summary>
        /// 客户端的命名
        /// </summary>
        public string Name { get => _name; set => _name = value; }
        public ClientMember(TcpClient tcpclient)
        {
            _tcpclient = tcpclient;
            _networkStream = tcpclient.GetStream();
        }
       
    }
}
