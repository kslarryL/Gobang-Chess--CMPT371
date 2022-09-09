using GameComponent;
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
    public partial class MainFrame : Form
    {
        public GameBoard board;
        public MainFrame()
        {
            InitializeComponent();
            SetGameBoard();
            this.Text += " Player " + Configuration.playerID;
        }

        private void SetGameBoard()
        {
            board = new GameBoard();
            board.OnGameEnd += Board_OnGameEnd;
            this.Controls.Add(board);
            Rectangle rec = this.ClientRectangle;
            int xPos = (rec.Width - board.Width) / 2;
            int yPos = (rec.Height - MainMenuStrip.Height - board.Height) / 2 + MainMenuStrip.Height;
            board.Left = xPos;
            board.Top = yPos;
        }

        private void Board_OnGameEnd(object sender, GameComponent.GameEndEventArgs e)
        {
            MessageBox.Show(e.Message, "Information", MessageBoxButtons.OK);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void restartGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Configuration.playerID == 1)
            {
                this.board.ReStartGame();
                Configuration.client.SendToServer("RestartGame");
            }
            else
            {
                MessageBox.Show("Please tell the first player to restart the game.");
            }
        }
    }
}
