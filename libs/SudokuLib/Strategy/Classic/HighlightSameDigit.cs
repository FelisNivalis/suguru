using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SudokuLib.Strategy.Op;

namespace SudokuLib.Strategy.Classic
{
    public class HighlightSameDigit : Strategy<ClassicSudoku, HighlightSameDigit>
    {
        override public OpBase ExecuteOnSubgrid(ClassicSudoku game, int row, int column)
        {
            int digit = game.board[row, column];
            return new OpList(
                from i in Enumerable.Range(0, 9)
                from j in Enumerable.Range(0, 9)
                select new Op.DigitSelectOp(i, j, digit) as OpBase
            );
        }

    }
}
