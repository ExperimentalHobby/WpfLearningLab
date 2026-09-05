using System.Windows;
using SetupWizard.Models;
using SetupWizard.Pages;

namespace SetupWizard;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly WizardState _state = new();

    public MainWindow()
    {
        InitializeComponent();
        WizardFrame.Navigate(new Step1BasicInfoPage(_state));
    }
}
