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

        public static void Execute(this OpBase op, ClassicSudokuScriptNode scriptNode)
        {
            Execute(op as dynamic, scriptNode);
        }

        private static void SetPanelStyle(this Node node, Resource style)
        {
            node.Set("theme_override_styles/panel", style);
        }

        public static void Execute(this OpList op, ClassicSudokuScriptNode scriptNode)
        {
            foreach (OpBase _op in op.ops) Execute(_op as dynamic, scriptNode);
        }

        public static void Execute(this DigitOp<DigitSelectOp> op, ClassicSudokuScriptNode scriptNode)
        {
            var idx = Common.GetIdxFromRC(op.Row, op.Column);
            scriptNode.GetDigitNode(idx.Item1, idx.Item2, op.Digit).GetNode("%BgCircle")?.SetPanelStyle(DigitBgCircle_SameNumberStyle);
        }

        public static void Execute(this SubgridOp<SubgridSelectOp> op, ClassicSudokuScriptNode scriptNode)
        {
            var idx = Common.GetIdxFromRC(op.Row, op.Column);
            scriptNode.GetSubgrid(idx.Item1, idx.Item2).GetNode("%Bg")?.SetPanelStyle(SubgridBg_SameRCSelectedStyle);
        }

        public static void Execute(this DigitOp<DigitUnselectOp> op, ClassicSudokuScriptNode scriptNode)
        {
            var idx = Common.GetIdxFromRC(op.Row, op.Column);
            scriptNode.GetDigitNode(idx.Item1, idx.Item2, op.Digit).GetNode("%BgCircle")?.SetPanelStyle(EmptyBoxStyle);
        }

        public static void Execute(this SubgridOp<SubgridUnselectOp> op, ClassicSudokuScriptNode scriptNode)
        {
            var idx = Common.GetIdxFromRC(op.Row, op.Column);
            scriptNode.GetSubgrid(idx.Item1, idx.Item2).GetNode("%Bg")?.SetPanelStyle(EmptyBoxStyle);
        }

        public static void Execute(this DigitOp<FillOp> op, ClassicSudokuScriptNode scriptNode)
        {
            //if (scriptNode.sudoku.board[op.Row, op.Column] > 0)
            var idx = Common.GetIdxFromRC(op.Row, op.Column);
            scriptNode.sudoku.board[op.Row, op.Column] = op.Digit;
            scriptNode.GetSubgrid(idx.Item1, idx.Item2).Call("fill_with", new Variant[] { op.Digit });
        }

        public static void Execute(this DigitOp<EliminateOp> op, ClassicSudokuScriptNode scriptNode)
        {
            var idx = Common.GetIdxFromRC(op.Row, op.Column);
            scriptNode.sudoku.candidates.Unset(op.Row, op.Column, op.Digit);
            scriptNode.GetDigitNode(idx.Item1, idx.Item2, op.Digit).Call("hide_text", new Variant[] { });
        }

        public static void Execute(this DigitOp<UnEliminateOp> op, ClassicSudokuScriptNode scriptNode)
        {
            var idx = Common.GetIdxFromRC(op.Row, op.Column);
            scriptNode.sudoku.candidates.Set(op.Row, op.Column, op.Digit);
            scriptNode.GetDigitNode(idx.Item1, idx.Item2, op.Digit).Call("show_text", new Variant[] { });
        }

        public static void Execute(this SubgridOp<SubgridHLDefaultOp> op, ClassicSudokuScriptNode scriptNode)
        {
            var idx = Common.GetIdxFromRC(op.Row, op.Column);
            (scriptNode.GetSubgrid(idx.Item1, idx.Item2).GetNode("%AnswerLabel") as Label).LabelSettings = SubgridLabel_DefaultSettings;
        }

        public static void Execute(this SubgridOp<SubgridHLErrorOp> op, ClassicSudokuScriptNode scriptNode)
        {
            var idx = Common.GetIdxFromRC(op.Row, op.Column);
            (scriptNode.GetSubgrid(idx.Item1, idx.Item2).GetNode("%AnswerLabel") as Label).LabelSettings = SubgridLabel_ErrorSettings;
        }
    }
}