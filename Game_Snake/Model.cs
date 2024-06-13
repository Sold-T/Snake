using Snake;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnakeGameExample
{
    internal class Model
    {
        readonly Form form;
        readonly View view;

        public Model(Form f, View v, Model m)
        {
            form = f;
            view = v;
        }

        // Размер клетки игрового поля, в пикселях
        private const int CellSizePixel = 30;
        // Количество рядов в игровом поле
        private const int Rows = 15;
        // Количество столбцов в игровом поле
        private const int Cols = 20;

        // Все стандартно - направления
        public enum Direction
        {
            Left,
            Right,
            Up,
            Down
        }

        public Direction snakeDirection = Direction.Up;
        public LinkedList<Point> snake = new LinkedList<Point>();
        public Point food;
        private Random random = new Random();
        public bool gameEnd;

        /// БЛОК Model \\\
        public void GenerateFood()
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

        }

        public void Move()
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
                // "Змейка" съела саму себя! Конец игры!
                form.Invalidate();
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


        public bool IsGameOver()
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

        public void GameOver()
        {
            gameEnd = true;
        }

        public void ChangeSnakeDirection(Direction restrictedDirection, Direction newDirection)
        {
            if (snakeDirection != restrictedDirection)
            {
                snakeDirection = newDirection;
            }
        }
    }
}
