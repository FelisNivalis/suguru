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
        public override OpList ExecuteOnSubgrid(ClassicSudoku game, int row, int column)
        {
            if (game.board[row, column] != 0) return new OpList();
            var candidates = from digit in Enumerable.Range(1, 9)
            where game.candidates[row, column, digit]
            select digit;
            if (candidates.Count() == 1) return UIFillDigit.Instance.ExecuteOnDigit(game, row, column, candidates.First());
            return new OpList();
        }
    }
}
