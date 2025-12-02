using Microsoft.Extensions.DependencyInjection;
using BankManagerApp.Converters;
using BankManagerApp.Views;
using BankManagerApp.Services;
using System;

namespace BankManagerApp;

public partial class App : Application
{
    private readonly IServiceProvider _serviceProvider;

    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        try
        {
            // Initialize database
            var databaseService = _serviceProvider.GetRequiredService<DatabaseService>();
            Task.Run(async () => await databaseService.Init()).Wait();

            // Resolve MainPage from DI
            var mainPage = _serviceProvider.GetRequiredService<MainPage>();
            
            return new Window(new NavigationPage(mainPage)
            {
                BarBackgroundColor = Color.FromArgb("#512BD4"),
                BarTextColor = Colors.White
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"App.CreateWindow CRASH: {ex}");
            throw;
        }
    }
}