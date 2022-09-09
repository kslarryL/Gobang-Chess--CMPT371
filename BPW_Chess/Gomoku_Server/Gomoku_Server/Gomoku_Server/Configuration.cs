using Gomoku;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Gomoku_Server
{
    public static class Configuration
    {
        public static MyTCPServer server;
        public static IPAddress localIP;
        public static int port = 51888;
        public static MainFrame mainFrame;
        static Configuration()
        {
            IPAddress[] addrIP = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress addr in addrIP)
            {
                if (addr.AddressFamily.ToString() == "InterNetwork")
                {
                    Debug.WriteLine(addr.ToString());
                    localIP = addr;
                    break;
                }
            }
        }
    }

}
