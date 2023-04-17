using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Xml;

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
        private const int MAX_TRIES = 10000;

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
            var startTime = DateTime.Now;
            Clear();
            int[,]? _answer;
            //while (!Solve(out _answer, ref board, out _
            while (!Solve2(1, ref board, out _answer)) ;
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
                    //if (Solve(out _, ref board, out unique) && unique)
                    if (!Solve2(2, ref board, out _))
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
            Debug.Print(String.Format("{0}", (DateTime.Now - startTime).TotalMilliseconds));
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
            double timeCount = 0;
            bool ret = _Solve(out answer, ref board, out unique, ref nTries, ref clues, ref timeCount);
            if (nTries > 1000) Debug.Print(String.Format("{0}:sortTime={4}:{1}:solved={2}:unique={3}", (DateTime.Now - startTime).TotalNanoseconds / nTries, nTries, ret, unique, timeCount / nTries));
            return ret;
        }

        struct StackInfo
        {
            public int idx = 0;
            public int digit = 0;
            public int[,] board = new int[9, 9];
            public int[,] candidates = new int[9, 9];
            public int[,] nCandidatesRow = new int[9, 10];
            public int[,] nCandidatesCol = new int[9, 10];
            public int[,] nCandidatesGrid = new int[10, 10];
            public StackInfo(in int[,] board)
            {
                // New stack. Find the first empty subgrid.
                this.board = board.Clone() as int[,];
                Next();
                // Init candidates
                Queue<(int, int, int)> q = new();
                for (int i = 0; i < 9; i++)
                    for (int j = 0; j < 9; j++)
                        if (board[i, j] > 0)
                            q.Enqueue((i, j, board[i, j]));
                Fill(ref q);
            }
            public void Next()
            {
                for (; idx < 9 * 9 && board[idx / 9, idx % 9] != 0; idx++) ;
                digit = 0;
            }
            public StackInfo(StackInfo s)
            {
                idx = s.idx;
                digit = s.digit;
                board = s.board.Clone() as int[,];
                candidates = s.candidates.Clone() as int[,];
                nCandidatesRow = s.nCandidatesRow.Clone() as int[,];
                nCandidatesCol = s.nCandidatesCol.Clone() as int[,];
                nCandidatesGrid = s.nCandidatesGrid.Clone() as int[,];
            }
            static public int[] cand = new int[1 << 9];
            static StackInfo()
            {
                Array.Clear(cand, 0, cand.Length);
                for (int i = 0; i < 9; i++)
                    cand[(1 << 9) - 1 - (1 << i)] = i + 1;
            }
            public record struct UpdateCandidatesType(bool CheckRow, bool CheckCol, bool CheckGrid) { }
            public bool UpdateCandidates(ref Queue<(int, int, int)> q, int r, int c, int d, UpdateCandidatesType type)
            {
                // Update after crossing out a candidate
                int b = 1 << d - 1;
                if (board[r, c] == 0 && (candidates[r, c] & b) == 0)
                {
                    candidates[r, c] |= b;
                    // Check naked single
                    int cand = StackInfo.cand[candidates[r, c]];
                    if (cand > 0) q.Enqueue((r, c, cand));
                    if (candidates[r, c] == (1 << 9) - 1) return false;
                    nCandidatesRow[r, d]++;
                    // nCandidatesRow
                    if (type.CheckRow)
                    {
                        // Hidden single
                        if (nCandidatesRow[r, d] == 8)
                            for (int j = 0; j < 9; j++)
                                if ((candidates[r, j] & b) == 0)
                                    q.Enqueue((r, j, d));
                        if (nCandidatesRow[r, d] == 9) return false;
                    }
                    // nCandidatesCol
                    nCandidatesCol[c, d]++;
                    if (type.CheckCol)
                    {
                        // Hidden single
                        if (nCandidatesCol[c, d] == 8)
                            for (int j = 0; j < 9; j++)
                                if ((candidates[j, c] & b) == 0)
                                    q.Enqueue((j, c, d));
                        if (nCandidatesCol[c, d] == 9) return false;
                    }
                    
                    var (idx_grid, idx_subgrid) = Common.GetIdxFromRC(r, c);
                    nCandidatesGrid[idx_grid, d]++;
                    if (type.CheckGrid)
                    {
                        // Hidden single
                        if (nCandidatesGrid[idx_grid, d] == 8)
                            for (int j = 0; j < 9; j++)
                            {
                                var (_r, _c) = Common.GetRCFromIdx(idx_grid, j + 1);
                                if ((candidates[_r, _c] & b) == 0)
                                    q.Enqueue((_r, _c, d));
                            }
                        if (nCandidatesGrid[idx_grid, d] == 9) return false;
                    }
                }
                return true;
            }
            public bool Fill(ref Queue<(int, int, int)> q)
            {
                while (q.Count > 0)
                {
                    var (r, c, d) = q.Dequeue();
                    if (!_Fill(ref q, r, c, d)) return false;
                }
                return true;
            }
            public bool _Fill(ref Queue<(int, int, int)> q, int r, int c, int d)
            {
                board[r, c] = d;
                var idx = Common.GetIdxFromRC(r, c);
                for (int i = 0; i < 9; i++)
                {
                    // Don't check row when crossing out same row, similar for col
                    // For grid, only when crossing out the last one of the grid, and not in the same grid as (_r, _c)
                    if (!UpdateCandidates(ref q, r, i, d, new UpdateCandidatesType(false, true, i % 3 == 2 && i / 3 != c / 3))) return false;
                    if (!UpdateCandidates(ref q, i, c, d, new UpdateCandidatesType(true, false, i % 3 == 2 && i / 3 != r / 3))) return false;
                    var (_r, _c) = Common.GetRCFromIdx(idx.Item1, i + 1);
                    if (!UpdateCandidates(ref q, _r, _c, d, new UpdateCandidatesType(_r != r && _c % 3 == 2, _c != c && _r % 3 == 2, false))) return false;
                }
                return true;
            }
        }

        protected bool Solve2(in int nSolutionsToFind, ref int[,] board, out int[,]? answer)
        {
            answer = null;
            int nSolution = 0;
            Stack<StackInfo> stack = new ();
            stack.Push(new StackInfo(board));
            while (stack.Count > 0)
            {
                var s = stack.Pop();
                if (s.idx == 9 * 9)
                {
                    if (nSolution == 0)
                        answer = s.board.Clone() as int[,];
                    nSolution++;
                    if (nSolution >= nSolutionsToFind)
                        break;
                    continue;
                }
                int r = s.idx / 9;
                int c = s.idx % 9;
                // Try every candidate
                for (s.digit++; s.digit <= 9; s.digit++)
                {
                    int b = 1 << s.digit - 1;
                    if ((s.candidates[r, c] & b) == 0)
                    {
                        // Until no hidden singles and naked singles
                        Queue<(int, int, int)> q = new();
                        q.Enqueue((r, c, s.digit));
                        var ns = new StackInfo(s);
                        if (ns.Fill(ref q))
                        {
                            ns.Next();
                            stack.Push(s);
                            stack.Push(ns);
                            break;
                        }
                    }
                }
            }
            return nSolution >= nSolutionsToFind;
        }

        protected bool _Solve(out int[,]? answer, ref int[,] board, out bool unique, ref int nTries, ref int[,,] clues, ref double timeCount)
        {
            answer = null;
            unique = true;
            // Get current subgrid.
            int r, c;
            r = c = 0;
            (int, int) idx;
            int[] order = Enumerable.Range(1, 9).ToArray();
            Shuffle(ref order);
            // Find a subgrid with least possible clues
            int minNV = 10;
            var startTime = DateTime.Now;
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    if (board[i, j] == 0)
                    {
                        int nV = 0;
                        idx = Common.GetIdxFromRC(i, j);
                        int cand = 0;
                        for (int d = 1; d <= 9; d++)
                            if (clues[0, i, d] == 0 && clues[1, j, d] == 0 && clues[2, idx.Item1, d] == 0)
                            {
                                nV++;
                                cand = d;
                            }
                        // No solutions
                        if (nV == 0)
                        {
                            timeCount += (DateTime.Now - startTime).TotalNanoseconds;
                            return false;
                        }
                        if (nV < minNV)
                        {
                            minNV = nV;
                            r = i;
                            c = j;
                            if (nV == 1)
                            {
                                order = new[] { cand };
                                goto L1;
                            }
                        }
                    }
            L1:
            timeCount += (DateTime.Now - startTime).TotalNanoseconds;
            if (minNV == 10)
            {
                answer = board.Clone() as int[,];
                return true;
            }
            idx = Common.GetIdxFromRC(r, c);
            // Search
            int nSolution = 0;
            foreach (int candidate in order)
            {
                if (clues[0, r, candidate] > 0 || clues[1, c, candidate] > 0 || clues[2, idx.Item1, candidate] > 0)
                    continue;
                board[r, c] = candidate;
                clues[0, r, candidate]++;
                clues[1, c, candidate]++;
                clues[2, idx.Item1, candidate]++;
                bool _unique;
                int[,]? _answer;
                if (++nTries < MAX_TRIES && _Solve(out _answer, ref board, out _unique, ref nTries, ref clues, ref timeCount))
                {
                    answer = _answer;
                    nSolution++;
                    if (!_unique)
                        nSolution++;
                }
                board[r, c] = 0;
                clues[0, r, candidate]--;
                clues[1, c, candidate]--;
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
