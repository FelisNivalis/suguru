using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku.Sudoku
{
    public abstract class SudokuBase
    {
        public int[,] board = new int[9, 9];
        public bool generated = false;
        protected int nFilled = 0;
        protected int seed = -1;
        protected Random rand;

        public SudokuBase(SudokuBase other)
        {
            this.board = other.board.Clone() as int[,];
            this.rand = other.rand;
            this.seed = other.seed;
            this.generated = other.generated;
            this.nFilled = other.nFilled;
        }

        public SudokuBase(int seed = -1)
        {
            this.seed = seed;
        }

        public void Generate()
        {
            Clear();
            if (Solve(out _))
            {
                generated = true;
                while (RemoveOne()) ;
            }
        }

        public bool RemoveOne()
        {
            int[] order = new int[9 * 9];
            for (int i = 0; i < 9 * 9; i++)
                order[i] = i;
            Shuffle(ref order);
            for (int i = 0; i < 9 * 9; i++)
            {
                int x = order[i] / 9, y = order[i] % 9;
                if (board[x, y] == 0)
                    continue;
                SudokuBase board2 = Activator.CreateInstance(this.GetType(), new object[] { this }) as SudokuBase;
                board2.board[x, y] = 0;
                board2.nFilled--;
                bool unique;
                board2.Solve(out unique);
                if (unique)
                {
                    board[x, y] = 0;
                    nFilled--;
                    return true;
                }
            }
            return false;
        }

        protected void Clear()
        {
            board = new int[9, 9];
            nFilled = 0;
            generated = false;
            rand = seed < 0 ? new Random() : new Random(seed);
        }

        protected bool Solve(out bool unique)
        {
            unique = true;
            int solution = 0;
            if (nFilled == 9 * 9)
                return true;
            // Find the first unfilled grid.
            int i, j = 0;
            for (i = 0; i < 9; i++)
            {
                for (j = 0; j < 9; j++)
                    if (board[i, j] == 0)
                        break;
                if (j < 9 && board[i, j] == 0)
                    break;
            }
            // Shuffle 1-9
            int[] order = new int[9];
            for (int idx = 0; idx < order.Length; idx++)
                order[idx] = idx + 1;
            Shuffle(ref order);
            // Get all valid numbers
            bool[] v;
            GetValid(i, j, out v);
            for (int idx = 0; idx < order.Length; idx++)
            {
                if (v[order[idx]])
                    continue;
                board[i, j] = order[idx];
                nFilled++;
                bool recur_unique;
                if (Solve(out recur_unique))
                {
                    if (!recur_unique || solution != 0)
                    {
                        if (solution != 0)
                            board[i, j] = solution;
                        unique = false;
                        return true;
                    }
                    solution = order[idx];
                }
                board[i, j] = 0;
                nFilled--;
            }
            if (solution != 0)
            {
                nFilled++;
                board[i, j] = solution;
                return true;
            }
            else
            {
                return false;
            }
        }

        protected void Shuffle(ref int[] l)
        {
            for (int i = 0; i < l.Length; i++)
            {
                int j = rand.Next(i, l.Length);
                int tmp = l[i];
                l[i] = l[j];
                l[j] = tmp;
            }
        }

        abstract protected void GetValid(int x, int y, out bool[] v);
    }
}
