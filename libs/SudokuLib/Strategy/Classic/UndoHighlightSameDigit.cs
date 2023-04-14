using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuLib.Strategy.Classic
{
    public class UndoHighlightSameDigit : Strategy<ClassicSudoku, UndoHighlightSameDigit>
    {
        override public IEnumerable<Op.OpBase> ExecuteOnSubgrid(ClassicSudoku game, int row, int column)
        {
            int digit = game.board[row, column];
            return from i in Enumerable.Range(0, 9) from j in Enumerable.Range(0, 9) select new Op.DigitDeselectOp(i, j, digit) as Op.OpBase;
        }

        override public IEnumerable<Op.OpBase> ExecuteOnBoard(ClassicSudoku game)
        {
            return from i in Enumerable.Range(0, 9) from j in Enumerable.Range(0, 9) from digit in Enumerable.Range(1, 9) select new Op.DigitDeselectOp(i, j, digit) as Op.OpBase;
        }
    }
}
