using System;
using Sudoku.Sudoku;
using Godot;

public partial class BoardNode: Node
{
    public SudokuBase board = new ClassicSudoku();

    public void Generate()
    {
        board.Generate();
    }

    public bool RemoveOne()
    {
        return board.RemoveOne();
    }

    public void UpdateGrids(Node boardNode)
    {
        for (int i = 0; i < 9; ++ i)
        {
            for (int j = 0; j < 9; ++ j)
            {
                int i1 = i / 3 * 3 + j / 3;
                int j1 = i % 3 * 3 + j % 3;
                int num = board.board[i1, j1];
                (boardNode.GetChild(i).GetChild(0).GetChild(j) as Label).Text = num > 0 ? num.ToString() : "";
            }
        }
    }
}
