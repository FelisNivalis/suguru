using System.Diagnostics;
using Godot;
using SudokuLib;
using SudokuLib.Strategy.Op;

namespace Sudoku.Executors
{
    public static class Executors
    {
        static StyleBoxFlat DigitBgCircle_SameNumberStyle = GD.Load<StyleBoxFlat>("res://res/themes/digit_bg_circle_same_number_style.tres");
        static StyleBoxFlat SubgridBg_SameRCSelectedStyle = GD.Load<StyleBoxFlat>("res://res/themes/subgrid_bg_rc_style.tres");
        static StyleBoxEmpty EmptyBoxStyle = GD.Load<StyleBoxEmpty>("res://res/themes/style_box_empty.tres");
        static LabelSettings SubgridLabel_DefaultSettings = GD.Load<LabelSettings>("res://res/themes/default_subgrid_label_settings.tres");
        static LabelSettings SubgridLabel_ErrorSettings = GD.Load<LabelSettings>("res://res/themes/error_subgrid_label_settings.tres");

        public static bool Execute(this OpBase op, ClassicSudokuScriptNode scriptNode)
        {
            return Execute(op as dynamic, scriptNode);
        }

        private static void SetPanelStyle(this Node node, Resource style)
        {
            node.Set("theme_override_styles/panel", style);
        }

        public static bool Execute(this EmptyOp op, ClassicSudokuScriptNode scriptNode) { return false; }

        public static bool Execute(this OpList op, ClassicSudokuScriptNode scriptNode)
        {
            bool ret = false;
            foreach (OpBase _op in op.ops) ret |= Execute(_op as dynamic, scriptNode);
            return ret;
        }

        public static bool Execute(this DigitOp<DigitSelectOp> op, ClassicSudokuScriptNode scriptNode)
        {
            var idx = Common.GetIdxFromRC(op.Row, op.Column);
            scriptNode.GetDigitNode(idx.Item1, idx.Item2, op.Digit).GetNode("%BgCircle")?.SetPanelStyle(DigitBgCircle_SameNumberStyle);
            return false;
        }

        public static bool Execute(this SubgridOp<SubgridSelectOp> op, ClassicSudokuScriptNode scriptNode)
        {
            var idx = Common.GetIdxFromRC(op.Row, op.Column);
            scriptNode.GetSubgrid(idx.Item1, idx.Item2).GetNode("%Bg")?.SetPanelStyle(SubgridBg_SameRCSelectedStyle);
            return false;
        }

        public static bool Execute(this DigitOp<DigitUnselectOp> op, ClassicSudokuScriptNode scriptNode)
        {
            var idx = Common.GetIdxFromRC(op.Row, op.Column);
            scriptNode.GetDigitNode(idx.Item1, idx.Item2, op.Digit).GetNode("%BgCircle")?.SetPanelStyle(EmptyBoxStyle);
            return false;
        }

        public static bool Execute(this SubgridOp<SubgridUnselectOp> op, ClassicSudokuScriptNode scriptNode)
        {
            var idx = Common.GetIdxFromRC(op.Row, op.Column);
            scriptNode.GetSubgrid(idx.Item1, idx.Item2).GetNode("%Bg")?.SetPanelStyle(EmptyBoxStyle);
            return false;
        }

        public static bool Execute(this DigitOp<FillOp> op, ClassicSudokuScriptNode scriptNode)
        {
            //if (scriptNode.sudoku.board[op.Row, op.Column] > 0)
            var idx = Common.GetIdxFromRC(op.Row, op.Column);
            scriptNode.GetSubgrid(idx.Item1, idx.Item2).Call("fill_with", new Variant[] { op.Digit });
            if (scriptNode.sudoku.board[op.Row, op.Column] == op.Digit) return false;
            scriptNode.sudoku.board[op.Row, op.Column] = op.Digit;
            Debug.Print(string.Format("FillOp {0},{1},{2}", op.Row, op.Column, op.Digit));
            return true;
        }

        public static bool Execute(this DigitOp<EliminateOp> op, ClassicSudokuScriptNode scriptNode)
        {
            var idx = Common.GetIdxFromRC(op.Row, op.Column);
            scriptNode.GetDigitNode(idx.Item1, idx.Item2, op.Digit).Call("hide_text", new Variant[] { });
            if (!scriptNode.sudoku.candidates.CheckValid(op.Row, op.Column, op.Digit) || scriptNode.sudoku.board[op.Row, op.Column] != 0) return false;
            scriptNode.sudoku.candidates.Eliminate(op.Row, op.Column, op.Digit);
            Debug.Print(string.Format("EliminateOp {0},{1},{2}", op.Row, op.Column, op.Digit));
            return true;
        }

        public static bool Execute(this DigitOp<UnEliminateOp> op, ClassicSudokuScriptNode scriptNode)
        {
            var idx = Common.GetIdxFromRC(op.Row, op.Column);
            scriptNode.GetDigitNode(idx.Item1, idx.Item2, op.Digit).Call("show_text", new Variant[] { });
            if (scriptNode.sudoku.candidates.CheckValid(op.Row, op.Column, op.Digit) || scriptNode.sudoku.board[op.Row, op.Column] != 0) return false;
            scriptNode.sudoku.candidates.Uneliminate(op.Row, op.Column, op.Digit);
            return true;
        }

        public static bool Execute(this SubgridOp<SubgridHLDefaultOp> op, ClassicSudokuScriptNode scriptNode)
        {
            var idx = Common.GetIdxFromRC(op.Row, op.Column);
            (scriptNode.GetSubgrid(idx.Item1, idx.Item2).GetNode("%AnswerLabel") as Label).LabelSettings = SubgridLabel_DefaultSettings;
            return false;
        }

        public static bool Execute(this SubgridOp<SubgridHLErrorOp> op, ClassicSudokuScriptNode scriptNode)
        {
            var idx = Common.GetIdxFromRC(op.Row, op.Column);
            (scriptNode.GetSubgrid(idx.Item1, idx.Item2).GetNode("%AnswerLabel") as Label).LabelSettings = SubgridLabel_ErrorSettings;
            return false;
        }
    }
}