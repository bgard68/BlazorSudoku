using Net10SudokuApp.Web.Domain.Models;

namespace Net10SudokuApp.Web.Application.Interfaces
{
    public interface ISudokuGenerator
    {
        SudokuPuzzle Generate(Difficulty difficulty);
    }

    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }
}