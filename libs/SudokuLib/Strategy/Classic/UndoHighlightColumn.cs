using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuLib.Strategy.Classic
{
    public class UndoHighlightColumn : Strategy<ClassicSudoku, UndoHighlightColumn>
    {
        override public IEnumerable<Op.OpBase> ExecuteOnSubgrid(ClassicSudoku game, int row, int column)
        {
            return from i in Enumerable.Range(0, 9) select new Op.SubgridDeselectOp(i, column) as Op.OpBase;
        }

    }
}
