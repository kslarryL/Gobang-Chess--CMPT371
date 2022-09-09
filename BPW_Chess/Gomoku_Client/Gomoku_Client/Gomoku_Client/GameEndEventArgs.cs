using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameComponent
{
    /// <summary>
    /// Game end parameters
    /// </summary>
    public class GameEndEventArgs: EventArgs
    {
        /// <summary>
        /// chess color
        /// </summary>
        public int Color { get; set; }
        /// <summary>
        /// message
        /// </summary>
        public string Message { get; set; }

        public GameEndEventArgs(int color)
        {
            this.Color = color;
            if (color == 1)
            {
                this.Message = "Black Win";
            }else if(color == 2)
            {
                this.Message = "Pink Win";
            }
            else
            {
                this.Message = "White Win";
            }
        }

    }
}
