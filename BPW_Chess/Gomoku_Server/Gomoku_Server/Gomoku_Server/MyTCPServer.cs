using System;
using System.Collections.Generic;
using System.Net;
using System.Windows.Forms;
using System.Net.Sockets;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Gomoku_Server;

namespace Gomoku
{
    public class MyTCPServer
    {
        public IPAddress localAddress = Configuration.localIP;
        public int port = Configuration.port;
        public TcpListener myListener;
        private int id = 1;
        public int ID
        {
            set{ id = value;}
        }
        public bool exitServerThread = false;
        public List<User> users = new List<User>();
        public int maxNumberOfPlayer = 2;

        public MyTCPServer()
        {
            StartServer();
        }
        public void StartServer()
        {
            myListener = new TcpListener(localAddress, port);
            myListener.Start();
            for(int i = 0; i < maxNumberOfPlayer; i++)
            {
                Thread myThread = new Thread(ListenClientConnect);
                myThread.Start();
            }
        }
        private object lockedObj = new object();
        private int GetID()
        {
            lock (lockedObj)
            {
                id++;
            }
            return id;
        }

        private void ListenClientConnect()
        {
            // keep listen to the link while there are 3 player link to the server
            while(users.Count < 3)
            {
                TcpClient newClient = null;
                try
                {
                    newClient = myListener.AcceptTcpClient();
                }
                catch
                {
                    return;
                }
                User user = new User(newClient);
                users.Add(user);
                Configuration.mainFrame.CallAddLogTextDG("A client has connected to this server.");
                Configuration.mainFrame.CallAddLogTextDG("There are " + users.Count.ToString() + " clients.");
                // playerID Once set up the connection, Send the playerID to the server
                SendToUser(user, "AssignPlayerID," + users.Count);
                Thread threadReceive = new Thread(ReceiveData);
                threadReceive.Start(user);
            }
        }

        private void ReceiveData(object obj)
        {
            User user = (User)obj;
            while (user.normalExit == false)
            {
                string receiveString = null;
                try
                {
                    receiveString = user.sr.ReadLine();
                }
                catch
                {
                    if (user.normalExit == false)
                    {
                        MessageBox.Show(user.client.Client.RemoteEndPoint + " Lost contact and terminated receiving the user information");
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
                        // recive start order, and contact to all the clients
                        Configuration.server.SendToAllUser(receiveString);
                        Configuration.mainFrame.CallAddLogTextDG("Receive the 'PlaceChess' command from player " + color.ToString() + ".");
                        Configuration.mainFrame.CallAddLogTextDG("Player " + color.ToString() + " wants to place chess at (" + x.ToString() + "," + y.ToString() + ").");
                        break;
                    case "StartGame":
                        int playerID = int.Parse(splitString[1]);
                        //  recieve any one client "start game" order, announce all the client server
                        Configuration.mainFrame.CallAddLogTextDG("Receive the 'StartGame' command from player " + playerID.ToString() + ".");
                        if (users.Count < 3)
                        {
                            SendToUserByPlayerID(playerID, "InsufficientConnections");
                            Configuration.mainFrame.CallAddLogTextDG("Can't start the game, because there are less than 3 players.");
                        }
                        else
                        {
                            SendToAllUser("EnterGame");
                            Configuration.mainFrame.CallAddLogTextDG("The game started successfully.");
                        }
                        break;
                    case "RestartGame":
                        Configuration.mainFrame.CallAddLogTextDG("Receive the 'RestartGame' command.");
                        SendToAllUser("RestartGame");
                        break;
                    default:
                        break;
                }
            }
            user.CloseUser();
        }
        public void SendToAllUser(string str)
        {
            foreach(User user in users)
            {
                SendToUser(user, str);
            }
        }
        public void SendToUser(User user, string str)
        {
            try
            {
                user.sw.WriteLine(str);
                user.sw.Flush();
            }
            catch
            {
                return;
            }
        }
        public void SendToUserByPlayerID(int id, string str)
        {
            SendToUser(users[id-1], str);
        }

    }
}
