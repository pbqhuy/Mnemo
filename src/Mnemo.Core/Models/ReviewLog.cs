using Mnemo.Core.Enums;

namespace Mnemo.Core.Models;

public class ReviewLog
{
    public int Id { get; set; }
    public int CardId { get; set; }
    public Card? Card { get; set; }

    public ReviewRating Rating { get; set; }
    public DateTime ReviewedAt { get; set; } = DateTime.UtcNow;
    public int IntervalDays { get; set; }
    public double EaseFactor { get; set; }
}
