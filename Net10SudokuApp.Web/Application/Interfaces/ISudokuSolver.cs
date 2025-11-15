using Net10SudokuApp.Web.Domain.Models;

namespace Net10SudokuApp.Web.Application.Interfaces
{
    public interface ISudokuSolver
    {
        bool Solve(SudokuPuzzle puzzle);
        SudokuPuzzle? SolveCopy(SudokuPuzzle puzzle);
    }
}