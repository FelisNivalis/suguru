using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SudokuLib.Strategy.Op;

namespace SudokuLib.Strategy.Classic
{
    public class RestartAction : Strategy<ClassicSudoku, RestartAction>
    {
        public override OpBase ExecuteOnBoard(ClassicSudoku game)
        {
            return new OpList {
                UndoHighlightRow.Instance.ExecuteOnBoard(game),
                UndoHighlightSameDigit.Instance.ExecuteOnBoard(game),
                new OpList(
                    from r in Enumerable.Range(0, 9)
                    from c in Enumerable.Range(0, 9)
                    from d in Enumerable.Range(1, 9)
                    select new DigitOp<UnEliminateOp>(r, c, d) as OpBase
                ),
                FillDigit.Instance.ExecuteOnEverySubgridDigit(game),
                BasicEliminate.Instance.ExecuteOnEverySubgridDigit(game),
            };
        }
    }
}
