using CommunityToolkit.Mvvm.ComponentModel;

namespace Mnemo.Wpf.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private string _title = "Mnemo";

    [ObservableProperty]
    private string _status = "Sẵn sàng (Đã kết nối SQLite)";
}
