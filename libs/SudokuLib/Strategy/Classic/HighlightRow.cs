﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SudokuLib.Strategy.Op;

namespace SudokuLib.Strategy.Classic
{
    public class HighlightRow : Strategy<ClassicSudoku, HighlightRow>
    {
        override public OpList ExecuteOnSubgrid(ClassicSudoku game, int row, int column)
        {
            return new OpList(
                from i in Enumerable.Range(0, 9)
                select new SubgridSelectOp(row, i) as OpBase
            );
        }

    }
}
