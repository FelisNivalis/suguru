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
        override public IEnumerable<OpBase> ExecuteOnDigit(ClassicSudoku game, int row, int column, int digit)
        {
            return new OpBase[] { new FillOp(row, column, digit) };
        }
    }
}
