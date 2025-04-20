/*
 * Author:    Felix Gabriel Montanez
 * Date:      2025‑04‑18
 *
 * Program:   TableService (MauiApp1.Services)
 * Description:
 *   This service class retrieves “main‑floor” table data from a MySQL database.
 *   It reads a configured connection string via IConfiguration, opens an asynchronous MySqlConnection,
 *   executes a SELECT query, and processes each returned row into a TableModel object.
 *   The method handles both database‑specific (MySqlException) and generic exceptions, logs any errors,
 *   and returns the results as a List<TableModel> (or an empty list if there’s an error or missing config).
 *
 * Inputs:
 *   – IConfiguration (injected) for retrieving the "DefaultConnection" string.
 *   – SQL Query to select table data from the database.
 *
 * Processing:
 *   – Null‑checks the connection string.
 *   – Opens and closes the database connection asynchronously.
 *   – Reads each row with ExecuteReaderAsync(), converts raw values to int/bool,
 *     and builds a collection of TableModel instances.
 *   – Catches/logs exceptions to avoid crashing the app.
 *
 * Outputs:
 *   – Returns a List<TableModel> containing the table data.
 *   – On error or misconfiguration, returns an empty list.
 */

using Microsoft.Extensions.Configuration;
using MauiApp1.Models;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Diagnostics;

namespace MauiApp1.Services
{
    public class TableService
    {
        private readonly IConfiguration _configuration;

        public TableService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<TableModel>> GetMainFloorTablesAsync()
        {
            var tables = new List<TableModel>();
            string sql = "SELECT table_id, seats, is_reserved FROM `Table` WHERE table_id BETWEEN 1 AND 11 ORDER BY table_id;";

            // --- SOLUTION for CS8600 implication ---
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                Debug.WriteLine("Error: Connection string 'DefaultConnection' not found or is empty in configuration.");
                return new List<TableModel>(); // Return empty for simplicity
            }
            // --- End Null Check ---

            try
            {
                await using var connection = new MySqlConnection(connectionString);
                await connection.OpenAsync();

                await using var command = new MySqlCommand(sql, connection);
                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    tables.Add(new TableModel
                    {
                        TableId = Convert.ToInt32(reader["table_id"]),
                        Seats = Convert.ToInt32(reader["seats"]),
                        IsReserved = Convert.ToBoolean(reader["is_reserved"])
                    });
                }
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine($"MySQL Error fetching tables: {ex.Message}");
                return new List<TableModel>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Generic Error fetching tables: {ex.Message}");
                return new List<TableModel>();
            }

            return tables;
        }
    }
}
