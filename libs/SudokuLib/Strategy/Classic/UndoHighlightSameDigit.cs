using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SudokuLib.Strategy.Op;

namespace SudokuLib.Strategy.Classic
{
    public class UndoHighlightSameDigit : Strategy<ClassicSudoku, UndoHighlightSameDigit>
    {
        override public OpBase ExecuteOnSubgrid(ClassicSudoku game, int row, int column)
        {
            int digit = game.board[row, column];
            return new OpList(
                from i in Enumerable.Range(0, 9)
                from j in Enumerable.Range(0, 9)
                select new DigitUnselectOp(i, j, digit) as OpBase
            );
        }

        override public OpBase ExecuteOnBoard(ClassicSudoku game)
        {
            return new OpList(
                from i in Enumerable.Range(0, 9)
                from j in Enumerable.Range(0, 9)
                from digit in Enumerable.Range(1, 9)
                select new DigitUnselectOp(i, j, digit) as OpBase
            );
        }
    }
}
