using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Drawing;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Gomoku;
using Gomoku_Client;

namespace Gomoku
{
    public class MyTCPClient
    {
        public TcpClient client;
        public StreamWriter sw;
        public StreamReader sr;
        public NetworkStream networkStream;

        public bool normalExit = false;
        public MyTCPClient()
        {
            //crearte client
            client = new TcpClient();
        }
        public bool ConnectToServer(string serverIP, int serverPort)
        {
            try
            {
                client.Connect(serverIP, serverPort);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public void CreateNetStream()
        {
            networkStream = client.GetStream();
            sr = new StreamReader(networkStream, System.Text.Encoding.UTF8);
            sw = new StreamWriter(networkStream, System.Text.Encoding.UTF8);
            sw.AutoFlush = true;
        }

        public void InitReceiveThread()
        {
            Debug.WriteLine("Client Start");
            Thread threadReceive = new Thread(new ThreadStart(ReceiveData));
            threadReceive.Start();
        }

        private void ReceiveData()
        {
            CreateNetStream();
            while (normalExit == false)
            {
                string receiveString = null;
                try
                {
                    receiveString = sr.ReadLine();
                }
                catch
                {
                    break;
                }
                if (receiveString == null)
                {
                    if (normalExit == false)
                    {
                        MessageBox.Show("Lost contact with the host!");
                    }
                    break;
                }
                Debug.WriteLine(receiveString);
                string[] splitString = receiveString.Split(',');
                switch (splitString[0])
                {
                    case "PlaceChess":
                        int x = int.Parse(splitString[1]);
                        int y = int.Parse(splitString[2]);
                        int color = int.Parse(splitString[3]);
                        Point point = new Point(x, y);
                        Configuration.mainFrame.board.CallPlaceOpponentChessDG(point, color);
                        break;
                    case "AssignPlayerID":
                        Configuration.playerID = int.Parse(splitString[1]);
                        // creating waiting windows
                        WaitClientFrame waitClientFrame = new WaitClientFrame();
                        Configuration.waitClientFrame = waitClientFrame;
                        break;
                    case "EnterGame":
                        // when recieve "start game" from the server, start the game
                        Configuration.waitClientFrame.CallCloseFrameDG();
                        break;
                    case "InsufficientConnections":
                        //  When the number of the client is not equal to 3, cannot start the game
                        Configuration.waitClientFrame.CallShowMessageDialogDG("Can't start game, Please wait opponents to connect.");
                        break;
                    case "RestartGame":
                        Configuration.mainFrame.board.CallReStartGameDG();
                        break;
                    default:
                        normalExit = true;
                        break;
                }
            }
        }

        public void SendToServer(string str)
        {
            try
            {
                sw.WriteLine(str);
                sw.Flush();
            }
            catch
            {
                return;
            }
         }

    }
}
