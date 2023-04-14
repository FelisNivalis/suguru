using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuLib.Strategy.Classic
{
    public class UndoHighlightRow : Strategy<ClassicSudoku, UndoHighlightRow>
    {
        override public IEnumerable<Op.OpBase> ExecuteOnSubgrid(ClassicSudoku game, int row, int column)
        {
            return from i in Enumerable.Range(0, 9) select new Op.SubgridUnselectOp(row, i) as Op.OpBase;
        }

        override public IEnumerable<Op.OpBase> ExecuteOnBoard(ClassicSudoku game)
        {
            return from row in Enumerable.Range(0, 9) from column in Enumerable.Range(0, 9) select new Op.SubgridUnselectOp(row, column) as Op.OpBase;
        }
    }
}
