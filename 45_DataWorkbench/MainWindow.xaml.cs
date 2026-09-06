using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DataWorkbench.Models;
using Microsoft.Win32;

namespace DataWorkbench;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	private readonly ObservableCollection<CsvRow> _rows = new();
	private ICollectionView? _view;
	private CsvTable? _table;

	public MainWindow()
	{
		InitializeComponent();
	}

	private void OpenButton_Click(object sender, RoutedEventArgs e)
	{
		var dialog = new OpenFileDialog { Filter = "CSVファイル (*.csv)|*.csv|すべてのファイル (*.*)|*.*" };
		if (dialog.ShowDialog() != true)
		{
			return;
		}

		try
		{
			// File.ReadAllLinesは全行を一括でメモリに読み込むため、巨大なCSVファイルで
			// メモリを圧迫する。File.ReadLinesは1行ずつ遅延読み込みするストリーミングAPIのため
			// こちらを使う(CsvParser.Parseは複数行にまたがるクォートフィールドにも対応済み)。
			var lines = File.ReadLines(dialog.FileName);
			_table = CsvParser.Parse(lines);
			LoadTable(_table);
			StatusTextBlock.Text = $"{_table.Rows.Count} 行読み込みました。";
		}
		catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
		{
			StatusTextBlock.Text = $"読み込みに失敗しました: {ex.Message}";
		}
	}

	private void LoadTable(CsvTable table)
	{
		RowsDataGrid.Columns.Clear();
		_rows.Clear();

		foreach (var header in table.Headers)
		{
			RowsDataGrid.Columns.Add(new DataGridTextColumn
			{
				Header = header,
				Binding = new Binding($"[{header}]"),
				SortMemberPath = $"[{header}]",
			});
		}

		foreach (var row in table.Rows)
		{
			_rows.Add(row);
		}

		_view = CollectionViewSource.GetDefaultView(_rows);
		RowsDataGrid.ItemsSource = _view;

		GroupByComboBox.ItemsSource = new[] { "(なし)" }.Concat(table.Headers).ToList();
		GroupByComboBox.SelectedIndex = 0;
	}

	private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
	{
		if (_view == null)
		{
			return;
		}

		var searchText = SearchTextBox.Text;
		_view.Filter = obj => obj is CsvRow row && CsvFilterEngine.Matches(row, searchText);
	}

	private void GroupByComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (_view == null)
		{
			return;
		}

		_view.GroupDescriptions.Clear();

		if (GroupByComboBox.SelectedItem is string columnName && columnName != "(なし)")
		{
			_view.GroupDescriptions.Add(new PropertyGroupDescription($"[{columnName}]"));
		}
	}

	private void ExportButton_Click(object sender, RoutedEventArgs e)
	{
		if (_table == null || _view == null)
		{
			StatusTextBlock.Text = "エクスポートするデータがありません。";
			return;
		}

		var dialog = new SaveFileDialog { Filter = "CSVファイル (*.csv)|*.csv" };
		if (dialog.ShowDialog() != true)
		{
			return;
		}

		var exportTable = new CsvTable();
		exportTable.Headers.AddRange(_table.Headers);
		foreach (var item in _view)
		{
			if (item is CsvRow row)
			{
				exportTable.Rows.Add(row);
			}
		}

		try
		{
			File.WriteAllLines(dialog.FileName, CsvParser.ToCsvLines(exportTable));
			StatusTextBlock.Text = $"{exportTable.Rows.Count} 行をエクスポートしました。";
		}
		catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
		{
			StatusTextBlock.Text = $"エクスポートに失敗しました: {ex.Message}";
		}
	}
}
