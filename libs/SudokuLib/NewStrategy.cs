using SudokuLib.Strategy.Op;

namespace SudokuLib
{
    abstract public class NewStrategy
    {
        public ClassicSudoku game;
        public NewStrategy(ClassicSudoku game) { this.game = game; }
    }
    public class NewHiddenSingle : NewStrategy
    {
        public Candidates candidates;
        public CandidatesCounter nCandidates;
        public NewHiddenSingle(ClassicSudoku game) : base(game)
        {
            candidates = game.GetData<Candidates>();
            nCandidates = game.GetData<CandidatesCounter>();
        }
        public void Execute(DigitOp<EliminateOp> op)
        {
            throw new NotImplementedException();
        }
    }
}
