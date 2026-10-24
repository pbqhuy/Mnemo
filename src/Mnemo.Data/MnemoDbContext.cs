using Microsoft.EntityFrameworkCore;

namespace Mnemo.Data;

public class MnemoDbContext : DbContext
{
    public MnemoDbContext()
    {
    }

    public MnemoDbContext(DbContextOptions<MnemoDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=mnemo.db");
        }
    }
}
