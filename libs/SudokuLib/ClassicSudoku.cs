namespace SudokuLib
{
    public partial class ClassicSudoku : SudokuBase
    {
        public ClassicSudoku(int seed = -1) : base(seed) { }
        public ClassicSudoku(ClassicSudoku other): base(other) { }
        override protected void GetValid(in int[,] board, int x, int y, out bool[] v)
        {
            v = new bool[9 + 1];
            for (int i = 1; i <= 9; i++) v[i] = true;
            var idx = Common.GetIdxFromRC(x, y);
            for (int i = 1; i <= 9; i++)
            {
                var rc = Common.GetRCFromIdx(idx.Item1, i);
                v[board[rc.Item1, rc.Item2]] = false;
            }
            for (int i = 0; i < 9; i++) v[board[i, y]] = v[board[x, i]] = false;
        }
    }
}
