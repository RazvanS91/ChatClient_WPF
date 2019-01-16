using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ChatClient
{
    public class Client
    {
        private TcpClient client = new TcpClient();
        private readonly IPAddress address = IPAddress.Parse("192.168.1.105");
        private StreamReader sReader;

        public Client(string username)
        {
            client.Connect(address, 12345);
            Stream stream = client.GetStream();
            sReader = new StreamReader(stream);
            SetUsername(username);
        }


        public void SendMessage(string message)
        {
            SendToServer(message);
        }

        private void SetUsername(string username)
        {
            SendToServer(username);
        }

        public string ReceiveFromServer()
        {
            var msg = sReader.GetData(sReader.ReadShort());
            return Encoding.ASCII.GetString(msg);
        }

        private void SendToServer(string message)
        {
            sReader.WriteShort((short)message.Length);
            sReader.WriteData(Encoding.ASCII.GetBytes(message));
        }

        public void Disconnect()
        {
            sReader.Close();
            client.Close();
        }
    }
}