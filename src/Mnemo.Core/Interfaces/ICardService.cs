using Mnemo.Core.Enums;
using Mnemo.Core.Models;

namespace Mnemo.Core.Interfaces;

public interface ICardService
{
    Task<List<Card>> GetCardsByDeckIdAsync(int deckId);
    Task<Card?> GetCardByIdAsync(int id);
    Task<Card> AddCardAsync(int deckId, string front, string back);
    Task<bool> UpdateCardAsync(Card card);
    Task<bool> DeleteCardAsync(int id);
    Task<List<Card>> GetDueCardsAsync(int deckId);
    Task RecordReviewAsync(int cardId, ReviewRating rating);
}
