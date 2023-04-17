using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SudokuLib.Strategy.Op;

namespace Sudoku
{
    public interface DigitSelectOp : DigitOpType { }
    public interface DigitUnselectOp : DigitOpType { }
    public interface SubgridSelectOp : SubgridOpType { }
    public interface SubgridUnselectOp : SubgridOpType { }
}
