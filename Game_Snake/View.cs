using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SnakeGameExample.Model;
using System.Windows.Forms;
using Snake;

namespace SnakeGameExample
{
    internal class View
    {
        readonly Model model;
        readonly Form form;
        readonly Snake.Snake formM;
        private const int CellSizePixel = 30;
        public View(Model m, Form f, Snake.Snake fM)
        {
            model = m;
            form = f;
            formM = fM;
        }
        private void InitializeSnake()
        {

            model.snakeDirection = Direction.Up;
            model.snake.Clear();
            model.snake.AddFirst(new Point(10, 10));
        }

        public void StartGame()
        {
            model.GenerateFood();
            InitializeSnake();
            model.gameEnd = false;
            formM.TimerGameLoop.Start();
            formM.TimerGameLoop.Interval = 300;
        }
        private void DrawSnake(Graphics g)
        {
            foreach (Point p in model.snake)
            {
                g.FillRectangle(Brushes.Lime, new Rectangle(
                    40 + p.Y * CellSizePixel,
                    p.X * CellSizePixel + 1,
                    CellSizePixel - 1,
                    CellSizePixel - 1));
            }
        }

        public void TimerTick(object sender, EventArgs e)
        {
            if (model.IsGameOver())
            {
                model.GameOver();
            }
            else
            {
                model.Move();
                formM.Invalidate();
            }
        }
        private void DrawFood(Graphics g)
        {
            g.FillRectangle(Brushes.Red, new Rectangle(
                40 + model.food.Y * CellSizePixel,
                model.food.X * CellSizePixel + 1,
                CellSizePixel - 1,
                CellSizePixel - 1));
        }
        private void GamePaint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            DrawFood(g);
            DrawSnake(g);
            DrawStatsAndKeyboardHints(g);
            DrawGrid(g);
        }

        private void GameLoad(object sender, EventArgs e)
        {
            formM.BackColor = Color.White;
            StartGame();
        }

        private void DrawStatsAndKeyboardHints(Graphics g)
        {
            Font fontStats = new Font("Consolas", 14);
            int statsLeftOffset = CellSizePixel * 15+ 50;
            g.DrawString(string.Format("Длина змейки: {0}", model.snake.Count), fontStats, Brushes.YellowGreen, new Point(statsLeftOffset, 100));
            g.DrawString("Управление:", fontStats, Brushes.Black, new Point(statsLeftOffset, 160));
            g.DrawString("Вверх: ↑ или W", fontStats, Brushes.Black, new Point(statsLeftOffset, 190));
            g.DrawString("Вниз:  ↓ или S", fontStats, Brushes.Black, new Point(statsLeftOffset, 220));
            g.DrawString("Влево: ← или A", fontStats, Brushes.Black, new Point(statsLeftOffset, 250));
            g.DrawString("Вправо: → или D", fontStats, Brushes.Black, new Point(statsLeftOffset, 280));
            g.DrawString("Выход: [Escape]", fontStats, Brushes.Black, new Point(statsLeftOffset, 310));
            fontStats.Dispose();
        }

        private void DrawGrid(Graphics g)
        {
            for (int row = 0; row <= 15; row++)
            {
                g.DrawLine(Pens.Plum,
                    new Point(0, 30 + row * CellSizePixel),
                    new Point(30 + CellSizePixel * CellSizePixel, 30 + row * CellSizePixel)
                );

                for (int col = 0; col <= 20; col++)
                {
                    g.DrawLine(Pens.Plum,
                        new Point(40 + col * CellSizePixel, 0),
                        new Point(40 + col * CellSizePixel, 30 + CellSizePixel)
                    );
                }
            }
        }
    }
}

