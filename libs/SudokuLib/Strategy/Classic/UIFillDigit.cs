using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SudokuLib.Strategy.Op;

namespace SudokuLib.Strategy.Classic
{
    public class UIFillDigit : Strategy<ClassicSudoku, UIFillDigit>
    {
        public override OpList ExecuteOnDigit(ClassicSudoku game, int row, int column, int digit)
        {
            return new OpList {
                FillDigit.Instance.ExecuteOnDigit(game, row, column, digit),
                BasicEliminate.Instance.ExecuteOnDigit(game, row, column, digit),
            };
        }
    }
}
