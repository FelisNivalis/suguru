using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuLib
{
    static public class Common
    {
        static public (int, int) GetIdxFromRC(int row, int column)
        {
            return (row / 3 * 3 + column / 3 + 1, row % 3 * 3 + column % 3 + 1);
        }

        static public (int, int) GetRCFromIdx(int i, int j)
        {
            i--;
            j--;
            return (i / 3 * 3 + j / 3, i % 3 * 3 + j % 3);
        }
    }
}
