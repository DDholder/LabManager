using System.Net.Sockets;

namespace LabManager.Net
{
    public partial class Server
    {
        public class ClientMember
        {
            private readonly TcpClient client;
            public string name;

            public ClientMember(TcpClient tcpclient)
            {
                client = tcpclient;
            }

            public TcpClient TCPClient => client; 

            public void Send(byte[] buffer)
            {
                client.Client.Send(buffer);
            }

        }
    }
}
