using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using ChessLib;

namespace Chess
{

    public partial class MainWindow : Window
    {
        private double cellWidth;
        private double cellHeight;
        Searcher searcher;
        private int lastClick = -1;
        private char[] board;
        private Rectangle[] cells;
        private Position pos;
        private readonly Color whiteCell = Color.FromRgb(240, 217, 181);
        private readonly Color blackCell = Color.FromRgb(181, 136, 99);
        private List<Move> moves;
        private List<Ellipse> possibleMoves = new List<Ellipse>();
        private bool whiteTurn = true;
        private bool playerWhite = true;
        private bool withBot = true;
        public MainWindow()
        {
            InitializeComponent();

        }

        private void InitBoard(string fen, bool black)
        {
            cellWidth = Field.Width / 8;
            cellHeight = Field.Height / 8;
            cells = new Rectangle[64];
            for (int i = 0; i < 64; i++)
            {
                var cell = new Rectangle();
                cell.Width = cellWidth;
                cell.Height = cellHeight;
                cell.Fill = new SolidColorBrush(((i / 8 + i % 8) % 2) == 0 ? whiteCell : blackCell);
                cell.Margin = new Thickness(i % 8 * cellWidth, i / 8 * cellHeight, 0, 0);
                cell.MouseLeftButtonUp += CellClick(i);
                cell.MouseRightButtonUp += MarkCell(i);
                Field.Children.Add(cell);
                Rectangle clickedCell = new Rectangle();
                clickedCell.Width = cellWidth;
                clickedCell.Height = cellHeight;
                clickedCell.IsHitTestVisible = false;
                clickedCell.Fill = new SolidColorBrush(Color.FromArgb(104, 155, 199, 0));
                clickedCell.Margin = new Thickness(i % 8 * cellWidth, i / 8 * cellHeight, 0, 0);
                clickedCell.Visibility = Visibility.Hidden;
                Canvas.SetZIndex(clickedCell, 1);
                Canvas.SetZIndex(cell, 0);
                cells[i] = clickedCell;
                Field.Children.Add(clickedCell);
            }
            pos = new Position(new Board(fen));
            this.searcher = new Searcher();

            board = pos.Board;
            DrawPieces(board);
            if (black)
            {
                var m = searcher.Search(pos, 10000);
                MakeMove(m);
            }
            moves = pos.GetLegalMoves();
        }
        private static Action EmptyDelegate = delegate () { };
        private MouseButtonEventHandler CellClick(int i)
        {
            return (sender, e) =>
            {
                int j = i;
                if (!whiteTurn)
                    j = 63 - j;
                if (withBot && whiteTurn != playerWhite)
                    return;
                foreach (var circle in possibleMoves)
                {
                    Field.Children.Remove(circle);
                }
                possibleMoves.Clear();
                if (lastClick == j)
                {
                    lastClick = -1;
                }
                else if (char.IsLetter(board[j]) && char.IsUpper(board[j]))
                {
                    lastClick = j;
                    foreach (Move m in moves)
                    {
                        if (m.FromIndex == lastClick)
                        {
                            var newPos = pos.Move(m);
                            var b = newPos.Board;
                            var el = new Ellipse();
                            el.Width = cellWidth / 2;
                            el.Height = cellHeight / 2;
                            int mIndex = whiteTurn ? m.ToIndex : 63 - m.ToIndex;
                            el.Margin = new Thickness(mIndex % 8 * cellWidth + cellWidth / 4, mIndex / 8 * cellHeight + cellHeight / 4, 0, 0);
                            el.IsHitTestVisible = false;
                            el.Fill = new SolidColorBrush(Colors.Black);
                            possibleMoves.Add(el);
                            Field.Children.Add(el);
                        }
                    }
                }
                else
                {
                    foreach (var move in moves)
                    {
                        if (move.FromIndex == lastClick && move.ToIndex == j)
                        {

                            MakeMove(move);
                            if (withBot)
                            {
                                MakeMove(searcher.Search(pos, 10000));
                            }
                            lastClick = -1;
                            return;
                        }
                    }
                    lastClick = j;
                }

            };
        }

        private void MakeMove(Move move)
        {
            pos = pos.Move(move);
            moves = pos.GetLegalMoves();
            board = pos.Board;
            if (!whiteTurn)
                DrawPieces(pos.Board);
            else
                DrawPieces(pos.Flip().Board);
            if (moves.Count == 0)
            {
                int ind = -1;
                for (int i = 0; i < 64; i++)
                {
                    if (board[i] == 'K')
                    {
                        ind = 63 - i;
                        break;
                    }
                }
                var moves = pos.Flip().Moves();
                foreach (var m in moves)
                {
                    if (m.ToIndex == ind)
                    {
                        MessageBox.Show(whiteTurn ? "Белые победили" : "Черные победили");
                        return;
                    }
                }
                MessageBox.Show("НИЧЬЯ");
            }


            whiteTurn = !whiteTurn;

        }

        private MouseButtonEventHandler MarkCell(int i)
        {
            return (sender, e) =>
            {
                if (cells[i].Visibility == Visibility.Visible)
                    cells[i].Visibility = Visibility.Hidden;
                else
                    cells[i].Visibility = Visibility.Visible;
            };
        }


        private string GetFigureImage(char c)
        {
            string str = char.IsUpper(c) ? "W" : "B";
            c = char.ToLower(c);
            if (c == 'k')
                return str + "King.png";
            if (c == 'n')
                return str + "Knight.png";
            if (c == 'r')
                return str + "Rook.png";
            if (c == 'q')
                return str + "Queen.png";
            if (c == 'p')
                return str + "Pawn.png";
            if (c == 'b')
                return str + "Bishop.png";

            return "";
        }


        public void ClearBoard()
        {
            for (int i = 0; i < Field.Children.Count; i++)
                if (Field.Children[i] != null && Field.Children[i] is Image)
                {
                    Field.Children.RemoveAt(i);
                    i--;
                }
        }

        public void DrawPieces(char[] pos)
        {
            ClearBoard();
            for (int i = 0; i < pos.Length; i++)
            {
                if (!char.IsLetter(pos[i])) continue;
                Image im = new Image();
                BitmapImage mipMap = new BitmapImage();
                mipMap.BeginInit();
                mipMap.UriSource = new Uri("pack://application:,,,/images/" + GetFigureImage(pos[i]));
                mipMap.EndInit();
                im.Source = mipMap;
                im.Height = cellHeight;
                im.Width = cellWidth;
                im.Margin = new Thickness(i % 8 * cellWidth, i / 8 * cellHeight, 0, 0);
                im.IsHitTestVisible = false;
                Canvas.SetZIndex(im, 2);
                Field.Children.Add(im);
            }
            ForceUIToUpdate();
        }

        public static void ForceUIToUpdate()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Render, new DispatcherOperationCallback(delegate (object parameter)
            {
                frame.Continue = false;
                return null;
            }), null);
            Dispatcher.PushFrame(frame);
        }


        private void ChooseColor(object sender, RoutedEventArgs e)
        {
            var a = (Button)sender;
            bool black = a.Content.ToString() == "Черный";
            playerWhite = !black;
            Field.Children.Remove(Init);
            InitBoard("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR", black);
        }

        private void PVP(object sender, RoutedEventArgs e)
        {
            this.withBot = false;
            Field.Children.Remove(Init);
            InitBoard("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR", false);
        }
    }
}