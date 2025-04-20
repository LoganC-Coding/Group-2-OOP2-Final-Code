/* Author: Omar Kasan
 * Date: 2025-04-19
 *
 * Program: OrderService (Service Class)
 * Description: Provides a method to save an Order and its associated items to a MySQL database within a single transaction.
 *
 * Inputs:
 * – Order object containing CustomerName, OrderTime, and a list of OrderItemModel instances.
 * – Static connection string for MySQL (server, user, password, database, port).
 *
 * Processing:
 * – Opens a MySqlConnection and begins a transaction.
 * – Inserts a new record into the orders table and retrieves the generated OrderId.
 * – Iterates through each item in order.Items, inserting a record into order_items with the OrderId, item name, quantity, and price.
 * – Commits the transaction if all inserts succeed; rolls back the transaction on any exception.
 *
 * Outputs:
 * – Persists the Order and its items to the database.
 * – Throws an exception and rolls back if any database operation fails, preserving data integrity.
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;
using MauiApp1.Models;


namespace MauiApp1.Services;

public static class OrderService
{
    private static readonly string connectionString =
        "server=localhost;user=root;password=password;database=test2db;port=3306;";

    public static void SaveOrder(Order order)
    {
        using var conn = new MySqlConnection(connectionString);
        conn.Open();
        using var transaction = conn.BeginTransaction();

        try
        {
            var cmdOrder = new MySqlCommand(
                "INSERT INTO orders (customer_name, order_time) VALUES (@customer_name, @order_time); SELECT LAST_INSERT_ID();",
                conn, transaction);

            cmdOrder.Parameters.AddWithValue("@customer_name", order.CustomerName);
            cmdOrder.Parameters.AddWithValue("@order_time", order.OrderTime);

            int orderId = Convert.ToInt32(cmdOrder.ExecuteScalar());

            foreach (var item in order.Items)
            {
                var cmdItem = new MySqlCommand(
                    "INSERT INTO order_items (order_id, item_name, quantity, price) VALUES (@order_id, @item_name, @quantity, @price);",
                    conn, transaction);

                cmdItem.Parameters.AddWithValue("@order_id", orderId);
                cmdItem.Parameters.AddWithValue("@item_name", item.ItemName);
                cmdItem.Parameters.AddWithValue("@quantity", item.Quantity);
                cmdItem.Parameters.AddWithValue("@price", item.Price);
                cmdItem.ExecuteNonQuery();
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}