using Net10SudokuApp.Web.Domain.Models;

namespace Net10SudokuApp.Web.Application.Interfaces
{
    public interface ISudokuValidator
    {
        bool[,] Validate(SudokuPuzzle puzzle);
    }
}