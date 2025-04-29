namespace ChessLib;

public class Piece
{
    private char piece;

    public Piece(char piece)
    {
        this.piece = piece;
    }

    public char PieceChar => piece;

    public int Value
    {
        get
        {
            return piece switch
            {
                'P' => 100, //пешка pawn
                'N' => 280, //конь knight
                'B' => 320, //слон bishop
                'R' => 479, //ладья rook
                'Q' => 929, //ферзь queen
                'K' => 60000, //король king
                _ => 0
            };
        }
    }

    public bool Ours => Value > 0;



    public Piece Flip()
    {
        if ('a' <= piece && piece <= 'z')
            return new Piece((char)(piece - 32)); // 32 = 'a-z' - 'A-Z' 
        else if ('A' <= piece && piece <= 'Z')
            return new Piece((char)(piece + 32));
        return this;
    }
}