using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static SnakeGameExample.Model;
using System.Windows.Forms;
using Snake;

namespace SnakeGameExample
{
    internal class Controaller
    {
        readonly Model model;
        readonly Form form;
        private readonly View view;
        // Размер клетки игрового поля, в пикселях
        private const int CellSizePixel = 30;
        // Количество рядов в игровом поле
        private const int Rows = 15;
        // Количество столбцов в игровом поле
        private const int Cols = 20;
        public Controaller(Model m, Form f, View v)
        {
            model = m;
            form = f;
            view = v;
        }

        public void SnakeKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left:
                case Keys.A:
                    model.ChangeSnakeDirection(Direction.Right, Direction.Left);
                    break;
                case Keys.Right:
                case Keys.D:
                    model.ChangeSnakeDirection(Direction.Left, Direction.Right);
                    break;
                case Keys.Down:
                case Keys.S:
                    model.ChangeSnakeDirection(Direction.Up, Direction.Down);
                    break;
                case Keys.Up:
                case Keys.W:
                    model.ChangeSnakeDirection(Direction.Down, Direction.Up);
                    break;
            }
        }
    }
}
