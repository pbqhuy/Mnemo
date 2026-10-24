namespace Mnemo.Data;

public static class DbInitializer
{
    public static async Task EnsureCreatedAsync(MnemoDbContext context)
    {
        // Khởi tạo cơ sở dữ liệu SQLite nếu chưa tồn tại
        await context.Database.EnsureCreatedAsync();
    }
}
