
/* Author: Felix Gabriel Montanez
 * Date: 2025-04-19
 *
 * Program: DatabaseInitializationService (Service Class)
 * Description: Executes a comprehensive SQL script to drop existing tables, recreate schema, and seed sample data in a MySQL database.
 *
 * Inputs:
 * – Hardcoded connection details: Server, Port, Database, User, Password.
 * – Multi-statement SQL initialization script containing DDL (DROP/CREATE) and DML (INSERT) commands.
 *
 * Processing:
 * – Constructs a MySQL connection string.
 * – Opens a database connection and logs connection status.
 * – Executes the entire SQL script via ExecuteNonQuery.
 * – Logs result code and success message.
 * – Catches MySqlException to log detailed DB errors and rethrow wrapped exceptions.
 * – Catches general Exception to log and rethrow wrapped exceptions.
 *
 * Outputs:
 * – Database schema reset: drops tables (`Transaction`, `OrderItem`, `Order`, `Menu`, `Table`), recreates them with constraints.
 * – Inserts sample menu, table, order, order item, and transaction data.
 * – Throws exceptions on failure to halt application startup and preserve integrity.
 */


using MySql.Data.MySqlClient;
using System;
using System.Diagnostics; // For Debug.WriteLine

namespace MauiApp1.Services // Make sure this namespace matches your project structure
{
    public class DatabaseInitializationService
    {
        // Hardcoded Connection Details
        private const string Server = "localhost";
        private const string Port = "3306";
        private const string Database = "test2db"; // Target database
        private const string User = "root";
        private const string Password = "password"; // 

        // The complete MySQL setup script
        // Using a verbatim string literal @"" for multi-line readability
        private static readonly string _initializeDbScript = @"
-- Disable checks for clean drop/create
SET FOREIGN_KEY_CHECKS=0;

-- Drop tables if they exist (Reverse order of creation/dependency)
DROP TABLE IF EXISTS `Transaction`;
DROP TABLE IF EXISTS `OrderItem`;
DROP TABLE IF EXISTS `Order`;
DROP TABLE IF EXISTS `Menu`;
DROP TABLE IF EXISTS `Table`;

-- Enable checks back
SET FOREIGN_KEY_CHECKS=1;

-- Create Tables (MySQL Syntax) --

CREATE TABLE `Menu` (
    `item_id` INT PRIMARY KEY NOT NULL,
    `name` VARCHAR(100) NOT NULL,
    `price` DECIMAL(10, 2) NOT NULL,
    `inventory` INT NOT NULL,
    `item_type` VARCHAR(20) NOT NULL CHECK (`item_type` IN ('Regular', 'Beverage', 'Seasonal')),
    `is_alcoholic` TINYINT(1) NULL,
    `season` VARCHAR(50) NULL,
    `valid_until` DATE NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `Table` (
    `table_id` INT PRIMARY KEY NOT NULL,
    `seats` INT NOT NULL,
    `is_reserved` TINYINT(1) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `Order` (
    `order_id` INT PRIMARY KEY NOT NULL,
    `order_type` VARCHAR(20) NOT NULL CHECK (`order_type` IN ('Online', 'Take out', 'Dine in')),
    `table_id` INT NULL,
    `is_served` TINYINT(1) NOT NULL DEFAULT 0,
    FOREIGN KEY (`table_id`) REFERENCES `Table`(`table_id`),
    CHECK ( (`order_type` IN ('Online', 'Take out') AND `table_id` IS NULL) OR (`order_type` = 'Dine in') )
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `OrderItem` (
    `order_id` INT NOT NULL,
    `item_id` INT NOT NULL,
    `quantity` INT NOT NULL,
    PRIMARY KEY (`order_id`, `item_id`),
    FOREIGN KEY (`order_id`) REFERENCES `Order`(`order_id`),
    FOREIGN KEY (`item_id`) REFERENCES `Menu`(`item_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `Transaction` (
    `transaction_id` INT PRIMARY KEY NOT NULL,
    `date` TIMESTAMP NOT NULL,
    `order_id` INT NOT NULL UNIQUE,
    `transaction_type` VARCHAR(20) NOT NULL CHECK (`transaction_type` IN ('DineIn', 'Online', 'TakeOut')),
    `address` VARCHAR(255) NULL,
    `delivery_fee` DECIMAL(5, 2) NULL,
    `pickup_time` VARCHAR(20) NULL,
    FOREIGN KEY (`order_id`) REFERENCES `Order`(`order_id`),
    CHECK (
        (`transaction_type` = 'DineIn' AND `address` IS NULL AND `delivery_fee` IS NULL AND `pickup_time` IS NULL) OR
        (`transaction_type` = 'Online' AND `address` IS NOT NULL AND `delivery_fee` IS NOT NULL AND `pickup_time` IS NULL) OR
        (`transaction_type` = 'TakeOut' AND `address` IS NULL AND `delivery_fee` IS NULL AND `pickup_time` IS NOT NULL)
    )
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Insert Sample Data --

INSERT INTO `Menu` (`item_id`, `name`, `price`, `inventory`, `item_type`, `is_alcoholic`, `season`, `valid_until`) VALUES
(1, 'Cheeseburger', 9.99, 50, 'Regular', NULL, NULL, NULL),
(2, 'Fries', 3.49, 100, 'Regular', NULL, NULL, NULL),
(3, 'Soda', 1.99, 200, 'Beverage', 0, NULL, NULL),
(4, 'Craft Beer', 6.50, 75, 'Beverage', 1, NULL, NULL),
(5, 'Pumpkin Spice Latte', 5.50, 40, 'Seasonal', 0, 'Fall', '2023-11-30'),
(6, 'Caesar Salad', 8.50, 30, 'Regular', NULL, NULL, NULL),
(7, 'Iced Tea', 2.29, 150, 'Beverage', 0, NULL, NULL),
(8, 'Winter Stew', 12.95, 25, 'Seasonal', NULL, 'Winter', '2024-03-15');

INSERT INTO `Table` (`table_id`, `seats`, `is_reserved`) VALUES
(1, 4, 1), (2, 2, 0), (3, 6, 1), (4, 4, 1);

INSERT INTO `Order` (`order_id`, `order_type`, `table_id`, `is_served`) VALUES
(101, 'Dine in', 1, 1), (102, 'Take out', NULL, 1), (103, 'Online', NULL, 1),
(104, 'Dine in', 3, 1), (105, 'Online', NULL, 0), (106, 'Dine in', 4, 0);

INSERT INTO `OrderItem` (`order_id`, `item_id`, `quantity`) VALUES
(101, 1, 1), (101, 2, 1), (101, 4, 1), (102, 6, 1), (102, 7, 1),
(103, 1, 2), (103, 3, 2), (104, 8, 1), (104, 3, 1), (105, 5, 1),
(106, 1, 1), (106, 7, 1);

INSERT INTO `Transaction` (`transaction_id`, `date`, `order_id`, `transaction_type`, `address`, `delivery_fee`, `pickup_time`) VALUES
(1001, '2023-10-27 12:15:00', 101, 'DineIn', NULL, NULL, NULL),
(1002, '2023-10-27 12:35:10', 102, 'TakeOut', NULL, NULL, '12:50 PM'),
(1003, '2023-10-27 13:05:00', 103, 'Online', '123 Main St, Anytown', 3.99, NULL);
"; // End of SQL script string

        // Synchronous method to run the initialization
        public void InitializeDatabase()
        {
            string connectionString = $"Server={Server};Port={Port};Database={Database};Uid={User};Pwd={Password};";
            Debug.WriteLine("Attempting to initialize database...");
            Debug.WriteLine($"Using Connection String: Server={Server};Port={Port};Database={Database};Uid={User};Pwd=*****;"); // Mask password in logs

            try
            {
                // Using statement ensures connection is disposed
                using var connection = new MySqlConnection(connectionString);
                connection.Open(); // Open connection synchronously
                Debug.WriteLine("Database connection opened.");

                // Using statement ensures command is disposed
                using var command = new MySqlCommand(_initializeDbScript, connection);

                // Execute the entire script
                // ExecuteNonQuery is suitable for scripts containing DDL (CREATE/DROP) and DML (INSERT)
                int result = command.ExecuteNonQuery(); // Execute synchronously
                Debug.WriteLine($"Database initialization script executed. Result code (may not indicate rows affected for multi-statement): {result}");
                Debug.WriteLine("Database initialized successfully!");
            }
            catch (MySqlException ex)
            {
                // Log detailed MySQL error
                Debug.WriteLine($"FATAL: MySQL error during database initialization: {ex.Message}\nError Code: {ex.Number}\nStackTrace: {ex.StackTrace}");
                // Rethrow the exception to potentially halt app startup
                throw new Exception($"Failed to initialize database due to MySQL error. See logs for details. Message: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                // Log general error
                Debug.WriteLine($"FATAL: General error during database initialization: {ex.Message}\nStackTrace: {ex.StackTrace}");
                // Rethrow the exception
                throw new Exception($"Failed to initialize database. See logs for details. Message: {ex.Message}", ex);
            }
            // Connection is automatically closed by the 'using' statement here
        }
    }
}