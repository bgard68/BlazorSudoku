namespace Net10SudokuApp.Web.Domain.Models
{
    public class Cell
    {
        public int Row { get; init; }
        public int Col { get; init; }
        public int Value { get; set; }
        public bool IsFixed { get; set; }
        public bool IsSelected { get; set; }
        public bool IsConflict { get; set; }
        public bool JustPlaced { get; set; }
    }
}