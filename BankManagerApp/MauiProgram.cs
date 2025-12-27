using Microsoft.Extensions.Logging;
using BankManager.Data;
using BankManagerApp.Services;
using BankManagerApp.Views;
using BankManagerApp.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BankManagerApp;

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
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		builder.Services.AddDbContext<BankDbContext>(options =>
		{
			// ===== SQLite (Local Database - Current) =====
			var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			var dbPath = Path.Combine(folder, "bankmanager.db");
			options.UseSqlite($"Data Source={dbPath}");

			// ===== SQL Server (Remote Database - Commented) =====
			// To use SQL Server instead of SQLite:
			// 1. Install package: Microsoft.EntityFrameworkCore.SqlServer
			// 2. Uncomment the line below and comment out the SQLite line above
			// 3. Replace YOUR_SERVER_IP, YOUR_DATABASE_NAME, YOUR_USERNAME, YOUR_PASSWORD with actual values
			// 
			// var connectionString = "Server=YOUR_SERVER_IP,1433;Database=YOUR_DATABASE_NAME;User Id=YOUR_USERNAME;Password=YOUR_PASSWORD;TrustServerCertificate=True;Encrypt=True;";
			// options.UseSqlServer(connectionString);
			//
			// Example:
			// var connectionString = "Server=192.168.1.50,1433;Database=BankManagerDB;User Id=sa;Password=MySecretPass123;TrustServerCertificate=True;Encrypt=True;";
			// options.UseSqlServer(connectionString);
			//
			// ⚠️ SECURITY WARNING: Storing passwords in code is NOT secure for production apps!
			// Consider using environment variables or a secure configuration service.
		});

		builder.Services.AddTransient<DatabaseService>();
		builder.Services.AddSingleton<AuthService>();

		// Register Pages and ViewModels
		builder.Services.AddTransient<MainPage>();
		builder.Services.AddTransient<MainViewModel>();
		builder.Services.AddTransient<AccountDetailPage>();
		builder.Services.AddTransient<AccountDetailViewModel>();
		builder.Services.AddTransient<LoginPage>();
		builder.Services.AddTransient<RegisterPage>();
		builder.Services.AddTransient<PasswordResetPage>();

		return builder.Build();
	}
}
