namespace SudokuLib
{
    public abstract class SudokuData
    {
        abstract public void Eliminate(int r, int c, int d);
        abstract public void UndoEliminate(int r, int c, int d);
    }
    public class Candidates : SudokuData
    {
        int[,] candidates = new int[9, 9];
        static int[] uniqueCandidate = new int[1 << 9];
        static Candidates()
        {
            for (int i = 1; i <= 9; i++)
                uniqueCandidate[(1 << 9) - 1 - (1 << i - 1)] = i;
        }
        public Candidates(Candidates c)
        {
            candidates = (int[,])c.candidates.Clone();
        }
        public Candidates() { }
        public int GetBits(int r, int c)
        {
            return candidates[r, c] ^ (1 << 9) - 1;
        }
        public bool CheckAtMost(int r, int c, int d)
        {
            return (GetBits(r, c) | d) == d;
        }
        public bool CheckAtMost(int r, int c, in int[] ds)
        {
            return CheckAtMost(r, c, ds.Select(_d => 1 << _d - 1).Sum());
        }
        public bool CheckAny(int r, int c, int d)
        {
            return (GetBits(r, c) & d) != 0;
        }
        public bool CheckAny(int r, int c, in int[] ds)
        {
            return CheckAny(r, c, ds.Select(_d => 1 << _d - 1).Sum());
        }
        public bool CheckValid(int r, int c, int d)
        {
            return (candidates[r, c] & (1 << d - 1)) == 0;
        }
        public bool CheckInvalid(int r, int c)
        {
            return candidates[r, c] == (1 << 9) - 1;
        }
        public int UniqueCandidate(int r, int c)
        {
            return uniqueCandidate[candidates[r, c]];
        }

        public override void Eliminate(int r, int c, int d)
        {
            candidates[r, c] |= 1 << d - 1;
        }
        public override void UndoEliminate(int r, int c, int d)
        {
            candidates[r, c] &= (1 << 9) - 1 - (1 << d - 1);
        }
    }

    public class CandidatesCounter : SudokuData
    {
        public int[,] nRow = new int[9, 10];
        public int[,] nCol = new int[9, 10];
        public int[,] nGrid = new int[10, 10];

        public CandidatesCounter(CandidatesCounter c)
        {
            nRow = (int[,])c.nRow.Clone();
            nCol = (int[,])c.nCol.Clone();
            nGrid = (int[,])c.nGrid.Clone();
        }
        public CandidatesCounter() { }

        public override void Eliminate(int r, int c, int d)
        {
            nRow[r, d]++;
            nCol[c, d]++;
            var (idx_grid, _) = Common.GetIdxFromRC(r, c);
            nGrid[idx_grid, d]++;
        }

        public override void UndoEliminate(int r, int c, int d)
        {
            nRow[r, d]--;
            nCol[c, d]--;
            var (idx_grid, _) = Common.GetIdxFromRC(r, c);
            nGrid[idx_grid, d]--;
        }
    }
}
