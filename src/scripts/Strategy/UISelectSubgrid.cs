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
    public class UISelectSubgrid : Strategy<ClassicSudoku, UISelectSubgrid>
    {
        public override OpBase ExecuteOnSubgrid(ClassicSudoku game, int row, int column)
        {
            return new OpList {
                HighlightSameDigit.Instance.ExecuteOnSubgrid(game, row, column),
                HighlightColumn.Instance.ExecuteOnSubgrid(game, row, column),
                HighlightRow.Instance.ExecuteOnSubgrid(game, row, column),
            };
        }
    }
}
