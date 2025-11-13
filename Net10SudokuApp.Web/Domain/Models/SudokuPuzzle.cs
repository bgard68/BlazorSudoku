namespace Net10SudokuApp.Web.Domain.Models
{
    public class SudokuPuzzle
    {
        public int[,] Grid { get; init; } = new int[9, 9];

        public SudokuPuzzle Clone()
        {
            var clone = new SudokuPuzzle();
            for (int r = 0; r < 9; r++)
                for (int c = 0; c < 9; c++)
                    clone.Grid[r, c] = Grid[r, c];
            return clone;
        }
    }
}