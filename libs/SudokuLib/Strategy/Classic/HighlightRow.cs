using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuLib.Strategy.Classic
{
    public class HighlightRow : Strategy<ClassicSudoku, HighlightRow>
    {
        override public IEnumerable<Op.OpBase> ExecuteOnSubgrid(ClassicSudoku game, int row, int column)
        {
            return from i in Enumerable.Range(0, 9) select new Op.SubgridSelectOp(row, i) as Op.OpBase;
        }

    }
}
