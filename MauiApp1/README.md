# Restaurant Management System (MauiApp1)

## Overview

This project is a simple Restaurant Management System built using .NET MAUI Blazor Hybrid. It aims to provide restaurant staff with a digital interface to manage table availability and track active customer orders efficiently.

## Problem Solved

This system addresses the need for a view of table occupancy and order statuses, reducing reliance on manual tracking methods, minimizing errors, and improving workflow for front-of-house and kitchen staff.

## Core Functionality

The application currently focuses on two main areas:

1.  **Table Management:**
    *   Displays a visual layout of main floor tables.
    *   Shows the current status of each table (Available, Reserved) based on database information.
    *   Allows staff to select an available table, typically as a precursor to creating a new order (includes confirmation).

2.  **Order Tracking:**
    *   Displays a list of currently *active* orders (those not yet fully completed/paid).
    *   Shows key details for each order: Order #, Type (Dine in, Online, Take out), Status (To Cook, Ready/Served), Table # (if applicable), and Items ordered.
    *   Provides functionality for staff to mark served orders as "Finished", removing them from the active display and updating the database.

## Key Features Implemented

*   **Table View (`TableSelectionPage.razor`):** Visual display of tables 1-11, color-coded by status. Click interaction with confirmation for available tables.
*   **Active Order View (`OrderDisplayPage.razor`):** Card-based display of orders fetched from the database that haven't been finalized (no transaction record). Includes status indicators and item lists.
*   **Finish Order Functionality:** Button on served orders to remove them from the active view and delete corresponding records from the database (Order, OrderItem, Transaction).
*   **Database Interaction:** Uses MySqlConnector to fetch and manipulate data from a MySQL database based on the defined ERD.

## Technology Stack

*   **Framework:** .NET MAUI Blazor Hybrid
*   **Language:** C#
*   **Database:** MySQL (using MySql.Data / MySqlConnector)
*   **UI:** HTML / CSS within Blazor components (.razor)

## Project Structure

*   **`Components/`**: Contains reusable UI components.
    *   `Layout/`: Defines the main application layout (`MainLayout.razor`, `NavMenu.razor`).
    *   `Pages/`: Contains the main routable pages/views of the application (`TableSelectionPage.razor`, `OrderDisplayPage.razor`, etc.).
*   **`Models/`**: Defines C# classes representing data structures (e.g., `TableModel`, `DisplayOrderModel`, `Order`).
*   **`Services/`**: Contains classes responsible for business logic and data access (e.g., `TableService`, `OrderService`). *Note: Some data access logic is currently direct within page components for simplicity.*
*   **`wwwroot/`**: Holds static web assets like CSS and JavaScript.
*   **`MauiProgram.cs`**: Application startup configuration, including service registration (Dependency Injection).
*   **`appsettings.json`**: Configuration file (e.g., for database connection strings).

## Getting Started

1.  **Prerequisites:**
    *   .NET SDK (compatible version for MAUI)
    *   MySQL Server instance.
2.  **Database Setup:**
    *   Ensure the database schema matches the provided SQL script.
    *   Project has an initialize database function in DatabaseInitializationService under Service Folder
    *   Populate with initial data if needed.
3.  **Configuration:**
    *   Default database configuration:
        ```
        Server: localhost
        Port: 3306
        Database: test2db
        User: root
        Password: password
        ```
    *   Update directly within the page components if needed to change 
4.  **Run:** Build and run the project using Visual Studio .

---