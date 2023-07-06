namespace SudokuLib
{
    public class Board
    {
        public int[,] board = new int[9, 9];
        public Candidates candidates = new();
        public CandidatesCounter nCandidates = new();
    }
    public class ClueData
    {
        public int[] row = new int[9];
        public int[] col = new int[9];
        public int[] grid = new int[10];
    }
    public abstract class SudokuBase
    {
        Dictionary<Type, SudokuData> dataDict = new();
        public int[,] board = new int[9, 9];
        public int[,] init_board = new int[9, 9];
        public int[,] answer = new int[9, 9];
        public Candidates candidates = new();
        protected int seed = -1;
        protected Random rand;

        public SudokuBase(SudokuBase other)
        {
            board = (int[,])other.board.Clone();
            init_board = (int[,])other.init_board.Clone();
            answer = (int[,])other.answer.Clone();
            candidates = new Candidates(other.candidates);
            rand = other.rand;
            seed = other.seed;
        }

        public SudokuBase(int seed = -1)
        {
            this.seed = seed;
            rand = new Random();
        }

        public D GetData<D>() where D : SudokuData
        {
            if (!dataDict.TryGetValue(typeof(D), out SudokuData? data))
            {
                D newdata = Activator.CreateInstance<D>();
                dataDict[typeof(D)] = newdata;
                return newdata;
            }
            return (D)data;
        }

        public bool Solved()
        {
            for (int r = 0; r < 9; r++) for (int c = 0; c < 9; c++) if (board[r, c] == 0) return false;
            return true;
        }

        public void Generate()
        {
            Clear();
            int[,]? _answer;
            //while (!Solve(out _answer, ref board, out _
            while (!Solve2(1, ref board, out _answer)) ;
            if (_answer == null) return;
            answer = _answer;
            board = (int[,])answer.Clone();
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
                    if (!Solve2(2, ref board, out _))
                        flag = true;
                    else
                        board[x, y] = answer[x, y];
                }
                if (!flag) break;
            }
            init_board = (int[,])board.Clone();
        }

        protected void Clear()
        {
            board = new int[9, 9];
            answer = new int[9, 9];
            init_board = new int[9, 9];
            candidates = new ();
            //rand = seed < 0 ? new Random() : new Random(seed);
        }

        class StackInfo
        {
            public int idx = 0;
            public int digit = 0;
            public int[,] board = new int[9, 9];
            public Candidates candidates = new();
            public CandidatesCounter nCandidates = new();
            public StackInfo(in int[,] board)
            {
                // New stack. Find the first empty subgrid.
                this.board = (int[,])board.Clone();
                Next();
                // Init candidates
                Queue<(int, int, int)> q = new();
                for (int i = 0; i < 9; i++)
                    for (int j = 0; j < 9; j++)
                        if (board[i, j] > 0)
                            q.Enqueue((i, j, board[i, j]));
                FillCont(ref q);
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
                board = (int[,])s.board.Clone();
                candidates = new Candidates(s.candidates);
                nCandidates = new CandidatesCounter(s.nCandidates);
            }
            static public int[] cand = new int[1 << 9];
            static StackInfo()
            {
                Array.Clear(cand, 0, cand.Length);
                for (int i = 0; i < 9; i++)
                    cand[(1 << 9) - 1 - (1 << i)] = i + 1;
            }
            public record struct UpdateCandidatesType(bool CheckRow, bool CheckCol, bool CheckGrid) { }
            public bool Eliminate(ref Queue<(int, int, int)> q, int r, int c, int d, UpdateCandidatesType type)
            {
                // Update after crossing out a candidate
                if (board[r, c] == 0 && candidates.CheckValid(r, c, d))
                {
                    candidates.Eliminate(r, c, d);
                    // Check naked single
                    int cand = candidates.UniqueCandidate(r, c);
                    if (cand > 0) q.Enqueue((r, c, cand));
                    if (candidates.CheckInvalid(r, c)) return false;
                    nCandidates.Eliminate(r, c, d);
                    // nCandidatesRow
                    if (type.CheckRow)
                    {
                        // Hidden single
                        if (nCandidates.nRow[r, d] == 8)
                            for (int j = 0; j < 9; j++)
                                if (candidates.CheckValid(r, j, d))
                                    q.Enqueue((r, j, d));
                        if (nCandidates.nRow[r, d] == 9) return false;
                    }
                    // nCandidatesCol
                    if (type.CheckCol)
                    {
                        // Hidden single
                        if (nCandidates.nCol[c, d] == 8)
                            for (int j = 0; j < 9; j++)
                                if (candidates.CheckValid(j, c, d))
                                    q.Enqueue((j, c, d));
                        if (nCandidates.nCol[c, d] == 9) return false;
                    }
                    
                    var (idx_grid, idx_subgrid) = Common.GetIdxFromRC(r, c);
                    if (type.CheckGrid)
                    {
                        // Hidden single
                        if (nCandidates.nGrid[idx_grid, d] == 8)
                            for (int j = 0; j < 9; j++)
                            {
                                var (_r, _c) = Common.GetRCFromIdx(idx_grid, j + 1);
                                if (candidates.CheckValid(_r, _c, d))
                                    q.Enqueue((_r, _c, d));
                            }
                        if (nCandidates.nGrid[idx_grid, d] == 9) return false;
                    }
                }
                return true;
            }
            public bool FillCont(ref Queue<(int, int, int)> q)
            {
                // Fill continuously until queue empty.
                while (q.Count > 0)
                {
                    var (r, c, d) = q.Dequeue();
                    if (!Fill(ref q, r, c, d)) return false;
                }
                return true;
            }
            public bool Fill(ref Queue<(int, int, int)> q, int r, int c, int d)
            {
                board[r, c] = d;
                var idx = Common.GetIdxFromRC(r, c);
                for (int i = 0; i < 9; i++)
                {
                    // Don't check row when crossing out same row, similar for col
                    // For grid, only when crossing out the last one of the grid, and not in the same grid as (_r, _c)
                    if (!Eliminate(ref q, r, i, d, new UpdateCandidatesType(false, true, i % 3 == 2 && i / 3 != c / 3))) return false;
                    if (!Eliminate(ref q, i, c, d, new UpdateCandidatesType(true, false, i % 3 == 2 && i / 3 != r / 3))) return false;
                    var (_r, _c) = Common.GetRCFromIdx(idx.Item1, i + 1);
                    if (!Eliminate(ref q, _r, _c, d, new UpdateCandidatesType(_r != r && _c % 3 == 2, _c != c && _r % 3 == 2, false))) return false;
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
                    if (s.candidates.CheckValid(r, c, s.digit))
                    {
                        // Until no hidden singles and naked singles
                        Queue<(int, int, int)> q = new();
                        q.Enqueue((r, c, s.digit));
                        var ns = new StackInfo(s);
                        if (ns.FillCont(ref q))
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
