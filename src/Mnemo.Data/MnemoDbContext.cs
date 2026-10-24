using Microsoft.EntityFrameworkCore;
using Mnemo.Core.Models;

namespace Mnemo.Data;

public class MnemoDbContext : DbContext
{
    public DbSet<Deck> Decks => Set<Deck>();
    public DbSet<Card> Cards => Set<Card>();
    public DbSet<ReviewLog> ReviewLogs => Set<ReviewLog>();

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
            // Đường dẫn cơ sở dữ liệu SQLite mặc định lưu trong thư mục làm việc của app
            optionsBuilder.UseSqlite("Data Source=mnemo.db");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Deck configuration
        modelBuilder.Entity<Deck>(entity =>
        {
            entity.HasKey(d => d.Id);
            entity.Property(d => d.Name).IsRequired().HasMaxLength(150);
            entity.HasMany(d => d.Cards)
                  .WithOne(c => c.Deck)
                  .HasForeignKey(c => c.DeckId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Card configuration
        modelBuilder.Entity<Card>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Front).IsRequired();
            entity.Property(c => c.Back).IsRequired();
            entity.HasIndex(c => c.DeckId);
            entity.HasIndex(c => c.DueDate);
            entity.HasIndex(c => c.State);

            entity.HasMany(c => c.ReviewLogs)
                  .WithOne(r => r.Card)
                  .HasForeignKey(r => r.CardId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ReviewLog configuration
        modelBuilder.Entity<ReviewLog>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.HasIndex(r => r.CardId);
        });
    }
}
