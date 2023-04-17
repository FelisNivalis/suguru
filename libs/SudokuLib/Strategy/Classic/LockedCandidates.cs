using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SudokuLib.Strategy.Op;

namespace SudokuLib.Strategy.Classic
{
    public class LockedCandidatesType1 : Strategy<ClassicSudoku, LockedCandidatesType1>
    {
        public override OpBase ExecuteOnBoard(ClassicSudoku game)
        {
            OpList opList = new();
            for (int idx = 1; idx <= 9; idx++)
            {
                for (int d = 1; d <= 9; d++)
                {
                    int uc = -1, ur = -1;
                    for (int idx_subgrid = 1; idx_subgrid <= 9; idx_subgrid++)
                    {
                        var (r, c) = Common.GetRCFromIdx(idx, idx_subgrid);
                        if (game.board[r, c] == 0 && game.candidates.CheckValid(r, c, d))
                        {
                            if (ur == -1) ur = r;
                            else if (ur >= 0 && ur != r) ur = -2; // candidate `d` valid on more than 1 row
                            if (uc == -1) uc = c;
                            else if (uc >= 0 && uc != c) uc = -2; // candidate `d` valid on more than 1 col
                        }
                    }
                    if (ur >= 0)
                    {
                        for (int _c = 0; _c < 9; _c++) // Eliminate all columns other than in the same subgrid
                            if (Common.GetIdxFromRC(ur, _c).Item1 != idx)
                                opList.Add(new DigitOp<EliminateOp>(ur, _c, d));
                    }
                    if (uc >= 0)
                    {
                        for (int _r = 0; _r < 9; _r++)
                            if (Common.GetIdxFromRC(_r, uc).Item1 != idx)
                                opList.Add(new DigitOp<EliminateOp>(_r, uc, d));
                    }
                }
            }
            return opList;
        }
    }
}
