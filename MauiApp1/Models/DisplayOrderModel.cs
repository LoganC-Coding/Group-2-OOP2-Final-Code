// Models/DisplayOrderModel.cs
using System.Collections.Generic;

namespace MauiApp1.Models
{
    public class DisplayOrderModel
    {
        public int OrderId { get; set; }
        public string OrderType { get; set; }
        public int? TableId { get; set; } // Nullable for Online/Take out
        public bool IsServed { get; set; } // Represents cooking/served status
        public List<DisplayOrderItemModel> Items { get; set; } = new List<DisplayOrderItemModel>();

        // Helper property for display text based on IsServed
        public string StatusText => IsServed ? "Ready/Served" : "To Cook";
        // Helper for visual differentiation (optional)
        public string StatusClass => IsServed ? "status-served" : "status-cooking";
    }
}