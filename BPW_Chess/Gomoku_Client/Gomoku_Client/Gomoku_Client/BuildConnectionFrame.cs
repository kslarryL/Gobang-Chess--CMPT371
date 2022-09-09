using Gomoku_Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gomoku
{
    public partial class BuildConnectionFrame : Form
    {
        public BuildConnectionFrame()
        {
            InitializeComponent();
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void ConnectToServerButton_Click(object sender, EventArgs e)
        {
            string serverIP = ServerIPTextBox.Text;
            int serverPort = Convert.ToInt32(ServerPortTextBox.Text);
            if (Configuration.client == null)
            {
                Configuration.client = new MyTCPClient();
            }
            if(Configuration.client.ConnectToServer(serverIP, serverPort))
            {
                // link to the server
                Configuration.client.InitReceiveThread();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Can not connect to this server");
            }
        }
    }
}
