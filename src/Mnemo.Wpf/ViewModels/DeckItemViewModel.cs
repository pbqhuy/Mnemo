using CommunityToolkit.Mvvm.ComponentModel;

namespace Mnemo.Wpf.ViewModels;

public partial class DeckItemViewModel : ObservableObject
{
    public int Id { get; set; }

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string? _description;

    [ObservableProperty]
    private int _newCount;

    [ObservableProperty]
    private int _learningCount;

    [ObservableProperty]
    private int _dueCount;

    [ObservableProperty]
    private int _totalCards;

    public int TotalDueToday => NewCount + LearningCount + DueCount;
}
