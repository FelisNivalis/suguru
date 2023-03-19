using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku.Sudoku
{
    public partial class ClassicSudoku : SudokuBase
    {
        public ClassicSudoku(int seed = -1) : base(seed) { }
        public ClassicSudoku(ClassicSudoku other): base(other) { }
        override protected void GetValid(int x, int y, out bool[] v)
        {
            v = new bool[9 + 1];
            int gx = x / 3 * 3, gy = y / 3 * 3;
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    v[board[gx + i, gy + j]] = true;
            for (int i = 0; i < 9; i++)
                v[board[i, y]] = v[board[x, i]] = true;
        }
    }
}
