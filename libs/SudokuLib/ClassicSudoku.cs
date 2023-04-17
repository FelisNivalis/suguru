namespace SudokuLib
{
    public partial class ClassicSudoku : SudokuBase
    {
        public ClassicSudoku(int seed = -1) : base(seed) { }
        public ClassicSudoku(ClassicSudoku other): base(other) { }
    }
}
