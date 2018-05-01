using System.Net.Sockets;

namespace LabManager.Net
{
    public partial class Server
    {
        public class ClientMember
        {
            private readonly TcpClient _tcpclient;
            public string name;
            private readonly NetworkStream _networkStream;
            public ClientMember(TcpClient tcpclient)
            {
                _tcpclient = tcpclient;
                _networkStream = tcpclient.GetStream();
            }

            public TcpClient TCPClient => _tcpclient;

            public NetworkStream NetworkStream => _networkStream;

            public void Send(byte[] buffer) => _tcpclient.Client.Send(buffer);
            public void Write(byte[] buffer, int size, int offset = 0)
                => _networkStream.Write(buffer, offset, size);
        }
    }
}
