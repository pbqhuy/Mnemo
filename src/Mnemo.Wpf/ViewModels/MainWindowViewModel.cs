using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mnemo.Core.Interfaces;

namespace Mnemo.Wpf.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly IDeckService _deckService;
    private readonly ICardService _cardService;

    [ObservableProperty]
    private ObservableCollection<DeckItemViewModel> _decks = [];

    [ObservableProperty]
    private DeckItemViewModel? _selectedDeck;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _statusMessage = "Sẵn sàng (Đã kết nối SQLite mnemo.db)";

    [ObservableProperty]
    private bool _isCreateDeckDialogOpen;

    [ObservableProperty]
    private string _newDeckName = string.Empty;

    public MainWindowViewModel(IDeckService deckService, ICardService cardService)
    {
        _deckService = deckService;
        _cardService = cardService;
    }

    [RelayCommand]
    public async Task InitializeAsync()
    {
        await LoadDecksAsync();
    }

    [RelayCommand]
    public async Task LoadDecksAsync()
    {
        IsLoading = true;
        StatusMessage = "Đang tải dữ liệu...";

        try
        {
            var deckList = await _deckService.GetAllDecksAsync();
            Decks.Clear();

            foreach (var d in deckList)
            {
                var stats = await _deckService.GetDeckStatsAsync(d.Id);
                Decks.Add(new DeckItemViewModel
                {
                    Id = d.Id,
                    Name = d.Name,
                    Description = d.Description,
                    NewCount = stats.NewCount,
                    LearningCount = stats.LearningCount,
                    DueCount = stats.DueCount,
                    TotalCards = d.Cards.Count
                });
            }

            StatusMessage = $"Đã tải {Decks.Count} bộ thẻ từ SQLite.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Lỗi khi tải dữ liệu: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public void OpenCreateDeckDialog()
    {
        NewDeckName = string.Empty;
        IsCreateDeckDialogOpen = true;
    }

    [RelayCommand]
    public void CloseCreateDeckDialog()
    {
        IsCreateDeckDialogOpen = false;
    }

    [RelayCommand]
    public async Task SaveNewDeckAsync()
    {
        if (string.IsNullOrWhiteSpace(NewDeckName))
            return;

        try
        {
            await _deckService.CreateDeckAsync(NewDeckName);
            IsCreateDeckDialogOpen = false;
            await LoadDecksAsync();
            StatusMessage = $"Đã tạo bộ thẻ '{NewDeckName}'.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Lỗi: {ex.Message}";
        }
    }

    [RelayCommand]
    public async Task DeleteDeckAsync(DeckItemViewModel? deck)
    {
        if (deck == null) return;

        try
        {
            await _deckService.DeleteDeckAsync(deck.Id);
            await LoadDecksAsync();
            StatusMessage = $"Đã xóa '{deck.Name}'.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Lỗi: {ex.Message}";
        }
    }
}
