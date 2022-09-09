using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.IO;

namespace Gomoku
{
    public class User
    {
        public bool normalExit = false;
        public TcpClient client;
        public StreamReader sr;
        public StreamWriter sw;
        public NetworkStream networkStream;

        public User(TcpClient client)
        {
            this.client = client;
            networkStream = client.GetStream();
            sr = new StreamReader(networkStream, System.Text.Encoding.UTF8);
            sw = new StreamWriter(networkStream, System.Text.Encoding.UTF8);
            sw.AutoFlush = true;
        }

        public void CloseUser()
        {
            sr.Close();
            sw.Close();
            networkStream.Close();
            client.Close();
        }
    }
}
