// Add necessary using statements at the top
using MauiApp1.Services;
using System.Diagnostics;
using Microsoft.Extensions.Configuration; // Keep if you still use it elsewhere
using MauiApp1; // Your root namespace for App class

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
#endif

        // --- Configuration for appsettings.json (If still needed for other services) ---
        var assembly = System.Reflection.Assembly.GetExecutingAssembly();
        string resourceName = "MauiApp1.appsettings.json";
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream != null)
        {
            var config = new ConfigurationBuilder().AddJsonStream(stream).Build();
            builder.Configuration.AddConfiguration(config);
        }
        else
        {
            Debug.WriteLine($"Warning: Embedded resource '{resourceName}' not found.");
        }
        // --- End Configuration ---

        // Register your other services (if any)
        // builder.Services.AddTransient<TableService>(); // Keep if you still use TableService with IConfiguration
        // builder.Services.AddTransient<OrderService>(); // Keep if you use OrderService

        // --- ADDED: Register the Database Initialization Service ---
        builder.Services.AddSingleton<DatabaseInitializationService>();

        // Build the app host
        var app = builder.Build();

        // --- ADDED: Invoke Database Initialization ---
        try
        {
            Debug.WriteLine("Getting DatabaseInitializationService...");
            // Get the service from the fully built app's service provider
            var dbInitializer = app.Services.GetRequiredService<DatabaseInitializationService>();

            Debug.WriteLine("Calling InitializeDatabase...");
            dbInitializer.InitializeDatabase(); // Call the synchronous method
            Debug.WriteLine("InitializeDatabase call completed.");
        }
        catch (Exception ex)
        {
            // Catch exceptions specifically from the initialization process
            Debug.WriteLine($"********************************************************");
            Debug.WriteLine($"FATAL ERROR DURING STARTUP: Database initialization failed.");
            Debug.WriteLine($"Error: {ex.Message}");
            Debug.WriteLine($"StackTrace: {ex.StackTrace}");
            Debug.WriteLine($"********************************************************");
            // Decide how to handle this error. You might want to:
            // 1. Exit the application
            //    Environment.Exit(1);
            // 2. Show an error to the user (more complex in MAUI startup)
            // 3. Just log and let it potentially fail later (less recommended for DB init)
            // For development, letting the exception propagate or exiting might be best.
            // Rethrowing ensures the failure is obvious if running with a debugger.
            throw; // Rethrow to make the failure obvious during development
        }
        // --- END: Invoke Database Initialization ---


        // Return the app host
        return app;
    }
}