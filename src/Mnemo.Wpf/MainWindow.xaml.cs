using System.Windows;
using Mnemo.Wpf.ViewModels;

namespace Mnemo.Wpf;

public partial class MainWindow : Window
{
    public MainWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}