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
        public Candidates candidates = new();
        protected int seed = -1;
        protected Random rand;
        private const int MAX_TRIES = 10000;

        public class Candidates
        {
            int[,] candidates = new int[9, 9];
            static int[] uniqueCandidate = new int[1 << 9];
            static Candidates()
            {
                for (int i = 1; i <= 9; i++)
                    uniqueCandidate[1 << i - 1] = i;
            }
            public Candidates() { }
            public Candidates(Candidates c)
            {
                candidates = c.candidates.Clone() as int[,];
            }
            public bool Get(int r, int c, int d)
            {
                return (candidates[r, c] & (1 << d - 1)) > 0;
            }
            public bool Set(int r, int c, int d)
            {
                bool ret = (candidates[r, c] & (1 << d - 1)) > 0;
                candidates[r, c] |= 1 << d - 1;
                return ret;
            }
            public bool Unset(int r, int c, int d)
            {
                bool ret = (candidates[r, c] & (1 << d - 1)) > 0;
                candidates[r, c] &= (1 << 9) - 1 - (1 << d - 1);
                return ret;
            }
            public int UniqueCandidate(int r, int c)
            {
                return uniqueCandidate[candidates[r, c]];
            }
        }

        public SudokuBase(SudokuBase other)
        {
            board = other.board.Clone() as int[,];
            init_board = other.init_board.Clone() as int[,];
            answer = other.answer.Clone() as int[,];
            candidates = new Candidates(other.candidates);
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
    }
}
