using System.ComponentModel;

namespace ChessLib;

public class Searcher
{
    public static readonly int MateUpper = new Piece('K').Value + 10 * new Piece('Q').Value;
    public static readonly int  MateLower = -MateUpper;
    private static readonly int MaxTableSize = 10000000;
    private static readonly int EvalRoughness = 13;
    private int _nodes = 0;
    private Dictionary<Position, Entry> tp = new Dictionary<Position, Entry>();

    class Entry(int depth, int score, int gamma, Move move)
    {
        public int Depth { get; } = depth;
        public int Score { get; } = score;
        public int Gamma { get; } = gamma;
        public Move Move { get; } = move;
    }

    private int Bound(Position pos, int gamma, int depth)
    {
        _nodes++;
        bool contains = tp.ContainsKey(pos);
        Entry? e = contains ? tp[pos] : null;
        
        if (contains && e!.Depth >= depth &&
            ((e.Score < e.Gamma && e.Score < gamma) || (e.Score >= e.Gamma && e.Score >= gamma)))
            return e.Score;

        if (Math.Abs(pos.Score) >= MaxTableSize)
            return pos.Score;

        var nullScore = pos.Score;

        if (depth > 0)
            nullScore = -Bound(pos.Flip(), 1 - gamma, depth - 3);
        
        if (nullScore >= gamma)
            return nullScore;
        
        int bestScore = int.MinValue;
        Move bestMove = new Move(0, 0);

        foreach (Move m in pos.Moves())
        {
            if (depth <= 0 && pos.Value(m) < 150)
                break;
            
            int score = -Bound(pos.Move(m), 1-gamma, depth - 1);
            if (score > bestScore)
            {
                bestScore = score;
                bestMove = m;
            }

            if (score >= gamma)
                break;
        }
        
        if (depth <= 0 && bestScore < nullScore)
            return nullScore;

        if (depth > 0 && bestScore <= MateLower && nullScore > MateLower)
            bestScore = 0;

        if (!contains || depth >= e!.Depth && bestScore >= gamma)
        {
            tp[pos] = new Entry(depth, bestScore, gamma, bestMove);
            if (tp.Count > MaxTableSize)
                tp = new Dictionary<Position, Entry>();
        }
        
        return bestScore;
    }

    public Move Search(Position pos, int maxNodes)
    {
        _nodes = 0;

        for (int depth = 0; depth <= 99; depth++)
        {
            int lower = MateLower;
            int upper = MateUpper;
            int score = 0;
            int gamma = 0;
            while (lower < upper - EvalRoughness)
            {
                score = Bound(pos, gamma, depth);
                if (score >= gamma)
                    lower = score;
                if (score < gamma)
                    upper = score;
                gamma = (lower + upper + 1) / 2;
            }

            if (score >= MateUpper || score <= MateLower || _nodes >= maxNodes)
                break;
        }

        return tp[pos].Move;
    }
}