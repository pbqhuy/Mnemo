using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Mnemo.Core.Interfaces;
using Mnemo.Data;
using Mnemo.Data.Services;
using Mnemo.Wpf.ViewModels;

namespace Mnemo.Wpf;

public partial class App : Application
{
    private ServiceProvider? _serviceProvider;

    public static IServiceProvider Services { get; private set; } = null!;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();
        ConfigureServices(services);

        _serviceProvider = services.BuildServiceProvider();
        Services = _serviceProvider;

        // Khởi tạo Database SQLite mnemo.db nếu chưa tồn tại
        using (var scope = _serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<MnemoDbContext>();
            await DbInitializer.EnsureCreatedAsync(dbContext);
        }

        // Khởi tạo và hiển thị cửa sổ chính
        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Đăng ký DbContext SQLite
        services.AddDbContext<MnemoDbContext>(options =>
        {
            options.UseSqlite("Data Source=mnemo.db");
        });

        // Đăng ký Business Services
        services.AddScoped<IDeckService, DeckService>();
        services.AddScoped<ICardService, CardService>();

        // Đăng ký ViewModels
        services.AddTransient<MainWindowViewModel>();

        // Đăng ký Views
        services.AddTransient<MainWindow>();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider?.Dispose();
        base.OnExit(e);
    }
}
