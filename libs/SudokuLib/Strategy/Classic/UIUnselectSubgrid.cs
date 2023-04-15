using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SudokuLib.Strategy.Op;

namespace SudokuLib.Strategy.Classic
{
    public class UIUnselectSubgrid : Strategy<ClassicSudoku, UIUnselectSubgrid>
    {
        public override OpList ExecuteOnSubgrid(ClassicSudoku game, int row, int column)
        {
            return new OpList {
                UndoHighlightSameDigit.Instance.ExecuteOnSubgrid(game, row, column),
                UndoHighlightColumn.Instance.ExecuteOnSubgrid(game, row, column),
                UndoHighlightRow.Instance.ExecuteOnSubgrid(game, row, column),
            };
        }
    }
}
