using Gomoku_Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gomoku
{
    public partial class WaitClientFrame : Form
    {
        public WaitClientFrame()
        {
            InitializeComponent();

        }

        private void CloseFrame()
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private delegate void CloseFrameDG();

        public void CallCloseFrameDG()
        {
            this.Invoke(new CloseFrameDG(CloseFrame));
        }

        private void ShowMessageDialog(string text)
        {
            MessageBox.Show(text);
        }

        private delegate void ShowMessageDialogDG(string text);

        public void CallShowMessageDialogDG(string text)
        {
            this.Invoke(new ShowMessageDialogDG(ShowMessageDialog), text);
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void StartGameButton_Click(object sender, EventArgs e)
        {
            //  after click start game, send server request of starting the game
            Configuration.client.SendToServer("StartGame,"+Configuration.playerID.ToString());
        }
    }
}
