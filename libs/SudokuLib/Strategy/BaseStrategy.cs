using SudokuLib.Strategy.Op;

namespace SudokuLib.Strategy
{
    public abstract class Singleton<T> where T : class, new()
    {
        public static readonly T Instance = new();
    }

    public class Strategy<G, T> : Singleton<T> where G : SudokuBase where T : class, new()
    {
        virtual public OpList ExecuteOnSubgrid(G game, int row, int column)
        {
            throw new NotImplementedException();
        }

        virtual public OpList ExecuteOnDigit(G game, int row, int column, int digit)
        {
            throw new NotImplementedException();
        }

        virtual public OpList ExecuteOnBoard(G game)
        {
            throw new NotImplementedException();
        }

        virtual public OpList ExecuteOnEverySubgrid(G game)
        {
            return new OpList((
                from r in Enumerable.Range(0, 9)
                from c in Enumerable.Range(0, 9)
                select ExecuteOnSubgrid(game, r, c)
            ).SelectMany(x => x));
        }

        virtual public OpList ExecuteOnEverySubgridDigit(G game)
        {
            return new OpList((
                from r in Enumerable.Range(0, 9)
                from c in Enumerable.Range(0, 9)
                select ExecuteOnDigit(game, r, c, game.board[r, c])
            ).SelectMany(x => x));
        }
    }
}
