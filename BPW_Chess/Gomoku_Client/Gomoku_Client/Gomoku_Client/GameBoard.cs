using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Gomoku;
using Gomoku_Client;

namespace GameComponent
{
    public class GameBoard : Panel
    {
        private enum GameMode
        {
            PersonToPerson,
            PersonToComputer
        }
        #region data and array
        private static readonly Bitmap blackChessImg = Gomoku_Client.Properties.Resources.black;
        private static readonly Bitmap pinkChessImg = Gomoku_Client.Properties.Resources.pink;
        private static readonly Bitmap whiteChessImg = Gomoku_Client.Properties.Resources.white;
        // the size of game board is gameSize*gameSize
        private const int gameSize = 15;
        // The size of a grid
        private const int blockSize = 30;
        // Chess picture size
        private readonly int chessmanSize = 24;
        // Coordinate offset
        private static readonly int delta = 42;
        // Square of distance
        private static readonly int eDistance = 100;
        // text size
        private static readonly int textSize = 10;
        // text color
        private static readonly Color textColor = Color.FromArgb(38, 38, 38);
        // whether the game starts
        private bool isStart = true;
        // Whether we play chess
        private bool isOurTurn = false;
        // 1means black，2means pink，3means white
        private int color = 1;
        // rocord every location's color, 0 means this location is empty
        private int[,] map = new int[gameSize, gameSize];
        // horizontal count
        private int hSum;
        // vertical count
        private int vSum;
        // left oblique count
        private int lSum;
        // right oblique count
        private int rSum;
        // size of the plate
        private readonly int boradSize;

        // game end events
        public event EventHandler<GameEndEventArgs> OnGameEnd;
        #endregion

        public GameBoard()
        {
            SetStyle(ControlStyles.UserPaint, true);
            // Do not erase the background
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            // Double buffering
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();
            boradSize = GetGameBoardSize();
            this.Size = new Size(boradSize, boradSize);
            Createbackgroudimage();
            if (Configuration.playerID == 1)
            {
                isOurTurn = true;
            }
            color = Configuration.playerID;
            // show your own chess color at left up corner
            AddChessman(IndexToScreen(-1, -1), color);
        }

        /// <summary>
        /// Get the size of the game panel
        /// </summary>
        /// <returns></returns>
        private int GetGameBoardSize()
        {
            return (gameSize - 1) * blockSize + delta * 2;
        }

        /// <summary>
        /// Tool method, change the internal coordinates into the coordinates on the screen
        /// </summary>
        /// <param name="x">Abscissa</param>
        /// <param name="y">Ordinate</param>
        /// <returns></returns>
        private Point IndexToScreen(int x, int y)
        {
            return new Point(x * blockSize + delta, y * blockSize + delta);
        }

        /// <summary>
        /// Draw the background picture of the checkerboard of gamesize*gamesize
        /// </summary>
        private void Createbackgroudimage()
        {
            int boardSize = GetGameBoardSize();
            Bitmap img = new Bitmap(boardSize, boardSize);
            Graphics g = Graphics.FromImage(img);
            drawLinesAndCircles(boardSize, g);
            DrawCoordinateText(g);
            g.Dispose();
            this.BackgroundImage = img;
        }

        /// <summary>
        /// Draw horizontal and vertical lines and small origin
        /// </summary>
        /// <param name="boardSize"></param>
        /// <param name="g"></param>
        private void drawLinesAndCircles(int boardSize, Graphics g)
        {
            g.FillRectangle(Brushes.BurlyWood, new Rectangle(0, 0, boardSize, boardSize));
            // Create pen object
            Pen mypen = new Pen(Color.Black, 1);
            SolidBrush brush = new SolidBrush(Color.Black);
            // (Delta, delta) is the initial point, and the width of each grid is blocksize
            for (int i = 0; i < gameSize; i++)
            {
                // Draw a horizontal line
                Point pHorizontal1 = IndexToScreen(0, i);
                Point pHorizontal2 = IndexToScreen(gameSize - 1, i);
                g.DrawLine(mypen, pHorizontal1, pHorizontal2);
                // Draw a vertical line
                Point pVertical1 = IndexToScreen(i, 0);
                Point pVertical2 = IndexToScreen(i, gameSize - 1);
                g.DrawLine(mypen, pVertical1, pVertical2);
            }
            // circle
            Point p1 = IndexToScreen(3, 3);
            Point p2 = IndexToScreen(12, 3);
            Point p3 = IndexToScreen(3, 12);
            Point p4 = IndexToScreen(12, 12);
            g.FillEllipse(brush, p1.X - 3, p1.Y - 3, 6, 6);
            g.FillEllipse(brush, p2.X - 3, p2.Y - 3, 6, 6);
            g.FillEllipse(brush, p3.X - 3, p3.Y - 3, 6, 6);
            g.FillEllipse(brush, p4.X - 3, p4.Y - 3, 6, 6);
        }

        /// <summary>
        /// Draw coordinate text
        /// </summary>
        /// <param name="g"></param>
        private void DrawCoordinateText(Graphics g)
        {
            Font myFont = new Font("Times New Roman", textSize, FontStyle.Bold);
            Brush brush = new SolidBrush(textColor);
            StringFormat format = new StringFormat();
            // Vertically centered
            format.LineAlignment = StringAlignment.Center;
            // horizontally
            format.Alignment = StringAlignment.Center;
            for (int i = 0; i < gameSize; i++)
            {
                Point pHorizontal = IndexToScreen(i, 0);
                Point pVertical = IndexToScreen(0, i);
                Rectangle rHorizontal = new Rectangle(pHorizontal.X - blockSize / 2, pHorizontal.Y - blockSize - chessmanSize / 2, blockSize, blockSize);
                Rectangle rVertical = new Rectangle(pVertical.X - blockSize - chessmanSize / 2, pVertical.Y - blockSize / 2, blockSize, blockSize);
                g.DrawString((i + 1).ToString(), myFont, brush, rHorizontal, format);
                g.DrawString((i + 1).ToString(), myFont, brush, rVertical, format);
            }
        }

        private bool IsEffectiveArea(int x, int y, ref Point p)
        {
            for (int i = 0; i <= gameSize; i++)
            {
                for (int j = 0; j <= gameSize; j++)
                {
                    Point pMain = IndexToScreen(i, j);
                    Point pCompare = new Point(x, y);
                    int distance = CalDistance(pMain, pCompare);
                    if (distance <= eDistance)
                    {
                        if (map[j, i] != 0)
                        {
                            return false;
                        }
                        else
                        {
                            p.X = i;
                            p.Y = j;
                            return true;
                        }

                    }
                }
            }
            return false;
        }

        private int CalDistance(Point p1, Point p2)
        {
            int deltaX = p1.X - p2.X;
            int deltaY = p1.Y - p2.Y;
            int distance = deltaX * deltaX + deltaY * deltaY;
            return distance;
        }

        private bool ExistSameColor(int xpos, int ypos, int color)
        {
            return map[ypos, xpos] == color;
        }

        private void AllDirectionsCount(int xpos, int ypos, int color)
        {
            // Count in horizontal direction
            hSum = 1;
            for (int i = xpos - 1; i >= 0; i--)
            {
                if (!ExistSameColor(i, ypos, color))
                {
                    break;
                }
                hSum++;
            }
            for (int i = xpos + 1; i <= gameSize; i++)
            {
                if (!ExistSameColor(i, ypos, color))
                {
                    break;
                }
                hSum++;
            }

            // Longitudinal count
            vSum = 1;
            for (int i = ypos - 1; i >= 0; i--)
            {
                if (!ExistSameColor(xpos, i, color))
                {
                    break;
                }
                vSum++;
            }
            for (int i = ypos + 1; i <= gameSize; i++)
            {
                if (!ExistSameColor(xpos, i, color))
                {
                    break;
                }
                vSum++;
            }

            // Left slash count
            lSum = 1;
            for (int i = xpos - 1, j = ypos - 1; i >= 0 && j >= 0; i--, j--)
            {
                if (!ExistSameColor(i, j, color))
                {
                    break;
                }
                lSum++;
            }
            for (int i = xpos + 1, j = ypos + 1; i <= gameSize && j <= gameSize; i++, j++)
            {
                if (!ExistSameColor(i, j, color))
                {
                    break;
                }
                lSum++;
            }
            // Judgment of right slash
            rSum = 1;
            for (int i = xpos - 1, j = ypos + 1; i >= 0 && j <= gameSize; i--, j++)
            {
                if (!ExistSameColor(i, j, color))
                {
                    break;
                }
                rSum++;
            }
            for (int i = xpos + 1, j = ypos - 1; i <= gameSize && j >= 0; i++, j--)
            {
                if (!ExistSameColor(i, j, color))
                {
                    break;
                }
                rSum++;
            }
        }

        private bool IsGameEnd(Point p, int color)
        {
            AllDirectionsCount(p.X, p.Y, color);
            if (hSum >= 5 || vSum >= 5 || lSum >= 5 || rSum >= 5)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void AddChessman(Point p, int color)
        {
            Bitmap img;
            if (color == 1)
            {
                img = blackChessImg;
            }else if (color == 2)
            {
                img = pinkChessImg;
            }
            else
            {
                img = whiteChessImg;
            }
            Graphics g = Graphics.FromImage(this.BackgroundImage);
            p.X = p.X - chessmanSize / 2;
            p.Y = p.Y - chessmanSize / 2;
            g.DrawImage(img, p.X, p.Y, chessmanSize, chessmanSize);
            g.Dispose();
            this.Invalidate();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (isStart)
            {
                if (e.Button == MouseButtons.Left && isOurTurn)
                {
                    Point p = new Point(0, 0);
                    if (IsEffectiveArea(e.X, e.Y, ref p))
                    {
                        PlaceChess(p);
                        isOurTurn = false;
                        // use own playerid, let server sned to other client
                        Configuration.client.SendToServer("PlaceChess," + p.X + "," + p.Y + "," + Configuration.playerID);
                    }
                }
                base.OnMouseClick(e);
            }
        }

        private void PlaceChess(Point p)
        {
            map[p.Y, p.X] = color;
            AddChessman(IndexToScreen(p.X, p.Y), color);
            if (IsGameEnd(p, color))
            {
                OnGameEnd(this, new GameEndEventArgs(color));
                isStart = false;
            }
        }

        private void PlaceOpponentChess(Point p, int color)
        {
            map[p.Y, p.X] = color;
            AddChessman(IndexToScreen(p.X, p.Y), color);
            if (IsGameEnd(p, color))
            {
                OnGameEnd(this, new GameEndEventArgs(color));
                isStart = false;
            }
           
            if ((this.color - color + 3) % 3 == 1)
            {
                isOurTurn = true;
            }
        }

        private delegate void PlaceOpponentChessDG(Point p, int color);

        public void CallPlaceOpponentChessDG(Point p, int color)
        {
            this.Invoke(new PlaceOpponentChessDG(PlaceOpponentChess), p, color);
        }

        public void ReStartGame()
        {
            Createbackgroudimage();
            // show own chess color at left corner
            AddChessman(IndexToScreen(-1, -1), color);
            map = new int[gameSize + 1, gameSize + 1];
            isStart = true;
            // set host play chess first
            if (Configuration.playerID == 1)
            {
                isOurTurn = true;
            }
        }

        private delegate void ReStartGameDG();

        public void CallReStartGameDG()
        {
            this.Invoke(new ReStartGameDG(ReStartGame));
        }
    }
}
