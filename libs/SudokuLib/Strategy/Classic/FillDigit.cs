using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SudokuLib.Strategy.Op;

namespace SudokuLib.Strategy.Classic
{
    public class FillDigit : Strategy<ClassicSudoku, FillDigit>
    {
        override public OpList ExecuteOnDigit(ClassicSudoku game, int row, int column, int digit)
        {
            OpBase fillOp = new FillOp(row, column, digit);
            if (digit == 0) return new OpList { fillOp };

            OpBase hlOp = digit == game.answer[row, column] ? new SubgridHLDefaultOp(row, column) : new SubgridHLErrorOp(row, column);
            return new OpList { fillOp, hlOp };
        }
    }
}
