using System.ComponentModel;
using System.IO;
using System.Windows;
using MarkdownMemo.Data;
using MarkdownMemo.Services;
using MarkdownMemo.ViewModels;

namespace MarkdownMemo;

/// <summary>
/// Markdownメモアプリのメイン画面。DataContextにMainViewModelを設定し、
/// WebView2は非同期初期化と<c>NavigateToString</c>呼び出しが必要でバインドできないため、
/// コードビハインドでViewModelの<see cref="MainViewModel.PreviewHtml"/>の変更を購読して描画する。
/// </summary>
public partial class MainWindow : Window
{
	private static readonly string NotesFolder = Path.Combine(
		Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
		"WpfLearningLab.MarkdownMemo",
		"notes");

	private readonly MainViewModel _viewModel;

	public MainWindow()
	{
		InitializeComponent();

		_viewModel = new MainViewModel(new FileMemoRepository(NotesFolder), new MarkdigMarkdownToHtmlConverter());
		DataContext = _viewModel;

		Loaded += OnLoaded;
		_viewModel.PropertyChanged += OnViewModelPropertyChanged;
	}

	private async void OnLoaded(object sender, RoutedEventArgs e)
	{
		try
		{
			await PreviewWebView.EnsureCoreWebView2Async();
			UpdatePreview();
		}
		catch (Exception ex)
		{
			// WebView2ランタイム未導入の環境ではEnsureCoreWebView2Asyncが例外を送出する。
			// OnLoadedはasync voidでcatchを持たないため、ここで捕捉し損ねるとプレビュー
			// 機能どころかアプリ全体がクラッシュしてしまう。プレビューは諦めても
			// 編集・保存自体はできるよう、エラーを表示するだけに留める。
			MessageBox.Show(
				$"プレビュー機能を初期化できませんでした。\nMicrosoft Edge WebView2 Runtimeが必要です。\n{ex.Message}",
				"Markdownメモ",
				MessageBoxButton.OK,
				MessageBoxImage.Warning);
		}
	}

	private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(MainViewModel.PreviewHtml))
		{
			UpdatePreview();
		}
	}

	private void UpdatePreview()
	{
		if (PreviewWebView.CoreWebView2 is null)
		{
			return;
		}

		PreviewWebView.CoreWebView2.NavigateToString(WrapHtml(_viewModel.PreviewHtml));
	}

	private static string WrapHtml(string bodyHtml) => $$"""
		<html>
		<head>
		<meta charset="utf-8">
		<style>body { font-family: sans-serif; padding: 8px; }</style>
		</head>
		<body>{{bodyHtml}}</body>
		</html>
		""";
}
