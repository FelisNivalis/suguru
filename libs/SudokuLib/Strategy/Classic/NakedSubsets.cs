using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SudokuLib.Strategy.Op;

namespace SudokuLib.Strategy.Classic
{
    static public class NakedSubsets
    {
        static public OpBase ExecuteOnBoard(ClassicSudoku game, int[] ds)
        {
            return new OpList(
                from i in Enumerable.Range(0, 9)
                from lst in new[] { from r in Enumerable.Range(0, 9)
                                    where game.board[r, i] != 0 || !game.candidates.CheckAtMost(r, i, ds)
                                    select (r, i),
                                    from c in Enumerable.Range(0, 9)
                                    where game.board[i, c] != 0 || !game.candidates.CheckAtMost(i, c, ds)
                                    select (i, c),
                                    from j in Enumerable.Range(1, 9)
                                    let rc = Common.GetRCFromIdx(i + 1, j)
                                    where game.board[rc.Item1, rc.Item2] != 0 || !game.candidates.CheckAtMost(rc.Item1, rc.Item2, ds)
                                    select rc }
                where lst.Count() == 9 - ds.Length
                select new OpList(
                    from rc in lst
                    from d in ds
                    select new DigitOp<EliminateOp>(rc.Item1, rc.Item2, d)
                ));
        }
    }
    public class NakedPair : Strategy<ClassicSudoku, NakedPair>
    {
        public override OpBase ExecuteOnBoard(ClassicSudoku game)
        {
            return new OpList(
                from d1 in Enumerable.Range(1, 8)
                from d2 in Enumerable.Range(d1 + 1, 9 - d1)
                select NakedSubsets.ExecuteOnBoard(game, new[] { d1, d2 })
                );
        }
    }
    public class NakedTuple : Strategy<ClassicSudoku, NakedTuple>
    {
        public override OpBase ExecuteOnBoard(ClassicSudoku game)
        {
            return new OpList(
                from d1 in Enumerable.Range(1, 7)
                from d2 in Enumerable.Range(d1 + 1, 8 - d1)
                from d3 in Enumerable.Range(d2 + 1, 9 - d2)
                select NakedSubsets.ExecuteOnBoard(game, new[] { d1, d2, d3 })
                );
        }
    }
    public class NakedQuadruple : Strategy<ClassicSudoku, NakedQuadruple>
    {
        public override OpBase ExecuteOnBoard(ClassicSudoku game)
        {
            return new OpList(
                from d1 in Enumerable.Range(1, 6)
                from d2 in Enumerable.Range(d1 + 1, 7 - d1)
                from d3 in Enumerable.Range(d2 + 1, 8 - d2)
                from d4 in Enumerable.Range(d3 + 1, 9 - d3)
                select NakedSubsets.ExecuteOnBoard(game, new[] { d1, d2, d3, d4 })
                );
        }
    }
}
