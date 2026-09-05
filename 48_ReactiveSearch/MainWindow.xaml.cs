using System.Windows;
using ReactiveSearch.ViewModels;

namespace ReactiveSearch;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new SearchViewModel();
    }
}
