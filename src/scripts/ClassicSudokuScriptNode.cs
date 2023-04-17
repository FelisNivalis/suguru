using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;
using Sudoku.Executors;
using SudokuLib;
using SudokuLib.Strategy;
using SudokuLib.Strategy.Classic;
using SudokuLib.Strategy.Op;

public partial class ClassicSudokuScriptNode: Node
{
    public ClassicSudoku sudoku = new ClassicSudoku(-1);
    private Dictionary<string, BaseStrategy<ClassicSudoku>> strategyPresets = new()
    {
        { "select_subgrid", UISelectSubgrid.Instance },
        { "unselect_subgrid", UIUnselectSubgrid.Instance },
        { "unfill_subgrid", UIUnfillSubgrid.Instance },
        { "fill_with", UIFillDigit.Instance },
        { "execute_naked_single", NakedSingle.Instance },
        { "execute_hidden_single", HiddenSingle.Instance },
        { "execute_locked_candidates_type1", LockedCandidatesType1.Instance },
        { "eliminate_candidate", SingleDigitOpStrategy<DigitOp<EliminateOp>, ClassicSudoku>.Instance },
        { "uneliminate_candidate", SingleDigitOpStrategy<DigitOp<UnEliminateOp>, ClassicSudoku>.Instance },
        { "restart", RestartAction.Instance },
    };

    public Node GetGrid(int idx) => GetNode(String.Format("%Grid{0}", idx));
    public Node GetSubgrid(int i, int j) => GetGrid(i).GetNode(String.Format("%Subgrid{0}", j));
    public Node GetDigitNode(int i, int j, int d) => GetSubgrid(i, j).GetNode(String.Format("%Digit{0}", d));

    public void Eliminate() => BasicEliminate.Instance.ExecuteOnEverySubgridDigit(sudoku);
    public void Restart()
    {
        while (true)
        {
            sudoku.Generate();
            ExecuteStrategiesOnBoard("restart");
            while (true)
            {
                bool flag = false;
                foreach(var strategy in new BaseStrategy<ClassicSudoku>[]{ NakedSingle.Instance, HiddenSingle.Instance, LockedCandidatesType1.Instance })
                {
                    var ops = strategy.ExecuteOnBoard(sudoku);
                    Debug.Print(String.Format("strategy {0}", strategy.GetType().FullName));
                    if (ops.Execute(this)) { flag = true; }
                }
                if (!flag) break;
            }
            if (!sudoku.Solved()) break;
        }
    }

    public bool CheckCorrect(int idx_grid, int idx_subgrid, int digit)
    {
        (int row, int column) = Common.GetRCFromIdx(idx_grid, idx_subgrid);
        return sudoku.answer[row, column] == digit;
    }

    void ExecuteStrategy(Func<BaseStrategy<ClassicSudoku>, OpBase> strategyMethod, string presetName)
    {
        if (strategyPresets.TryGetValue(presetName, out var strategy))
            strategyMethod(strategy).Execute(this);
    }

    public void ExecuteStrategiesOnSubgrid(int idx_grid, int idx_subgrid, string presetName)
    {
        (int row, int column) = Common.GetRCFromIdx(idx_grid, idx_subgrid);
        ExecuteStrategy(strategy => strategy.ExecuteOnSubgrid(sudoku, row, column), presetName);
    }

    public void ExecuteStrategiesOnDigit(int idx_grid, int idx_subgrid, int digit, string presetName)
    {
        (int row, int column) = Common.GetRCFromIdx(idx_grid, idx_subgrid);
        ExecuteStrategy(strategy => strategy.ExecuteOnDigit(sudoku, row, column, digit), presetName);
    }

    public void ExecuteStrategiesOnBoard(string presetName)
    {
        ExecuteStrategy(strategy => strategy.ExecuteOnBoard(sudoku), presetName);
    }
}
