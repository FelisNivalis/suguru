using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SudokuLib.Strategy.Op;

namespace SudokuLib.Strategy.Classic
{
    public class UndoHighlightRow : Strategy<ClassicSudoku, UndoHighlightRow>
    {
        override public OpList ExecuteOnSubgrid(ClassicSudoku game, int row, int column)
        {
            return new OpList(
                from i in Enumerable.Range(0, 9)
                select new SubgridUnselectOp(row, i) as OpBase
            );
        }

        override public OpList ExecuteOnBoard(ClassicSudoku game)
        {
            return new OpList(
                from row in Enumerable.Range(0, 9)
                from column in Enumerable.Range(0, 9)
                select new SubgridUnselectOp(row, column) as OpBase
            );
        }
    }
}
