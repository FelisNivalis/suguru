using SudokuLib.Strategy.Op;

namespace SudokuLib.Strategy.Classic
{
    public class UIUnfillSubgrid : Strategy<ClassicSudoku, UIUnfillSubgrid>
    {
        override public OpBase ExecuteOnSubgrid(ClassicSudoku game, int row, int column)
        {
            if (game.init_board[row, column] != 0) return new OpList();
            int digit = game.board[row, column];
            if (digit == 0) throw new ArgumentException(String.Format("The subgrid (row={0}, col={1}) is not filled.", row, column));

            var idx = Common.GetIdxFromRC(row, column);
            return new OpList {
                FillDigit.Instance.ExecuteOnDigit(game, row, column, 0),

                new OpList(
                    from i in Enumerable.Range(0, 9)
                    select new UnEliminateOp(row, i, digit) as OpBase
                ),

                new OpList(
                    from i in Enumerable.Range(0, 9)
                    select new UnEliminateOp(i, column, digit) as OpBase
                ),

                new OpList(
                    from i in Enumerable.Range(1, 9)
                    let rc = Common.GetRCFromIdx(idx.Item1, i)
                    select new UnEliminateOp(rc.Item1, rc.Item2, digit) as OpBase
                ),

                new OpList(
                    from r in Enumerable.Range(0, 9)
                    from c in Enumerable.Range(0, 9)
                    where game.board[r, c] == digit
                    select BasicEliminate.Instance.ExecuteOnDigit(game, r, c, digit)
                )
            };
        }
    }
}
