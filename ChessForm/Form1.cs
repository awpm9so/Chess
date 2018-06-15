using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ChessRules;

namespace ChessForm
{
    public partial class FormChess : Form
    {
        const int CELL = 60;
        Panel[,] map;
        Chess chess;

        bool next;

        int xFrom;
        int yFrom;
        public FormChess()
        {
            // rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1
            InitializeComponent();
            InitBoard();
            next = true;
            chess = new Chess();
            ShowPosition();
        }
        /// <summary>
        /// Создание шахматной доски. 
        /// Во вложенном цикле заполняем двумерный массив Panel[,] map, который выступает в качестве доски
        /// </summary>
        void InitBoard()
        {
            map = new Panel[8, 8];
            for (int x = 0; x < 8; x++)
                for (int y = 0; y < 8; y++)
                    map[x, y] = AddPanel(x, y);
        }
        /// <summary>
        /// Отображение текущего расположения фигур на доске
        /// </summary>
        void ShowPosition()
        {
            for (int x = 0; x < 8; x++)
                for (int y = 0; y < 8; y++)
                    ShowFigure(x, y, chess.GetFigureAt(x,y));
            MarkSquares();
        }
        /// <summary>
        /// Отображение изображения фигуры на соответсвующей клетке
        /// </summary>
        /// <param name="x">Координата клетки по оси Y</param>
        /// <param name="y">Координата коетки по оси X</param>
        /// <param name="figure">ASCII представление фигуры (B - белый слон, q - черынй ферзь и т.д.)</param>
        void ShowFigure(int x ,int y , char figure)
        {
            map[x, y].BackgroundImage = GetFigureImage(figure);
        }
        /// <summary>
        /// По полученному ASCII символу возвращет нужное изображение фигигуры
        /// </summary>
        /// <param name="figure">ASCII представление фигуры</param>
        /// <returns>Изображение фигуры</returns>
        Image GetFigureImage(char figure)
        {
            switch (figure)
            {
                case 'R': return Properties.Resources.WhiteRook;
                case 'N': return Properties.Resources.WhiteKnight;
                case 'B': return Properties.Resources.WhiteBishop;
                case 'Q': return Properties.Resources.WhiteQueen;
                case 'K': return Properties.Resources.WhiteKing;
                case 'P': return Properties.Resources.WhitePawn;

                case 'r': return Properties.Resources.BlackRook;
                case 'n': return Properties.Resources.BlackKnight;
                case 'b': return Properties.Resources.BlackBishop;
                case 'q': return Properties.Resources.BlackQueen;
                case 'k': return Properties.Resources.BlackKing;
                case 'p': return Properties.Resources.BlackPawn;

                default:return null;
            }
        }
        /// <summary>
        /// Создание панели (шахматной клетки)
        /// </summary>
        /// <param name="x">Координата клетки по оси Y</param>
        /// <param name="y">Координата коетки по оси X</param>
        /// <returns>Готовая клетка, расположенная на доске в зависисти от своих координат</returns>
        Panel AddPanel(int x , int y) // рисуем клетки
        {
            Panel panel = new System.Windows.Forms.Panel();
            panel.BackColor = GetColor(x, y);
            panel.Location = GetLocation(x, y);
            panel.Name = "p" + x + y;
            panel.Size = new System.Drawing.Size(CELL, CELL);
            panel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            panel.MouseClick += new System.Windows.Forms.MouseEventHandler(panel_MouseClick);
            Board.Controls.Add(panel);
            return panel;
        }
        /// <summary>
        /// Раскрашивание доски в черно-белый цвет
        /// Если сумма координат чётна, то клетка серая, иначе белая
        /// </summary>
        /// <param name="x">Координата клетки по оси X</param>
        /// <param name="y">Координата клетки по оси Y</param>
        /// <returns>Цвет клетки</returns>
        Color GetColor(int x, int y)
        {
            return (x + y) % 2 == 0 ? Color.Gray : Color.White;
        }
        /// <summary>
        /// Подсветка клеток
        /// Если сумма координат чётна, то клетка зелёная, иначе светло-зеленая
        /// </summary>
        /// <param name="x">Координата клетки по оси X</param>
        /// <param name="y">Координата клетки по оси Y</param>
        /// <returns>Цвет клетки</returns>
        Color GetMarkedColor(int x, int y)
        {
            return (x + y) % 2 == 0 ? Color.Green : Color.LightGreen;
        }
        /// <summary>
        /// Метод определяет расположение клетки на шахматной доске
        /// </summary>
        /// <param name="x">Координата клетки по оси X</param>
        /// <param name="y">Координата клетки по оси Y</param>
        /// <returns>Точка, где должен быть центр отдельной клетки на шахматной доске</returns>
        Point GetLocation(int x, int y)
        {
            return new Point(CELL / 2 + x * CELL, CELL / 2 + (7 - y) * CELL);
        }
        /// <summary>
        /// Обработчик событий мыши для передвижения фигур 
        /// </summary>
        /// <param name="sender">Объет по которому был выполнен клик мышью</param>
        /// <param name="e">Данные о том, в какой области доски был выполнен клик</param>
        private void panel_MouseClick(object sender, MouseEventArgs e)
        {
            string xy = ((Panel)sender).Name.Substring(1);
            int x = xy[0] - '0';
            int y = xy[1] - '0';


            if (next)
            {
                next = false;
                xFrom = x;
                yFrom = y;
            }
            else
            {
                next = true;
                string figure = chess.GetFigureAt(xFrom, yFrom).ToString();
                string move = figure + ToCoord(xFrom, yFrom) + ToCoord(x, y);
                chess = chess.Move(move);                               
            }
            ShowPosition();
        }
        /// <summary>
        ///  Метод, который "чистит" доску от ненужных выделений клеток.
        ///  После этого (в завистисто от состояния игры(next) - ожидание выбора фигуры/фигура выбрана) вызывает методы,
        ///  которые подсвечивают нужные клетки
        /// </summary>
        void MarkSquares()
        {
            for (int x = 0; x < 8; x++)
                for (int y = 0; y < 8; y++)
                    map[x, y].BackColor = GetColor(x,y);

            if (next) MarkSquaresFrom(); 
            else MarkSquaresTo(); 
        }
        /// <summary>
        /// Подсветка тех клеток, с которых фигуры имею доступные ходы (ситуация, когда не выбрана конкретаня фигура для хода)
        /// </summary>
        void MarkSquaresFrom()
        {
            foreach (string move in chess.YieldValidMoves())
            {
                int x = move[1] - 'a';
                int y = move[2] - '1';
                map[x, y].BackColor = GetMarkedColor(x,y);                
            }
        }
        /// <summary>
        /// Подсветка клеток, на которые может переместиться выбранная финура
        /// Здесь же проверка на конец игры(мат).
        /// </summary>
        void MarkSquaresTo()
        {
            string s = chess.GetFigureAt(xFrom, yFrom) + ToCoord(xFrom, yFrom);
            foreach (string move in chess.YieldValidMoves())
                if (move.StartsWith (s))
                {
                    int x = move[3] - 'a';
                    int y = move[4] - '1';
                    map[x, y].BackColor = GetMarkedColor(x, y);
                } 
            if (chess.IsCheckmate)
            {
                int count_move = int.Parse(chess.fen[chess.fen.Length - 1].ToString()) - 1;
                if (chess.IsWhiteMove)
                {
                    MessageBox.Show("Мат! Черные победили!\nКоличесво ходов в партии " + count_move, "Конец игры", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Мат! Белые победили!\nКоличесво ходов в партии " + count_move, "Конец игры", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                var result = MessageBox.Show("Начать новую игру?", "Новая игра", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    chess = new Chess();
                }
                else
                {
                    Application.Exit();
                }
            }                       
        }
        /// <summary>
        /// Перевод координат в строковое представление, которое необходимо для поиска подсвеченных клеток
        /// </summary>
        /// <param name="x">Координата клетки по оси X</param>
        /// <param name="y">Координата клетки по оси Y</param>
        /// <returns></returns>
        string ToCoord (int x, int y)
        {
            return ((char)('a' + x)).ToString() + ((char)('1' + y)).ToString();
        }

       
    }
}
