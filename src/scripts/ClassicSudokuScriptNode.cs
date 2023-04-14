using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Sudoku.Executors;
using SudokuLib;
using SudokuLib.Strategy.Classic;
using SudokuLib.Strategy.Op;

public partial class ClassicSudokuScriptNode: Node
{
    public ClassicSudoku sudoku = new ClassicSudoku(42);
    private Dictionary<string, Func<ClassicSudoku, int, int, IEnumerable<OpBase>>[]> strategyOnSubgridPresets = new()
    {
        { "select_subgrid", new Func<ClassicSudoku, int, int, IEnumerable<OpBase>>[]
        {
            HighlightSameDigit.Instance.ExecuteOnSubgrid,
            HighlightColumn.Instance.ExecuteOnSubgrid,
            HighlightRow.Instance.ExecuteOnSubgrid,
        } },
        { "deselect_subgrid", new Func<ClassicSudoku, int, int, IEnumerable<OpBase>>[]
        {
            UndoHighlightSameDigit.Instance.ExecuteOnSubgrid,
            UndoHighlightColumn.Instance.ExecuteOnSubgrid,
            UndoHighlightRow.Instance.ExecuteOnSubgrid,
        } },
    };
    private Dictionary<string, Func<ClassicSudoku, int, int, int, IEnumerable<OpBase>>[]> strategyOnDigitPresets = new()
    {
        { "fill_with", new Func<ClassicSudoku, int, int, int, IEnumerable<OpBase>>[]
        {
            FillDigit.Instance.ExecuteOnDigit,
            BasicEliminate.Instance.ExecuteOnDigit,
        } },
        { "eliminate_candidate", new Func<ClassicSudoku, int, int, int, IEnumerable<OpBase>>[]
        {
            (_, r, c, d) => new OpBase[] { new EliminateOp(r, c, d) },
        } },
        { "uneliminate_candidate", new Func<ClassicSudoku, int, int, int, IEnumerable<OpBase>>[]
        {
            (_, r, c, d) => new OpBase[] { new UnEliminateOp(r, c, d) },
        } },
    };
    private Dictionary<string, Func<ClassicSudoku, IEnumerable<OpBase>>[]> strategyOnBoardPresets = new()
    {
        { "restart", new Func<ClassicSudoku, IEnumerable<OpBase>>[]
        {
            UndoHighlightRow.Instance.ExecuteOnBoard,
            UndoHighlightSameDigit.Instance.ExecuteOnBoard,
            _ => from r in Enumerable.Range(0, 9) from c in Enumerable.Range(0, 9) from d in Enumerable.Range(1, 9) select new UnEliminateOp(r, c, d) as OpBase,
            FillDigit.Instance.ExecuteOnEverySubgridDigit,
            BasicEliminate.Instance.ExecuteOnEverySubgridDigit,
        } }
    };

    public Node GetGrid(int idx) => GetNode(String.Format("%Grid{0}", idx));
    public Node GetSubgrid(int i, int j) => GetGrid(i).GetNode(String.Format("%Subgrid{0}", j));
    public Node GetDigitNode(int i, int j, int d) => GetSubgrid(i, j).GetNode(String.Format("%Digit{0}", d));

    public void Eliminate() => BasicEliminate.Instance.ExecuteOnEverySubgridDigit(sudoku);
    public void Restart()
    {
        sudoku.Generate();
        ExecuteStrategiesOnBoard("restart");
    }

    public bool CheckCorrect(int idx_grid, int idx_subgrid, int digit)
    {
        (int row, int column) = Common.GetRCFromIdx(idx_grid, idx_subgrid);
        return sudoku.answer[row, column] == digit;
    }

    public void ExecuteStrategiesOnSubgrid(int idx_grid, int idx_subgrid, string presetName)
    {
        (int row, int column) = Common.GetRCFromIdx(idx_grid, idx_subgrid);
        if (strategyOnSubgridPresets.TryGetValue(presetName, out var strategyFuncList))
            foreach (var strategyFunc in strategyFuncList)
                foreach (var op in strategyFunc(sudoku, row, column))
                    op.Execute(this);
    }

    public void ExecuteStrategiesOnDigit(int idx_grid, int idx_subgrid, int digit, string presetName)
    {
        (int row, int column) = Common.GetRCFromIdx(idx_grid, idx_subgrid);
        if (strategyOnDigitPresets.TryGetValue(presetName, out var strategyFuncList))
            foreach (var strategyFunc in strategyFuncList)
                foreach (var op in strategyFunc(sudoku, row, column, digit))
                    op.Execute(this);
    }

    public void ExecuteStrategiesOnBoard(string presetName)
    {
        if (strategyOnBoardPresets.TryGetValue(presetName, out var strategyFuncList))
            foreach (var strategyFunc in strategyFuncList)
                foreach (var op in strategyFunc(sudoku))
                    op.Execute(this);
    }
}
