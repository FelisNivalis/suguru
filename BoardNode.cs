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
                Node subgridNode = boardNode.GetNode(String.Format("%Grid{0}", i + 1)).GetNode(String.Format("%Subgrid{0}", j + 1));
                if (num > 0)
                {
                    (subgridNode.GetNode("Options") as CanvasItem).Visible = false;
                    (subgridNode.GetNode("Number") as CanvasItem).Visible = true;
                    (subgridNode.GetNode("Number").GetNode("Label") as Label).Text = num.ToString();
                }
                else
                {
                    (subgridNode.GetNode("Options") as CanvasItem).Visible = true;
                    (subgridNode.GetNode("Number") as CanvasItem).Visible = false;
                    (subgridNode.GetNode("Number").GetNode("Label") as Label).Text = "";
                }
            }
        }
    }
}
