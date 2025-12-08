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
			var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			var dbPath = Path.Combine(folder, "bankmanager.db");
			options.UseSqlite($"Data Source={dbPath}");
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
