// See https://aka.ms/new-console-template for more information
using SudokuLib;

Console.WriteLine("Hello, World!");
var sudoku = new ClassicSudoku(-1);
sudoku.Generate();
for (int i = 0; i < 9; i++)
{
    for (int j = 0; j < 9; j++)
        Console.Write(sudoku.board[i, j].ToString());
    Console.WriteLine();
}