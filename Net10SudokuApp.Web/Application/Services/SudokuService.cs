using Net10SudokuApp.Web.Application.Interfaces;
using Net10SudokuApp.Web.Domain.Models;
using System;
using System.Collections.Generic;

namespace Net10SudokuApp.Web.Application.Services
{
    public class SudokuService : ISudokuGenerator, ISudokuSolver, ISudokuValidator
    {
        private static readonly Random _rand = new();

        public SudokuPuzzle Generate(Difficulty difficulty)
        {
            var solved = GenerateSolvedBoard();
            var puzzle = solved.Clone();

            int clues = difficulty switch
            {
                Difficulty.Easy => 36,
                Difficulty.Medium => 30,
                Difficulty.Hard => 24,
                _ => 30
            };

            var positions = new List<(int r, int c)>();
            for (int r = 0; r < 9; r++)
                for (int c = 0; c < 9; c++)
                    positions.Add((r, c));

            for (int i = positions.Count - 1; i > 0; i--)
            {
                int j = _rand.Next(i + 1);
                var tmp = positions[i];
                positions[i] = positions[j];
                positions[j] = tmp;
            }

            int cellsToRemove = 81 - clues;
            int removed = 0;
            foreach (var (r, c) in positions)
            {
                if (removed >= cellsToRemove) break;

                int backup = puzzle.Grid[r, c];
                puzzle.Grid[r, c] = 0;

                var copy = puzzle.Clone();
                var solver = SolveCopy(copy);
                if (solver == null)
                {
                    puzzle.Grid[r, c] = backup;
                }
                else
                {
                    removed++;
                }
            }

            return puzzle;
        }

        private SudokuPuzzle GenerateSolvedBoard()
        {
            var puzzle = new SudokuPuzzle();
            FillGrid(0, 0, puzzle.Grid);
            return puzzle;
        }

        private bool FillGrid(int row, int col, int[,] grid)
        {
            if (row == 9) return true;
            int nextRow = col == 8 ? row + 1 : row;
            int nextCol = col == 8 ? 0 : col + 1;

            var numbers = new List<int>();
            for (int i = 1; i <= 9; i++) numbers.Add(i);
            for (int i = numbers.Count - 1; i > 0; i--)
            {
                int j = _rand.Next(i + 1);
                (numbers[i], numbers[j]) = (numbers[j], numbers[i]);
            }

            foreach (var num in numbers)
            {
                if (IsSafe(grid, row, col, num))
                {
                    grid[row, col] = num;
                    if (FillGrid(nextRow, nextCol, grid))
                        return true;
                    grid[row, col] = 0;
                }
            }
            return false;
        }

        private bool IsSafe(int[,] grid, int row, int col, int num)
        {
            for (int c = 0; c < 9; c++)
                if (grid[row, c] == num) return false;
            for (int r = 0; r < 9; r++)
                if (grid[r, col] == num) return false;
            int br = (row / 3) * 3, bc = (col / 3) * 3;
            for (int r = 0; r < 3; r++)
                for (int c = 0; c < 3; c++)
                    if (grid[br + r, bc + c] == num) return false;
            return true;
        }

        public bool Solve(SudokuPuzzle puzzle)
        {
            return SolveInternal(puzzle.Grid);
        }

        public SudokuPuzzle SolveCopy(SudokuPuzzle puzzle)
        {
            var copy = puzzle.Clone();
            if (SolveInternal(copy.Grid))
                return copy;
            return null;
        }

        private bool SolveInternal(int[,] grid)
        {
            for (int r = 0; r < 9; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    if (grid[r, c] == 0)
                    {
                        for (int n = 1; n <= 9; n++)
                        {
                            if (IsSafe(grid, r, c, n))
                            {
                                grid[r, c] = n;
                                if (SolveInternal(grid)) return true;
                                grid[r, c] = 0;
                            }
                        }
                        return false;
                    }
                }
            }
            return true;
        }

        public bool[,] Validate(SudokuPuzzle puzzle)
        {
            var conflicts = new bool[9, 9];
            var grid = puzzle.Grid;

            for (int r = 0; r < 9; r++)
            {
                var seen = new Dictionary<int, List<int>>();
                for (int c = 0; c < 9; c++)
                {
                    int v = grid[r, c];
                    if (v == 0) continue;
                    if (!seen.ContainsKey(v)) seen[v] = new List<int>();
                    seen[v].Add(c);
                }
                foreach (var kv in seen)
                {
                    if (kv.Value.Count > 1)
                    {
                        foreach (var c in kv.Value) conflicts[r, c] = true;
                    }
                }
            }

            for (int c = 0; c < 9; c++)
            {
                var seen = new Dictionary<int, List<int>>();
                for (int r = 0; r < 9; r++)
                {
                    int v = grid[r, c];
                    if (v == 0) continue;
                    if (!seen.ContainsKey(v)) seen[v] = new List<int>();
                    seen[v].Add(r);
                }
                foreach (var kv in seen)
                {
                    if (kv.Value.Count > 1)
                    {
                        foreach (var r in kv.Value) conflicts[r, c] = true;
                    }
                }
            }

            for (int br = 0; br < 3; br++)
            {
                for (int bc = 0; bc < 3; bc++)
                {
                    var seen = new Dictionary<int, List<(int r, int c)>>();
                    for (int r = 0; r < 3; r++)
                    {
                        for (int c = 0; c < 3; c++)
                        {
                            int rr = br * 3 + r;
                            int cc = bc * 3 + c;
                            int v = grid[rr, cc];
                            if (v == 0) continue;
                            if (!seen.ContainsKey(v)) seen[v] = new List<(int r, int c)>();
                            seen[v].Add((rr, cc));
                        }
                    }
                    foreach (var kv in seen)
                    {
                        if (kv.Value.Count > 1)
                        {
                            foreach (var pos in kv.Value) conflicts[pos.r, pos.c] = true;
                        }
                    }
                }
            }

            return conflicts;
        }
    }
}