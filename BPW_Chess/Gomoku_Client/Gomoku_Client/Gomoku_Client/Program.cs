using Gomoku;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gomoku_Client
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            BuildConnectionFrame buildConnectionFrame = new BuildConnectionFrame();
            if (buildConnectionFrame.ShowDialog() == DialogResult.OK)
            {
                //  creating waiting windows
                WaitClientFrame waitClientFrame = new WaitClientFrame();
                Configuration.waitClientFrame = waitClientFrame;
                // When the waiting windows end, enter the game or waiting the instruction form the server.
                if (Configuration.waitClientFrame.ShowDialog() == DialogResult.OK)
                {
                    MainFrame mainFrame = new MainFrame();
                    Configuration.mainFrame = mainFrame;
                    Application.Run(mainFrame);
                }
            }
        }
    }
}
