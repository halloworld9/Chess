using System.Collections;
using System.Collections.ObjectModel;
using ChessLib;

namespace ChessLib;

public class Position
{
    private const int N = -10;
    private const int E = 1;
    private const int S = 10;
    private const int W = -1;
    private const int A1 = 91;
    private const int H1 = 98;
    private const int A8 = 21;
    private const int H8 = 28;
    private readonly Board _board;
    public char[] Board => _board.CharBoard();
    private int _score;
    public int Score => _score;
    private readonly bool[] _wc;//white castle белая рокировка
    public ReadOnlyCollection<bool> Wc => _wc.AsReadOnly();
    private readonly bool[] _bc; //black castle черная рокировка
    public ReadOnlyCollection<bool> Bc => _bc.AsReadOnly();
    private Square _ep; //клетка для взятия на проходе
    public Square Ep => _ep;
    private Square _kp; //клетка где короля могут взять во время рокировки
    public Square Kp => _kp;
    Dictionary<char, int[]> pst = new Dictionary<char, int[]>
        {
            {
                'P',
                [
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                    0, 178, 183, 186, 173, 202, 182, 185, 190, 0,
                    0, 107, 129, 121, 144, 140, 131, 144, 107, 0,
                    0, 83, 116, 98, 115, 114, 0, 115, 87, 0,
                    0, 74, 103, 110, 109, 106, 101, 0, 77, 0,
                    0, 78, 109, 105, 89, 90, 98, 103, 81, 0,
                    0, 69, 108, 93, 63, 64, 86, 103, 69, 0,
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                ]
            },
            {
                'N',
                [
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                    0, 214, 227, 205, 205, 270, 225, 222, 210, 0,
                    0, 277, 274, 380, 244, 284, 342, 276, 266, 0,
                    0, 290, 347, 281, 354, 353, 307, 342, 278, 0,
                    0, 304, 304, 325, 317, 313, 321, 305, 297, 0,
                    0, 279, 285, 311, 301, 302, 315, 282, 0, 0,
                    0, 262, 290, 293, 302, 298, 295, 291, 266, 0,
                    0, 257, 265, 282, 0, 282, 0, 257, 260, 0,
                    0, 206, 257, 254, 256, 261, 245, 258, 211, 0,
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0
                ]
            },
            {
                'B',
                [
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                    0, 261, 242, 238, 244, 297, 213, 283, 270, 0,
                    0, 309, 340, 355, 278, 281, 351, 322, 298, 0,
                    0, 311, 359, 288, 361, 372, 310, 348, 306, 0,
                    0, 345, 337, 340, 354, 346, 345, 335, 330, 0,
                    0, 333, 330, 337, 343, 337, 336, 0, 327, 0,
                    0, 334, 345, 344, 335, 328, 345, 340, 335, 0,
                    0, 339, 340, 331, 326, 327, 326, 340, 336, 0,
                    0, 313, 322, 305, 308, 306, 305, 310, 310, 0,
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0
                ]
            },
            {
                'R',
                [
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                    0, 514, 508, 512, 483, 516, 512, 535, 529, 0,
                    0, 534, 508, 535, 546, 534, 541, 513, 539, 0,
                    0, 498, 514, 507, 512, 524, 506, 504, 494, 0,
                    0, 0, 484, 495, 492, 497, 475, 470, 473, 0,
                    0, 451, 444, 463, 458, 466, 450, 433, 449, 0,
                    0, 437, 451, 437, 454, 454, 444, 453, 433, 0,
                    0, 426, 441, 448, 453, 450, 436, 435, 426, 0,
                    0, 449, 455, 461, 484, 477, 461, 448, 447, 0,
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                ]
            },
            {
                'Q',
                [
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                    0, 935, 930, 921, 825, 998, 953, 1017, 955, 0,
                    0, 943, 961, 989, 919, 949, 1005, 986, 953, 0,
                    0, 927, 972, 961, 989, 1001, 992, 972, 931, 0,
                    0, 930, 913, 951, 946, 954, 949, 916, 923, 0,
                    0, 915, 914, 927, 924, 928, 919, 909, 907, 0,
                    0, 899, 923, 916, 918, 913, 918, 913, 902, 0,
                    0, 893, 911, 0, 910, 914, 914, 908, 891, 0,
                    0, 890, 899, 898, 916, 898, 893, 895, 887, 0,
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0
                ]
            },
            {
                'K',
                [
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                    0, 60004, 60054, 60047, 59901, 59901, 60060, 60083, 59938, 0,
                    0, 59968, 60010, 60055, 60056, 60056, 60055, 60010, 60003, 0,
                    0, 59938, 60012, 59943, 60044, 59933, 60028, 60037, 59969, 0,
                    0, 59945, 60050, 60011, 59996, 59981, 60013, 0, 59951, 0,
                    0, 59945, 59957, 59948, 59972, 59949, 59953, 59992, 59950, 0,
                    0, 59953, 59958, 59957, 59921, 59936, 59968, 59971, 59968, 0,
                    0, 59996, 60003, 59986, 59950, 59943, 59982, 60013, 60004, 0,
                    0, 60017, 60030, 59997, 59986, 60006, 59999, 60040, 60018, 0,
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                ]
            },
            { ' ', new int[120] },
            { '.', new int[120] }
        };
    public Position(Board board, int score, bool[] wc, bool[] bc, Square ep, Square kp)
    {
        _board = board;
        _score = score;
        _wc = wc;
        _bc = bc;
        _ep = ep;
        _kp = kp;
    }

    public Position(Board board)
    {
        this._board = board;
        this._score = 0;
        for (int i = 0; i < board.Size; i++)
        {
            var c = board[i];
            if (c.Value > 0){
                _score += pst[c.PieceChar][i];
          }
        }
        var b = board.Flip();
        for (int i = 0; i < board.Size; i++)
        {
            var c = b[i];
            if (c.Value > 0)
            {
                _score -= pst[c.PieceChar][i];
            }
        }
        this._wc = [true, true];
        this._bc = [true, true];
        this._ep = new Square(0);
        this._kp = _ep;
    }

    public List<Move> Moves()
    {
        var moves = new List<Move>();

        var directions = new Dictionary<char, int[]>
        {
            { 'P', [N, N + N, N + W, N + E] },
            { 'N', [N + N + E, E + N + E, E + S + E, S + S + E, S + S + W, W + S + W, W + N + W, N + N + W] },
            { 'B', [N + E, S + E, S + W, N + W] },
            { 'R', [N, E, S, W] },
            { 'Q', [N, E, S, W, N + E, S + E, S + W, N + W] },
            { 'K', [N, E, S, W, N + E, S + E, S + W, N + W] }
        };

        for (int i = 0; i < _board.Size; i++)
        {
            var p = _board[i];
            if (!p.Ours) continue;


            foreach (var d in directions[p.PieceChar])
            {
                for (int j = i + d;; j += d)
                {
                    var q = _board[j];
                    if (q.PieceChar == ' ' || (q.PieceChar != '.' && q.Ours))
                        break;

                    if (p.PieceChar == 'P')
                    {
                        if ((d == N || d == N + N) && q.PieceChar != '.') break; // проверка выхода за поле
                        if (d == N + N && (i < A1 + N || _board[i + N].PieceChar != '.')) break; // 
                        if ((d == N + W || d == N + E) && q.PieceChar == '.' && j != _ep.Index && j != _kp.Index &&
                            j != _kp.Index - 1 && j != _kp.Index + 1) break; //проверка взятия на проходе
                    }

                    moves.Add(new Move(i, j));
                    if (p.PieceChar == 'P' || p.PieceChar == 'N' || p.PieceChar == 'K' ||
                        (q.PieceChar != ' ' && q.PieceChar != '.' && !q.Ours))
                        break;

                    if (i == A1 && _board[j + E].PieceChar == 'K' && _wc[0])
                        moves.Add(new Move(j + E, j + W));

                    if (i == H1 && _board[j + W].PieceChar == 'K' && _wc[1])
                        moves.Add(new Move(j + W, j + E));
                }
            }
        }

        return moves;
    }


    public List<Move> GetLegalMoves()
    {
        var m = new List<Move>();
        var moves = Moves();
        foreach (Move move in moves)
        {
            var newPos = Move(move);
            bool f = true;
            foreach (Move move1 in newPos.Moves())
            {
                if (newPos.Board[move1.ToIndex] == 'k'){
                    f = false;
                    break;
                }
            }
            if (f) m.Add(move);
        }
        return m;
    }

    public Position Move(Move m)
    {
        var newPos = new Position((_board.Clone() as Board)!, _score + Value(m), [_wc[0], _wc[1]], [_bc[0], _bc[1]],
            new Square(0), new Square(0));
        int i = m.From.Index;
        int j = m.To.Index;
        Piece p = _board[i];
        newPos._board[j] = _board[i];
        newPos._board[i] = new Piece('.');
        if (i == A1)
            newPos._wc[0] = false;
        if (i == H1)
            newPos._wc[1] = false;
        if (j == A8)
            newPos._bc[1] = false;
        if (j == H8)
            newPos._bc[0] = false;

        if (p.PieceChar == 'K')
        {
            newPos._wc[0] = false;
            newPos._wc[1] = false;
            if (Math.Abs(j - i) == 2)
            {
                if (j > i)
                    newPos._board[H1] = new Piece('.');
                else
                    newPos._board[A1] = new Piece('.');
                newPos._board[(i + j) / 2] = new Piece('R');
            }
        }

        if (p.PieceChar == 'P')
        {
            if (A8 <= j && j <= H8)
                newPos._board[j] = new Piece('Q');

            if (j - i == 2 * N)
                newPos._ep = new Square(i + N);

            if (j == _ep.Index)
                newPos._board[j + S] = new Piece('.');
        }

        return newPos.Flip();
    }

    public int Value(Move m)
    {
        int score = _score;
        

        Square i = m.From;
        Square j = m.To;
        Piece p = _board[i];
        Piece q = _board[j];
        score = pst[p.PieceChar][j.Index] - pst[p.PieceChar][i.Index];
        if (q.PieceChar != '.' && q.PieceChar != ' ' && !q.Ours)
            score += pst[q.Flip().PieceChar][j.Flip().Index];

        //Проверка направление рокировки
        if (Math.Abs(j.Index - _kp.Index) < 2)
            score += pst['K'][j.Flip().Index];

        //рокировка
        if (p.PieceChar == 'K' && Math.Abs(j.Index - i.Index) == 2)
        {
            score += pst['R'][(i.Index + j.Index) / 2];
            if (j.Index > i.Index)
                score -= pst['R'][A1];
            else
                score -= pst['R'][H1];
        }

        if (p.PieceChar == 'P')
        {
            if (A8 <= j.Index && j.Index <= H8)
            {
                score += pst['Q'][j.Index] - pst['P'][j.Index];
            }

            if (j.Index == _ep.Index)
                score += pst['P'][new Square(j.Index + S).Flip().Index];
        }

        return score;
    }


    public Position Flip()
    {
        return new Position(
            _board.Flip(),
            -_score,
            [_bc[0], _bc[1]],
            [_wc[0], _wc[1]],
            _ep.Flip(),
            _kp.Flip()
        );
    }

    public override string ToString()
    {
        return _board + $"\nScore: {_score}";
    }

    public override int GetHashCode()
    {
        return ToString().GetHashCode();
    }
}