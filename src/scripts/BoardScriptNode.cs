using System;
using SudokuLib;
using Godot;

public partial class BoardScriptNode: Node
{
    public SudokuBase sudoku = new ClassicSudoku();
    public Node boardNode;

    public void Generate()
    {
        sudoku.Generate();
    }

    public void InitGrids()
    {
        for (int i = 0; i < 9; ++ i)
        {
            for (int j = 0; j < 9; ++ j)
            {
                int i1 = i / 3 * 3 + j / 3;
                int j1 = i % 3 * 3 + j % 3;
                int num = sudoku.board[i1, j1];
                Node subgridNode = boardNode.GetNode(String.Format("%Grid{0}", i + 1)).GetNode(String.Format("%Subgrid{0}", j + 1));
                subgridNode.EmitSignal("s_fill_with", num);
            }
        }
    }

    public void Eliminate()
    {
        for (int i = 0; i < 9; ++i)
        {
            for (int j = 0; j < 9; ++j)
            {
                int i1 = i / 3 * 3 + j / 3;
                int j1 = i % 3 * 3 + j % 3;
                int num = sudoku.board[i1, j1];
                Node subgridNode = boardNode.GetNode(String.Format("%Grid{0}", i + 1)).GetNode(String.Format("%Subgrid{0}", j + 1));
                foreach (int digit in sudoku.EnumerateInvalid(sudoku.board, i1, j1))
                {
                    subgridNode.EmitSignal("s_eliminate", digit);
                }
            }
        }
    }

    public void FillWith(int x, int y, int v)
    {
        sudoku.board[x, y] = v;
    }

    public bool CheckCorrect(int x, int y, int v)
    {
        return v == sudoku.answer[x, y];
    }
}
