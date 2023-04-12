namespace SudokuLib
{
    public abstract class SudokuBase
    {
        public int[,] board = new int[9, 9];
        public int[,] answer = new int[9, 9];
        protected int seed = -1;
        protected Random rand;

        public SudokuBase(SudokuBase other)
        {
            board = other.board.Clone() as int[,];
            answer = other.answer.Clone() as int[,];
            rand = other.rand;
            seed = other.seed;
        }

        public SudokuBase(int seed = -1)
        {
            this.seed = seed;
            rand = new Random();
        }

        public void Generate()
        {
            Clear();
            int[,]? _answer;
            Solve(out _answer, ref board, out _);
            if (_answer == null) return;
            answer = _answer;
            board = answer.Clone() as int[,];
            while (true)
            {
                int[] order = Enumerable.Range(0, 9 * 9).ToArray();
                Shuffle(ref order);
                bool flag = false;
                foreach (int idx in order)
                {
                    int x = idx / 9;
                    int y = idx % 9;
                    if (board[x, y] == 0)
                        continue;
                    board[x, y] = 0;
                    bool unique;
                    Solve(out _, ref board, out unique);
                    if (unique)
                    {
                        flag = true;
                        break;
                    }
                    else
                        board[x, y] = answer[x, y];
                }
                if (!flag) break;
            }
        }

        protected void Clear()
        {
            board = new int[9, 9];
            answer = new int[9, 9];
            rand = seed < 0 ? new Random() : new Random(seed);
        }

        protected bool Solve(out int[,]? answer, ref int[,] board, out bool unique)
        {
            answer = null;
            unique = true;
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
            if (i == 9)
            {
                answer = board.Clone() as int[,];
                return true;
            }
            // Search
            int nSolution = 0;
            foreach (int candidate in EnumerateValid(board, i, j))
            {
                board[i, j] = candidate;
                bool _unique;
                int[,]? _answer;
                if (Solve(out _answer, ref board, out _unique))
                {
                    answer = _answer;
                    nSolution++;
                    if (!_unique)
                        nSolution++;
                }
                board[i, j] = 0;
                if (nSolution > 1) break;
            }
            unique = nSolution == 1;
            return nSolution > 0;
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

        protected IEnumerable<int> EnumerateValid(int[,] board, int x, int y)
        {
            // Shuffle 1-9
            int[] order = Enumerable.Range(1, 9).ToArray();
            Shuffle(ref order);
            // Get all valid numbers
            bool[] v;
            GetValid(board, x, y, out v);
            foreach (int i in order)
            {
                if (v[i])
                    yield return i;
            }
        }

        public IEnumerable<int> EnumerateInvalid(int[,] board, int x, int y)
        {
            // Get all valid numbers
            bool[] v;
            GetValid(board, x, y, out v);
            foreach (int i in Enumerable.Range(1, 9))
                if (!v[i])
                    yield return i;
        }

        abstract protected void GetValid(in int[,] board, int x, int y, out bool[] v);
    }
}
