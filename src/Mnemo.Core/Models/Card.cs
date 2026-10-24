using Mnemo.Core.Enums;

namespace Mnemo.Core.Models;

public class Card
{
    public int Id { get; set; }
    public int DeckId { get; set; }
    public Deck? Deck { get; set; }

    public string Front { get; set; } = string.Empty;
    public string Back { get; set; } = string.Empty;

    public CardState State { get; set; } = CardState.New;
    public int Repetitions { get; set; } = 0;
    public int IntervalDays { get; set; } = 0;
    public double EaseFactor { get; set; } = 2.5;

    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastReviewedAt { get; set; }

    public List<ReviewLog> ReviewLogs { get; set; } = [];
}
