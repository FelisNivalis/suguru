using System.Collections;
using System.Data.Common;
using System.Diagnostics;
using SudokuLib.Strategy.Op;

namespace SudokuLib.Strategy
{
    public abstract class BaseStrategy<G> where G : SudokuBase
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

    public abstract class Strategy<G, T> : BaseStrategy<G> where G : SudokuBase where T : class, new()
    {
        public static readonly T Instance = new();
    }

    public class SingleDigitOpStrategy<OP, G> : Strategy<G, SingleDigitOpStrategy<OP, G>> where OP : DigitOpBase, new() where G : SudokuBase
    {
        public override OpBase ExecuteOnDigit(G game, int row, int column, int digit)
        {
            return new OP() with { Row = row, Column = column, Digit = digit };
        }
    }

    public class StrategyList<G> : BaseStrategy<G>, IEnumerable<BaseStrategy<G>> where G : SudokuBase
    {
        ICollection<BaseStrategy<G>> strategyList;
        public StrategyList()
        {
            strategyList = new List<BaseStrategy<G>>();
        }
        public StrategyList(ICollection<BaseStrategy<G>> strategyList)
        {
            this.strategyList = strategyList;
        }
        public StrategyList(IEnumerable<BaseStrategy<G>> strategyList)
        {
            this.strategyList = strategyList.ToList();
        }
        override public OpBase ExecuteOnSubgrid(G game, int row, int column)
        {
            return new OpList(strategyList.Select(strategy => strategy.ExecuteOnSubgrid(game, row, column)));
        }

        override public OpBase ExecuteOnDigit(G game, int row, int column, int digit)
        {
            return new OpList(strategyList.Select(strategy => strategy.ExecuteOnDigit(game, row, column, digit)));
        }

        override public OpBase ExecuteOnBoard(G game)
        {
            return new OpList(strategyList.Select(strategy => strategy.ExecuteOnBoard(game)));
        }

        public IEnumerator<BaseStrategy<G>> GetEnumerator()
        {
            return strategyList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(BaseStrategy<G> strategy)
        {
            strategyList.Add(strategy);
        }
    }
}
