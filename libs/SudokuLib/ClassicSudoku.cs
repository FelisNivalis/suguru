namespace SudokuLib
{
    public partial class ClassicSudoku : SudokuBase
    {
        public int[,,] candidates = new int[9, 9, 9];
        public ClassicSudoku(int seed = -1) : base(seed) { }
        public ClassicSudoku(ClassicSudoku other): base(other)
        {
            candidates = other.candidates.Clone() as int[,,];
        }
        override protected void GetValid(in int[,] board, int x, int y, out bool[] v)
        {
            v = new bool[9 + 1];
            for (int i = 1; i <= 9; i++) v[i] = true;
            int gx = x / 3 * 3, gy = y / 3 * 3;
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    v[board[gx + i, gy + j]] = false;
            for (int i = 0; i < 9; i++)
                v[board[i, y]] = v[board[x, i]] = false;
        }

        static void Main(string[] args)
        {
            // Display the number of command line arguments.
            Console.WriteLine(args.Length);
        }
    }
}
