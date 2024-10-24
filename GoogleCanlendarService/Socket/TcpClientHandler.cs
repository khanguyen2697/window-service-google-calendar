using System;
using System.Configuration;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GoogleCanlendarService.Socket
{
    internal class TcpClientHandler
    {
        private TcpClient tcpClient;
        private NetworkStream networkStream;
        public TcpClientHandler(string host, int port)
        {
            tcpClient = new TcpClient(host, port);
            networkStream = tcpClient.GetStream();
            SendClientType();
        }

        public async Task ListenForMessagesAsync(Action<string> onMessageReceived)
        {
            byte[] buffer = new byte[1024];

            while (true)
            {
                try
                {
                    int bytesRead = await networkStream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        onMessageReceived?.Invoke(message); 
                    }
                }
                catch (IOException ex)
                {
                    Console.WriteLine("Connection lost: " + ex.Message);
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error ListenForMessagesAsync: " + ex.ToString());
                    break;
                }
            }
        }

        public void SendMessage(string message)
        {
            if (networkStream != null)
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                networkStream.Write(data, 0, data.Length);
            }
        }

        private void SendClientType()
        {
            if (networkStream.CanWrite)
            {
                string tcpClientType = ConfigurationManager.AppSettings["TCPClientType"];
                if (tcpClientType == null)
                {
                    throw new Exception("Can not find TCPClientType in App.Config !");
                }
                byte[] clientTypeMessage = Encoding.ASCII.GetBytes(tcpClientType);
                networkStream.Write(clientTypeMessage, 0, clientTypeMessage.Length);
            }
        }

        public void Close()
        {
            networkStream?.Close();
            tcpClient?.Close();
        }
    }
}
