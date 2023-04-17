using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SudokuLib.Strategy.Op;

namespace SudokuLib.Strategy.Classic
{
    public class NakedSingle : Strategy<ClassicSudoku, NakedSingle>
    {
        public override OpBase ExecuteOnSubgrid(ClassicSudoku game, int row, int column)
        {
            if (game.board[row, column] != 0) return new OpList();
            var candidate = game.candidates.UniqueCandidate(row, column);
            if (candidate > 0) return UIFillDigit.Instance.ExecuteOnDigit(game, row, column, candidate);
            return new OpList();
        }
    }
}
