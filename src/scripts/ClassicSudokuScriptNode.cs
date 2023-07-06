using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;
using Sudoku.Executors;
using Sudoku.Strategy.Classic;
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
        { "unfill_subgrid", UnfillSubgrid.Instance },
        { "fill_with", FillDigit.Instance },
        { "execute_naked_single", NakedSingle.Instance },
        { "execute_hidden_single", HiddenSingle.Instance },
        { "execute_locked_candidates_type1", LockedCandidatesType1.Instance },
        { "execute_locked_candidates_type2", LockedCandidatesType2.Instance },
        { "execute_locked_candidates", new StrategyList<ClassicSudoku> { LockedCandidatesType1.Instance, LockedCandidatesType2.Instance } },
        { "execute_naked_pair", NakedPair.Instance },
        { "execute_naked_subsets", new StrategyList<ClassicSudoku> { NakedPair.Instance, NakedTuple.Instance, NakedQuadruple.Instance } },
        { "execute_hidden_subsets", new StrategyList<ClassicSudoku> { HiddenPair.Instance, HiddenTuple.Instance, HiddenQuadruple.Instance } },
        { "eliminate_candidate", SingleDigitOpStrategy<DigitOp<EliminateOp>, ClassicSudoku>.Instance },
        { "uneliminate_candidate", SingleDigitOpStrategy<DigitOp<UnEliminateOp>, ClassicSudoku>.Instance },
        { "restart", RestartAction.Instance },
    };

    public Node GetSubgrid(int i, int j) => GetNode(String.Format("%Grid{0}", i)).GetNode(String.Format("%Subgrid{0}", j));
    public Node GetDigitNode(int i, int j, int d) => GetSubgrid(i, j).GetNode(String.Format("%Digit{0}", d));

    public void Eliminate() => BasicEliminate.Instance.ExecuteOnEverySubgridDigit(sudoku);
    public void Restart()
    {
        while (true)
        {
            var startTime = DateTime.Now;
            sudoku.Generate();
            Debug.Print(String.Format("Generate {0}", (DateTime.Now - startTime).TotalMilliseconds));
            ExecuteStrategiesOnBoard("restart");
            ClassicSudoku _sudoku = new(sudoku);
            startTime = DateTime.Now;
            while ((
                from strategy in new BaseStrategy<ClassicSudoku>[]
                    { NakedSingle.Instance, HiddenSingle.Instance,
                      LockedCandidatesType1.Instance, LockedCandidatesType2.Instance,
                      NakedPair.Instance, HiddenPair.Instance, NakedTuple.Instance,
                      HiddenTuple.Instance, NakedQuadruple.Instance, HiddenQuadruple.Instance }
                select strategy.ExecuteOnBoard(_sudoku).Execute(_sudoku)
            ).Any(b => b)) ;
            Debug.Print(String.Format("Solve    {0}", (DateTime.Now - startTime).TotalMilliseconds));
            if (!_sudoku.Solved())
                break;
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
