using Microsoft.EntityFrameworkCore;
using Mnemo.Core.Enums;
using Mnemo.Core.Interfaces;
using Mnemo.Core.Models;

namespace Mnemo.Data.Services;

public class DeckService : IDeckService
{
    private readonly IDbContextFactory<MnemoDbContext>? _contextFactory;
    private readonly MnemoDbContext? _context;

    public DeckService(MnemoDbContext context)
    {
        _context = context;
    }

    public DeckService(IDbContextFactory<MnemoDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    private MnemoDbContext GetContext() => _context ?? _contextFactory!.CreateDbContext();

    public async Task<List<Deck>> GetAllDecksAsync()
    {
        await using var context = GetContext();
        return await context.Decks
            .AsNoTracking()
            .Include(d => d.Cards)
            .OrderBy(d => d.Name)
            .ToListAsync();
    }

    public async Task<Deck?> GetDeckByIdAsync(int id)
    {
        await using var context = GetContext();
        return await context.Decks
            .Include(d => d.Cards)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<Deck> CreateDeckAsync(string name, string? description = null)
    {
        await using var context = GetContext();
        var deck = new Deck
        {
            Name = name.Trim(),
            Description = description?.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        context.Decks.Add(deck);
        await context.SaveChangesAsync();
        return deck;
    }

    public async Task<bool> UpdateDeckAsync(Deck deck)
    {
        await using var context = GetContext();
        var existing = await context.Decks.FindAsync(deck.Id);
        if (existing == null) return false;

        existing.Name = deck.Name.Trim();
        existing.Description = deck.Description?.Trim();
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteDeckAsync(int id)
    {
        await using var context = GetContext();
        var existing = await context.Decks.FindAsync(id);
        if (existing == null) return false;

        context.Decks.Remove(existing);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<(int NewCount, int LearningCount, int DueCount)> GetDeckStatsAsync(int deckId)
    {
        await using var context = GetContext();
        var now = DateTime.UtcNow;

        var cards = await context.Cards
            .AsNoTracking()
            .Where(c => c.DeckId == deckId)
            .ToListAsync();

        var newCount = cards.Count(c => c.State == CardState.New);
        var learningCount = cards.Count(c => c.State == CardState.Learning || c.State == CardState.Relearning);
        var dueCount = cards.Count(c => c.State == CardState.Review && (c.DueDate == null || c.DueDate <= now));

        return (newCount, learningCount, dueCount);
    }
}
