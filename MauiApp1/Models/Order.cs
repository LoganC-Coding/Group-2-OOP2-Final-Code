using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Models;

public class Order
{
    public int OrderId { get; set; }
    public string CustomerName { get; set; }
    public List<OrderItem> Items { get; set; } = new();
    public DateTime OrderTime { get; set; }

    public decimal TotalPrice => Items.Sum(i => i.Quantity * i.Price);
}

public class OrderItem
{
    public string ItemName { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}