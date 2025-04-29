using System.Text;

namespace ChessLib;

public class Square
{
    private readonly int _index;

    public Square(int index)
    {
        _index = index;
    }

    public int Index => _index;
    public static Square A1 = new(91);
    public static readonly Square H1 = new(98);
    public static readonly Square A8 = new(21);
    public static readonly Square H8 = new(28);

    public Square Flip()
    {
        return new Square(119 - Index);
    }

    public Square(string code)
    {
        if (code.Length != 2 || !('a' <= code[0] && code[0] <= 'h') || !('1' <= code[1] && code[1] <= '8') ) throw new ArgumentException("Wrong square format, should be a-h1-8");
        _index += code[0] - 'a' + 1;
        _index += ('8' - code[1] + 2) * 10;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append(" abcdefgh "[_index % 10]);
        sb.Append("  87654321  "[_index / 10]);
        return sb.ToString();
    }

    public static Square operator +(Square a, Square b)
    {
        return new Square(a.Index + b.Index);
    }
    public static Square operator +(int a, Square b)
    {
        return new Square(a + b.Index);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;
        if (obj is Square square)
            return square.Index == Index;
        return false;
    }

    public override int GetHashCode()
    {
        return Index;
    }

    public static Square operator +(Square a, int b)
    {
        return new Square(a.Index + b);
    }
}