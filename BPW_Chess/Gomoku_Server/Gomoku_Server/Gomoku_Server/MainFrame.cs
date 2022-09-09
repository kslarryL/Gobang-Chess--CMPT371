using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gomoku_Server
{
    public partial class MainFrame : Form
    {
        public MainFrame()
        {
            InitializeComponent();

            IPText.Text = Configuration.localIP.ToString();
            PortText.Text = Configuration.port.ToString();
        }

        private void AddLogText(string text)
        {
            LogText.AppendText(text + "\r\n");
            LogText.ScrollToCaret();
        }

        private delegate void AddLogTextDG(string text);

        public void CallAddLogTextDG(string text)
        {
            this.Invoke(new AddLogTextDG(AddLogText), text);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }
    }
}
