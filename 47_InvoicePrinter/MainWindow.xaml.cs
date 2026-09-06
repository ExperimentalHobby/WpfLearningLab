using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using InvoicePrinter.Models;
using Microsoft.Win32;

namespace InvoicePrinter;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly ObservableCollection<InvoiceLine> _lines = new()
    {
        new InvoiceLine { ItemName = "商品A", Quantity = 1, UnitPrice = 1000m },
    };

    private readonly InvoiceCalculator _calculator = new();

    public MainWindow()
    {
        InitializeComponent();
        LinesDataGrid.ItemsSource = _lines;
    }

    private void AddLineButton_Click(object sender, RoutedEventArgs e)
    {
        _lines.Add(new InvoiceLine());
    }

    private void RemoveLineButton_Click(object sender, RoutedEventArgs e)
    {
        if (LinesDataGrid.SelectedItem is InvoiceLine selected)
        {
            _lines.Remove(selected);
        }
    }

    private void PreviewButton_Click(object sender, RoutedEventArgs e)
    {
        PreviewViewer.Document = BuildDocument();
    }

    private FlowDocument BuildDocument()
    {
        var subtotal = _calculator.CalculateSubtotal(_lines);
        var tax = _calculator.CalculateTax(subtotal);
        var total = _calculator.CalculateTotal(subtotal, tax);
        return FlowDocumentBuilder.Build(CustomerNameTextBox.Text, _lines, subtotal, tax, total);
    }

    private void PrintButton_Click(object sender, RoutedEventArgs e)
    {
        var printDialog = new PrintDialog();
        if (printDialog.ShowDialog() != true)
        {
            return;
        }

        // プレビュー用とは別に新しいFlowDocumentインスタンスを生成する。
        // 1つのFlowDocumentは同時に複数のビューア/ページネータへバインドできないため。
        var document = BuildDocument();
        var paginatorSource = (IDocumentPaginatorSource)document;

        try
        {
            printDialog.PrintDocument(paginatorSource.DocumentPaginator, "請求書");
        }
        catch (Exception ex)
        {
            // プリンタドライバ由来の失敗モードは多様(Win32Exception・COMException等)で
            // ドライバ依存のため網羅的に列挙できない。このアプリの他のtry/catchとは意図的に異なり、
            // ここは印刷操作の最終防御境界として広く捕捉する。
            MessageBox.Show($"印刷に失敗しました: {ex.Message}", "請求書プレビュー・印刷", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ExportXpsButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new SaveFileDialog { Filter = "XPS Document (*.xps)|*.xps" };
        if (dialog.ShowDialog() != true)
        {
            return;
        }

        var document = BuildDocument();
        var paginatorSource = (IDocumentPaginatorSource)document;

        try
        {
            if (File.Exists(dialog.FileName))
            {
                File.Delete(dialog.FileName);
            }

            using var xpsDocument = new XpsDocument(dialog.FileName, FileAccess.ReadWrite);
            var writer = XpsDocument.CreateXpsDocumentWriter(xpsDocument);
            writer.Write(paginatorSource.DocumentPaginator);
            xpsDocument.Close();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            MessageBox.Show($"XPS出力に失敗しました: {ex.Message}", "請求書プレビュー・印刷", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
