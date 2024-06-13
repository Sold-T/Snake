using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using WMPLib;

namespace Snake
{
    public partial class Snake : Form
    {
        // Размер клетки игрового поля, в пикселях
        private const int CellSizePixel = 30;
        // Количество рядов в игровом поле
        private const int Rows = 15;
        // Количество столбцов в игровом поле
        private const int Cols = 20;

        // Все стандартно - направления
        private enum Direction
        {
            Left,
            Right,
            Up,
            Down
        }
        // Текущее направление змейки
        private Direction snakeDirection = Direction.Up;
        // Список точек, содержащих координаты змейки
        private LinkedList<Point> snake = new LinkedList<Point>();
        // Еда
        private Point food;
        // Рандом
        private Random random = new Random();
        // игра завершена?
        private bool gameEnd;
        WindowsMediaPlayer player = new WindowsMediaPlayer();
        public Snake()
        {
            InitializeComponent();
        }

        private void InitializeSnake()
        {
            player.controls.play();
            snakeDirection = Direction.Up;
            snake.Clear();
            snake.AddFirst(new Point(10, 10));
        }

        private void GenerateFood()
        {
            bool isFoodClashWithSnake;
            do
            {
                food = new Point(random.Next(0, 15), random.Next(0, 15));
                isFoodClashWithSnake = false;
                foreach (Point p in snake)
                {
                    if (p.X == food.X && p.Y == food.Y)
                    {
                        isFoodClashWithSnake = true;
                        break;
                    }
                }
            } while (isFoodClashWithSnake);

            TimerGameLoop.Interval -= 5;
        }

        private void Move()
        {
            LinkedListNode<Point> head = snake.First;
            Point newPointHead = new Point(0, 0);
            switch (snakeDirection)
            {
                case Direction.Left:
                    newPointHead = new Point(head.Value.X, head.Value.Y - 1);
                    break;
                case Direction.Right:
                    newPointHead = new Point(head.Value.X, head.Value.Y + 1);
                    break;
                case Direction.Down:
                    newPointHead = new Point(head.Value.X + 1, head.Value.Y);
                    break;
                case Direction.Up:
                    newPointHead = new Point(head.Value.X - 1, head.Value.Y);
                    break;
            }

            if (snake.Any(point => point.X == newPointHead.X && point.Y == newPointHead.Y))
            {
                // змейка съела саму себя! Конец игры!
                Invalidate();
                GameOver();
                return;
            }

            snake.AddFirst(newPointHead);

            if (newPointHead.X == food.X && newPointHead.Y == food.Y)
            {
                GenerateFood();
            }
            else
            {
                snake.RemoveLast();
            }
        }

        private void StartGame()
        {
            if (MessageBox.Show("Начать игру?", "Змейка", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                GenerateFood();
                InitializeSnake();
                gameEnd = false;
                TimerGameLoop.Start();
                TimerGameLoop.Interval = 300;
                player.URL = "Mundian.mp3";
            }
            else
            {
                Application.Exit();
            }
        }

        private bool IsGameOver()
        {
            LinkedListNode<Point> head = snake.First;
            switch (snakeDirection)
            {
                case Direction.Left:
                    return head.Value.Y - 1 < 0;
                case Direction.Right:
                    return head.Value.Y + 1 >= Cols;
                case Direction.Down:
                    return head.Value.X + 1 >= Rows;
                case Direction.Up:
                    return head.Value.X - 1 < 0;
            }
            return false;
        }

        private void GameOver()
        {
            gameEnd = true;
            TimerGameLoop.Stop();
            if (MessageBox.Show("Конец игры! Начать заново?", "Конец игры", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                StartGame();
            }
            else
            {
                Application.Exit();
            }
        }

        private void TimerTick(object sender, EventArgs e)
        {
            if (IsGameOver())
            {
                GameOver();
            }
            else
            {
                Move();
                Invalidate();
            }
        }
        private void ChangeSnakeDirection(Direction restrictedDirection, Direction newDirection)
        {
            if (snakeDirection != restrictedDirection)
            {
                snakeDirection = newDirection;
            }
        }

        private void SnakeKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left:
                case Keys.A:
                    ChangeSnakeDirection(Direction.Right, Direction.Left);
                    break;
                case Keys.Right:
                case Keys.D:
                    ChangeSnakeDirection(Direction.Left, Direction.Right);
                    break;
                case Keys.Down:
                case Keys.S:
                    ChangeSnakeDirection(Direction.Up, Direction.Down);
                    break;
                case Keys.Up:
                case Keys.W:
                    ChangeSnakeDirection(Direction.Down, Direction.Up);
                    break;
                case Keys.Escape:
                    TimerGameLoop.Stop();
                    Close();
                    break;
                case Keys.Space:
                    if (gameEnd && !TimerGameLoop.Enabled)
                    {
                        StartGame();
                    }
                    break;
            }
        }

        private void DrawSnake(Graphics g)
        {
            int snakePoint = 0;
            foreach (Point p in snake)
            {
                if (snakePoint > 0)
                    g.FillRectangle(Brushes.Lime, new Rectangle(
                    40 + p.Y * CellSizePixel,
                    p.X * CellSizePixel + 1,
                    CellSizePixel - 1,
                    CellSizePixel - 1));

                if (snakePoint == 0)
                {
                    g.FillRectangle(Brushes.Lime, new Rectangle(
                    40 + p.Y * CellSizePixel,
                    p.X * CellSizePixel + 1,
                    CellSizePixel - 1,
                    CellSizePixel - 1));

                    g.FillEllipse(Brushes.Black, new Rectangle(
                    55 + p.Y * CellSizePixel,
                    p.X * CellSizePixel + 5,
                    5,
                    5));


                    g.FillEllipse(Brushes.Black, new Rectangle(
                    45 + p.Y * CellSizePixel,
                    p.X * CellSizePixel + 5,
                    5,
                    5));
                }
                snakePoint++;
            }
        }

        // Добавим переменную для отслеживания состояния анимации
        private bool isFoodAnimated = true;

        // Метод для отрисовки яблока с анимацией
        private void DrawFood(Graphics g)
        {
            if (isFoodAnimated)
            {
                // Анимация мерцания яблока
                if (DateTime.Now.Millisecond % 500 < 250)
                {
                    g.FillEllipse(Brushes.OrangeRed, new Rectangle(
                        40 + food.Y * CellSizePixel,
                        food.X * CellSizePixel + 1,
                        CellSizePixel - 1,
                        CellSizePixel - 1));
                }
                else
                {
                    g.FillEllipse(Brushes.Red, new Rectangle(
                        40 + food.Y * CellSizePixel,
                        food.X * CellSizePixel + 1,
                        CellSizePixel - 1,
                        CellSizePixel - 1));
                }
            }
            else
            {
                // Отрисовка яблока без анимации
                g.FillEllipse(Brushes.Orange, new Rectangle(
                    40 + food.Y * CellSizePixel,
                    food.X * CellSizePixel + 1,
                    CellSizePixel - 1,
                    CellSizePixel - 1));
            }
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
            DoubleBuffered = true;
            BackColor = Color.Linen;
            StartGame();
        }

        private void DrawStatsAndKeyboardHints(Graphics g)
        {
            Font fontStats = new Font("Consolas", 14);
            int statsLeftOffset = CellSizePixel * Cols + 50;
            g.DrawString(string.Format("Длина змейки: {0}", snake.Count), fontStats, Brushes.YellowGreen, new Point(statsLeftOffset, 100));
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
            for (int row = -1; row <= 20; row++)
            {
                g.DrawLine(Pens.Green,
                    new Point(40, 30 + row * CellSizePixel),
                    new Point(40 + CellSizePixel * 20, 30 + row * CellSizePixel)
                );

                for (int col = 0; col <= 20; col++)
                {
                    g.DrawLine(Pens.Green,
                        new Point(40 + col * CellSizePixel, 0),
                        new Point(40 + col * CellSizePixel, 30 + CellSizePixel * 20)
                    );
                }
            }
        }
    }
}
