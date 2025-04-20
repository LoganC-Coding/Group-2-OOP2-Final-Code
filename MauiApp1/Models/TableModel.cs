namespace MauiApp1.Models // Ensure namespace matches your project
{
    public class TableModel
    {
        public int TableId { get; set; }
        public int Seats { get; set; } // Keep even if not displayed yet
        public bool IsReserved { get; set; } // Use bool in C#
    }
}