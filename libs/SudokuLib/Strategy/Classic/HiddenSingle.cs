using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SudokuLib.Strategy.Op;

namespace SudokuLib.Strategy.Classic
{
    public class HiddenSingle : Strategy<ClassicSudoku, HiddenSingle>
    {
        public override OpBase ExecuteOnBoard(ClassicSudoku game)
        {
            var row = new int[9, 9+1];
            var col = new int[9, 9+1];
            var grid = new int[9+1, 9+1];
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    row[i, j + 1] = col[i, j + 1] = grid[i + 1, j + 1] = -1;
            for (int r = 0; r < 9; r ++)
                for (int c = 0; c < 9; c ++)
                    if (game.board[r, c] == 0)
                        for (int d = 1; d <= 9; d ++)
                            if (game.candidates.CheckValid(r, c, d))
                            {
                                if (row[r, d] == -1)
                                    row[r, d] = c;
                                else if (row[r, d] >= 0)
                                    row[r, d] = -2;
                                if (col[c, d] == -1)
                                    col[c, d] = r;
                                else if (col[c, d] >= 0)
                                    col[c, d] = -2;
                                var (idx0, idx1) = Common.GetIdxFromRC(r, c);
                                if (grid[idx0, d] == -1)
                                    grid[idx0, d] = idx1;
                                else if (grid[idx0, d] >= 0)
                                    grid[idx0, d] = -2;
                            }
            return new OpList
            {
                new OpList(
                    from r in Enumerable.Range(0, 9)
                    from d in Enumerable.Range(1, 9)
                    let c = row[r, d]
                    where c >= 0
                    select FillDigit.Instance.ExecuteOnDigit(game, r, c, d) as OpBase
                ),
                new OpList(
                    from c in Enumerable.Range(0, 9)
                    from d in Enumerable.Range(1, 9)
                    let r = col[c, d]
                    where r >= 0
                    select FillDigit.Instance.ExecuteOnDigit(game, r, c, d) as OpBase
                ),
                new OpList(
                    from idx0 in Enumerable.Range(1, 9)
                    from d in Enumerable.Range(1, 9)
                    let idx1 = grid[idx0, d]
                    where idx1 > 0
                    let rc = Common.GetRCFromIdx(idx0, idx1)
                    select FillDigit.Instance.ExecuteOnDigit(game, rc.Item1, rc.Item2, d) as OpBase
                )
            };
        }
    }
}
