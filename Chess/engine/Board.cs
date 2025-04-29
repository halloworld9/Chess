using System.Text;
namespace ChessLib;

public class Board : ICloneable
{
    private Piece[] _pieces = new Piece[120]; // 12 * 10 чтобы не проверять вылезание за размерность доски
    public int Size => _pieces.Length;

    public Board()
    {
    }

    public Board(string fen)
    {
        var parts = fen.Split(' ');
        var rows = parts[0].Split('/');
        if (rows.Length != 8)
            throw new ArgumentException($"Invalid board size: {fen}");
        var empty = new Piece(' ');
        var dot = new Piece('.');
        for (int i = 0; i < _pieces.Length; i++)
        {
            _pieces[i] = empty;
        }

        for (int i = 0; i < 8; i++)
        {
            int index = i * 10 + 21;
            foreach (var c in rows[i])
            {
                var q = new Piece(c);
                if ('1' <= q.PieceChar && q.PieceChar <= '9')
                {
                    for (int j = 0; q.PieceChar - j >= '1'; j++)
                    {
                        _pieces[index] = dot;
                        index++;
                    }
                } else if (q.Value == 0 && q.Flip().Value == 0)
                {
                    throw new ArgumentException($"invalid piece value: {c}" );
                }
                else
                {
                    _pieces[index] = q;
                    index++;
                }
            }
            if (index % 10 != 9)
                throw new ArgumentException($"invalid row lenght: {rows[index].Length}");

            if (parts.Length > 1 && parts[1] == "b")
                _pieces = Flip()._pieces;
        }
    }

    public char[] CharBoard()
    {
        char[] c = new char[64];
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                c[row * 8 + col] = _pieces[(row+2) * 10 + (col+1)].PieceChar;
            }
        }
        return c;
    }

    public Board Flip()
    {
        var board = new Board();
        for (int i = _pieces.Length - 1; i >= 0; i--)
        {
            board._pieces[i] = _pieces[_pieces.Length - i - 1].Flip();
        }

        return board;
    }

    public Piece this[int ind]
    {
        get => _pieces[ind];
        set => _pieces[ind] = value;
    }

    public Piece this[Square ind]
    {
        get => _pieces[ind.Index];
        set => _pieces[ind.Index] = value;
    }


    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        for (int row = 2; row < 10; row++)
        {
            for (int col = 1; col < 9; col++)
            {
                sb.Append(_pieces[row * 10 + col].PieceChar);
            }

            sb.Append('\n');
        }

        return sb.ToString();
    }

    public object Clone()
    {
        var board = new Board();
        for (int i = 0; i < _pieces.Length; i++)
            board._pieces[i] = _pieces[i];

        return board;
    }
}