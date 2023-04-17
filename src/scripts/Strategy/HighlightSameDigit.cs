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
    public class HighlightSameDigit : Strategy<ClassicSudoku, HighlightSameDigit>
    {
        override public OpBase ExecuteOnSubgrid(ClassicSudoku game, int row, int column)
        {
            int digit = game.board[row, column];
            return new OpList(
                from i in Enumerable.Range(0, 9)
                from j in Enumerable.Range(0, 9)
                select new DigitOp<DigitSelectOp>(i, j, digit) as OpBase
            );
        }

    }
}
