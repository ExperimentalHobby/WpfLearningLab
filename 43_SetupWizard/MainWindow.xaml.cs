using System;
using System.IO;
using System.Windows;
using SetupWizard.Models;
using SetupWizard.Pages;
using SetupWizard.Services;

namespace SetupWizard;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	private readonly WizardState _state = new();
	private readonly IWizardSettingsRepository _repository = new JsonWizardSettingsRepository(
		Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
			"WpfLearningLab.SetupWizard",
			"wizard-settings.json"));

	public MainWindow()
	{
		InitializeComponent();
		WizardFrame.Navigate(new Step1BasicInfoPage(_state, _repository));
	}
}
