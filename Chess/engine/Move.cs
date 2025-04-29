namespace ChessLib;

public class Move
{
    private readonly Square _from;
    private readonly Square _to;
    public Move(int from, int to) : this(new Square(from), new Square(to))
    {
    }

    public Move(string from, string to)
    {
        _from = new Square(from);
        _to = new Square(to);
    }

    public Move(Square from, Square to)
    {
        _from = from;
        _to = to;
    }

    public Square From => _from;
    public Square To => _to;
    public int FromIndex => (_from.Index / 10 - 2) * 8 + (_from.Index % 10 - 1);
    public int ToIndex => (_to.Index / 10 - 2) * 8 + (_to.Index % 10 - 1);



    public override string ToString()
    {
        return _from.ToString() + _to.ToString();
    }
}