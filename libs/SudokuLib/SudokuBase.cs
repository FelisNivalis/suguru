using System.Diagnostics;

namespace SudokuLib
{
    public abstract class SudokuBase
    {
        public int[,] board = new int[9, 9];
        public int[,] init_board = new int[9, 9];
        public int[,] answer = new int[9, 9];
        public bool[,,] candidates = new bool[9, 9, 10];
        protected int seed = -1;
        protected Random rand;
        private const int MAX_TRIES = 1000000;

        public SudokuBase(SudokuBase other)
        {
            board = other.board.Clone() as int[,];
            init_board = other.init_board.Clone() as int[,];
            answer = other.answer.Clone() as int[,];
            candidates = other.candidates.Clone() as bool[,,];
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
            while (!Solve(out _answer, ref board, out _));
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
                    if (Solve(out _, ref board, out unique) && unique)
                    {
                        flag = true;
                        break;
                    }
                    else
                        board[x, y] = answer[x, y];
                }
                if (!flag) break;
            }
            init_board = board.Clone() as int[,];
        }

        protected void Clear()
        {
            board = new int[9, 9];
            answer = new int[9, 9];
            //rand = seed < 0 ? new Random() : new Random(seed);
        }

        protected bool Solve(out int[,]? answer, ref int[,] board, out bool unique)
        {
            var startTime = DateTime.Now;
            int nTries = 0;
            int[,,] clues = new int[3, 10, 10];
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                {
                    int candidate = board[i, j];
                    if (candidate == 0)
                        continue;
                    var idx = Common.GetIdxFromRC(i, j);
                    clues[0, i, candidate]++;
                    clues[1, j, candidate]++;
                    clues[2, idx.Item1, candidate]++;
                }
            /*for (int i = 0; i < 9; i++)
            {
                String s = "";
                for (int j = 0; j < 9; j++)
                {
                    s += board[i, j].ToString();
                }
                Debug.Print(s);
            }*/
            answer = board.Clone() as int[,];
            int level = 0;
            while (level < 9 * 9 && board[level / 9, level % 9] > 0) level++;
            int nSolution = 0;
            LinkedList<int[]> candidatesList = new();
            candidatesList.AddLast(Enumerable.Range(0, 10).ToArray());
            while (nTries++ < MAX_TRIES)
            {
                int r = level / 9;
                int c = level % 9;
                var idx = Common.GetIdxFromRC(r, c);
                var candidates = candidatesList.Last();
                if (candidates[0] > 0)
                {
                    int candidate = candidates[candidates[0]];
                    clues[0, r, candidate]--;
                    clues[1, c, candidate]--;
                    clues[2, idx.Item1, candidate]--;
                }
                while (candidates[0]++ < 9)
                {
                    int cur = candidates[0];
                    int rIdx = rand.Next(candidates[0], 10);
                    int candidate = candidates[rIdx];
                    candidates[rIdx] = candidates[cur];
                    candidates[cur] = candidate;
                    if (clues[0, r, candidate] == 0 && clues[1, c, candidate] == 0 && clues[2, idx.Item1, candidate] == 0)
                        break;
                }
                
                if (candidates[0] <= 9)
                {
                    int candidate = candidates[candidates[0]];
                    answer[r, c] = candidate;
                    clues[0, r, candidate]++;
                    clues[1, c, candidate]++;
                    clues[2, idx.Item1, candidate]++;
                    //Debug.Print(String.Format("Search level={0}, candidateIdx={1}", level, candidates[0]));
                    while (++level < 9 * 9 && board[level / 9, level % 9] > 0) ;
                    candidatesList.AddLast(Enumerable.Range(0, 10).ToArray());
                    if (level == 9 * 9)
                    {
                        nSolution++;
                        if (nSolution > 1)
                            break;
                    }
                }
                if (candidates[0] > 9 || level == 9 * 9)
                {
                    //Debug.Print(String.Format("Backtracking level={0}", level));
                    candidatesList.RemoveLast();
                    while (--level >= 0 && board[level / 9, level % 9] > 0) ;
                    if (level < 0) break;
                }
            }
            unique = nSolution == 1;
            bool ret = nSolution > 0;
            //bool ret = _Solve(out answer, ref board, out unique, 0, ref nTries, ref clues);
            Debug.Print(String.Format("{0}:{1}:solved={2}:unique={3}", (DateTime.Now - startTime).TotalSeconds, nTries, ret, unique));
            return ret;
        }

        protected bool _Solve(out int[,]? answer, ref int[,] board, out bool unique, int level, ref int nTries, ref int[,,] clues)
        {
            answer = null;
            unique = true;
            // Get current subgrid.
            if (level == 81)
            {
                answer = board.Clone() as int[,];
                return true;
            }
            int i = level / 9;
            int j = level % 9;
            if (board[i, j] > 0) return _Solve(out answer, ref board, out unique, level + 1, ref nTries, ref clues);
            var idx = Common.GetIdxFromRC(i, j);
            // Search
            int nSolution = 0;
            int[] order = Enumerable.Range(1, 9).ToArray();
            Shuffle(ref order);
            foreach (int candidate in order)
            {
                if (clues[0, i, candidate] > 0 || clues[1, j, candidate] > 0 || clues[2, idx.Item1, candidate] > 0)
                    continue;
                board[i, j] = candidate;
                clues[0, i, candidate]++;
                clues[1, j, candidate]++;
                clues[2, idx.Item1, candidate]++;
                bool _unique;
                int[,]? _answer;
                if (++nTries < MAX_TRIES && _Solve(out _answer, ref board, out _unique, level + 1, ref nTries, ref clues))
                {
                    answer = _answer;
                    nSolution++;
                    if (!_unique)
                        nSolution++;
                }
                board[i, j] = 0;
                clues[0, i, candidate]--;
                clues[1, j, candidate]--;
                clues[2, idx.Item1, candidate]--;
                if (nTries >= MAX_TRIES) return false;
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

        abstract protected void GetValid(in int[,] board, int x, int y, out bool[] v);
    }
}
