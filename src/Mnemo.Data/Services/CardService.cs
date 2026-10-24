using Microsoft.EntityFrameworkCore;
using Mnemo.Core.Enums;
using Mnemo.Core.Interfaces;
using Mnemo.Core.Models;

namespace Mnemo.Data.Services;

public class CardService : ICardService
{
    private readonly IDbContextFactory<MnemoDbContext>? _contextFactory;
    private readonly MnemoDbContext? _context;

    public CardService(MnemoDbContext context)
    {
        _context = context;
    }

    public CardService(IDbContextFactory<MnemoDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    private MnemoDbContext GetContext() => _context ?? _contextFactory!.CreateDbContext();

    public async Task<List<Card>> GetCardsByDeckIdAsync(int deckId)
    {
        await using var context = GetContext();
        return await context.Cards
            .AsNoTracking()
            .Where(c => c.DeckId == deckId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<Card?> GetCardByIdAsync(int id)
    {
        await using var context = GetContext();
        return await context.Cards
            .Include(c => c.ReviewLogs)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Card> AddCardAsync(int deckId, string front, string back)
    {
        await using var context = GetContext();
        var card = new Card
        {
            DeckId = deckId,
            Front = front.Trim(),
            Back = back.Trim(),
            State = CardState.New,
            EaseFactor = 2.5,
            Repetitions = 0,
            IntervalDays = 0,
            DueDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        context.Cards.Add(card);
        await context.SaveChangesAsync();
        return card;
    }

    public async Task<bool> UpdateCardAsync(Card card)
    {
        await using var context = GetContext();
        var existing = await context.Cards.FindAsync(card.Id);
        if (existing == null) return false;

        existing.Front = card.Front.Trim();
        existing.Back = card.Back.Trim();
        existing.State = card.State;
        existing.EaseFactor = card.EaseFactor;
        existing.IntervalDays = card.IntervalDays;
        existing.Repetitions = card.Repetitions;
        existing.DueDate = card.DueDate;

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteCardAsync(int id)
    {
        await using var context = GetContext();
        var existing = await context.Cards.FindAsync(id);
        if (existing == null) return false;

        context.Cards.Remove(existing);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Card>> GetDueCardsAsync(int deckId)
    {
        await using var context = GetContext();
        var now = DateTime.UtcNow;

        return await context.Cards
            .Where(c => c.DeckId == deckId &&
                       (c.State == CardState.New ||
                        c.State == CardState.Learning ||
                        c.DueDate == null ||
                        c.DueDate <= now))
            .OrderBy(c => c.State)
            .ThenBy(c => c.DueDate)
            .ToListAsync();
    }

    public async Task RecordReviewAsync(int cardId, ReviewRating rating)
    {
        await using var context = GetContext();
        var card = await context.Cards.FindAsync(cardId);
        if (card == null) return;

        var now = DateTime.UtcNow;

        // Thuật toán Spaced Repetition (SM-2 căn bản)
        if (rating == ReviewRating.Again)
        {
            card.Repetitions = 0;
            card.IntervalDays = 1;
            card.State = CardState.Learning;
            card.EaseFactor = Math.Max(1.3, card.EaseFactor - 0.2);
            card.DueDate = now.AddDays(1);
        }
        else
        {
            card.Repetitions++;
            card.State = CardState.Review;

            if (card.Repetitions == 1)
            {
                card.IntervalDays = 1;
            }
            else if (card.Repetitions == 2)
            {
                card.IntervalDays = 6;
            }
            else
            {
                var factor = card.EaseFactor;
                if (rating == ReviewRating.Hard) factor = Math.Max(1.2, factor - 0.15);
                else if (rating == ReviewRating.Easy) factor += 0.15;

                card.IntervalDays = Math.Max(1, (int)Math.Round(card.IntervalDays * factor));
                card.EaseFactor = Math.Max(1.3, factor);
            }

            card.DueDate = now.AddDays(card.IntervalDays);
        }

        card.LastReviewedAt = now;

        // Lưu lịch sử review
        context.ReviewLogs.Add(new ReviewLog
        {
            CardId = card.Id,
            Rating = rating,
            ReviewedAt = now,
            IntervalDays = card.IntervalDays,
            EaseFactor = card.EaseFactor
        });

        await context.SaveChangesAsync();
    }
}
