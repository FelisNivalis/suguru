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

        static public void DebugPrintBoard(in int[,] board)
        {
            for (int i = 0; i < 9; i++)
            {
                String s = "";
                for (int j = 0; j < 9; j++)
                    s += board[i, j].ToString();
                Debug.Print(s);
            }
        }
    }
}
