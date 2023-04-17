using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SudokuLib.Strategy.Op;

namespace SudokuLib.Strategy.Classic
{
    public class UndoHighlightColumn : Strategy<ClassicSudoku, UndoHighlightColumn>
    {
        override public OpBase ExecuteOnSubgrid(ClassicSudoku game, int row, int column)
        {
            return new OpList(from i in Enumerable.Range(0, 9) select new SubgridUnselectOp(i, column) as OpBase);
        }

    }
}
