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
    public class LockedCandidatesType2 : Strategy<ClassicSudoku, LockedCandidatesType2>
    {
        public override OpBase ExecuteOnBoard(ClassicSudoku game)
        {
            OpList opList = new();
            for (int i = 0; i < 9; i++)
            {
                for (int d = 1; d <= 9; d++)
                {
                    int ug = -1, ug2 = -1;
                    for (int j = 0; j < 9; j++)
                    {
                        var idx = Common.GetIdxFromRC(j, i);
                        if (game.board[j, i] == 0 && game.candidates.CheckValid(j, i, d))
                        {
                            if (ug == -1) ug = idx.Item1;
                            else if (ug >= 0 && ug != idx.Item1) ug = -2; // candidate `d` valid in more than 1 grid
                        }
                        idx = Common.GetIdxFromRC(i, j);
                        if (game.board[i, j] == 0 && game.candidates.CheckValid(i, j, d))
                        {
                            if (ug2 == -1) ug2 = idx.Item1;
                            else if (ug2 >= 0 && ug2 != idx.Item1) ug2 = -2;
                        }
                    }
                    if (ug >= 0)
                    {
                        for (int sg = 1; sg <= 9; sg++) // Eliminate all subgrids other than in the same column
                        {
                            var rc = Common.GetRCFromIdx(ug, sg);
                            if (rc.Item2 != i)
                                opList.Add(new DigitOp<EliminateOp>(rc.Item1, rc.Item2, d));
                        }
                    }
                    if (ug2 >= 0)
                    {
                        for (int sg = 1; sg <= 9; sg++) // Eliminate all subgrids other than in the same row
                        {
                            var rc = Common.GetRCFromIdx(ug2, sg);
                            if (rc.Item1 != i)
                                opList.Add(new DigitOp<EliminateOp>(rc.Item1, rc.Item2, d));
                        }
                    }
                }
            }
            return opList;
        }
    }
}
