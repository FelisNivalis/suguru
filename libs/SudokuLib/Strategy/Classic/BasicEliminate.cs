using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SudokuLib.Strategy.Op;

namespace SudokuLib.Strategy.Classic
{
    public class BasicEliminate : Strategy<ClassicSudoku, BasicEliminate>
    {
        override public OpList ExecuteOnDigit(ClassicSudoku game, int row, int column, int digit)
        {
            if (digit == 0)
                return new OpList();
            var idx = Common.GetIdxFromRC(row, column);
            return new OpList {
                new OpList(
                    from r in Enumerable.Range(0, 9)
                    select new EliminateOp(r, column, digit) as OpBase
                ),

                new OpList(
                    from c in Enumerable.Range(0, 9)
                    select new EliminateOp(row, c, digit) as OpBase
                ),

                new OpList(
                    from i in Enumerable.Range(1, 9)
                    let rc = Common.GetRCFromIdx(idx.Item1, i)
                    select new EliminateOp(rc.Item1, rc.Item2, digit) as OpBase
                )
            };
        }
    }
}
