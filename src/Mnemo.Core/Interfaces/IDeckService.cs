using Mnemo.Core.Models;

namespace Mnemo.Core.Interfaces;

public interface IDeckService
{
    Task<List<Deck>> GetAllDecksAsync();
    Task<Deck?> GetDeckByIdAsync(int id);
    Task<Deck> CreateDeckAsync(string name, string? description = null);
    Task<bool> UpdateDeckAsync(Deck deck);
    Task<bool> DeleteDeckAsync(int id);
    Task<(int NewCount, int LearningCount, int DueCount)> GetDeckStatsAsync(int deckId);
}
