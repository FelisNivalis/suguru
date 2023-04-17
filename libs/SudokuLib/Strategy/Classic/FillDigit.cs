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
        public override OpBase ExecuteOnDigit(ClassicSudoku game, int row, int column, int digit)
        {
            return new OpList {
                new DigitOp<FillOp>(row, column, digit),
                BasicEliminate.Instance.ExecuteOnDigit(game, row, column, digit),
            };
        }
    }
}
