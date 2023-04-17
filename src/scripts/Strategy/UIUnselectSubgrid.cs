using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SudokuLib.Strategy.Op;
using SudokuLib.Strategy;
using SudokuLib;

namespace Sudoku.Strategy.Classic
{
    public class UIUnselectSubgrid : Strategy<ClassicSudoku, UIUnselectSubgrid>
    {
        public override OpBase ExecuteOnSubgrid(ClassicSudoku game, int row, int column)
        {
            return new OpList {
                UndoHighlightSameDigit.Instance.ExecuteOnSubgrid(game, row, column),
                UndoHighlightColumn.Instance.ExecuteOnSubgrid(game, row, column),
                UndoHighlightRow.Instance.ExecuteOnSubgrid(game, row, column),
            };
        }
    }
}
