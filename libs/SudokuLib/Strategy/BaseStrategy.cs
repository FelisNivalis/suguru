using SudokuLib.Strategy.Op;

namespace SudokuLib.Strategy
{
    public abstract class Strategy<G, T> : BaseStrategy<G> where G : SudokuBase where T : class, new()
    {
        public static readonly T Instance = new();
    }

    public class BaseStrategy<G> where G : SudokuBase
    {
        virtual public OpBase ExecuteOnSubgrid(G game, int row, int column)
        {
            throw new NotImplementedException();
        }

        virtual public OpBase ExecuteOnDigit(G game, int row, int column, int digit)
        {
            throw new NotImplementedException();
        }

        virtual public OpBase ExecuteOnBoard(G game)
        {
            throw new NotImplementedException();
        }

        virtual public OpBase ExecuteOnEverySubgrid(G game)
        {
            return new OpList(
                from r in Enumerable.Range(0, 9)
                from c in Enumerable.Range(0, 9)
                select ExecuteOnSubgrid(game, r, c)
            );
        }

        virtual public OpBase ExecuteOnEverySubgridDigit(G game)
        {
            return new OpList(
                from r in Enumerable.Range(0, 9)
                from c in Enumerable.Range(0, 9)
                select ExecuteOnDigit(game, r, c, game.board[r, c])
            );
        }
    }

    public class SingleDigitOpStrategyBase<G> : Strategy<G, SingleDigitOpStrategyBase<G>> where G : SudokuBase { }

    public class SingleDigitOpStrategy<OP, G> : SingleDigitOpStrategyBase<G> where OP : DigitOpBase, new() where G : SudokuBase
    {
        public override OpBase ExecuteOnDigit(G game, int row, int column, int digit)
        {
            return new OP() with { Row = row, Column = column, Digit = digit };
        }
    }
}
