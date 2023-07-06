using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using SudokuLib.Strategy.Op;

namespace SudokuLib.Strategy
{
    public static class Executors
    {
        public static bool Execute(this OpBase op, ClassicSudoku game)
        {
            return Execute(op as dynamic, game);
        }

        public static bool Execute(this EmptyOp op, ClassicSudoku game) { return false; }

        public static bool Execute(this OpList op, ClassicSudoku game)
        {
            return (from _op in op.ops
                    select Execute(_op as dynamic, game))
                    .Count(b => b) > 0;
        }

        public static bool Execute(this DigitOp<FillOp> op, ClassicSudoku game)
        {
            if (game.board[op.Row, op.Column] == op.Digit) return false;
            game.board[op.Row, op.Column] = op.Digit;
            return true;
        }

        public static bool Execute(this DigitOp<EliminateOp> op, ClassicSudoku game)
        {
            if (!game.candidates.CheckValid(op.Row, op.Column, op.Digit) || game.board[op.Row, op.Column] != 0) return false;
            game.candidates.Eliminate(op.Row, op.Column, op.Digit);
            return true;
        }

        public static bool Execute(this DigitOp<UnEliminateOp> op, ClassicSudoku game)
        {
            if (game.candidates.CheckValid(op.Row, op.Column, op.Digit) || game.board[op.Row, op.Column] != 0) return false;
            game.candidates.UndoEliminate(op.Row, op.Column, op.Digit);
            return true;
        }
    }
}
