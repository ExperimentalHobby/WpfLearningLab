# CSVデータブラウザ

## 学習ポイント
CollectionViewSourceによるソート/フィルタ/グループ化、CSV入出力、DataGridとの連携

## 概要
CSVファイルを読み込み、DataGrid上でソート/フィルタ/グループ化して閲覧できるデータブラウザ。フィルタ後の内容をCSVとしてエクスポートできる。

## 実装メモ
- CSVの読み書きロジックを `CsvParser` に、検索フィルタの判定ロジックを `CsvFilterEngine` に分離し、xUnitでTDD(Red→Green→Refactor)で実装した。`CsvParser` はダブルクォートで囲まれたフィールド内のカンマ・`""`によるクォートのエスケープに対応した簡易CSVパーサ/ライタになっている
- CSVは列数が可変なため、行モデル `CsvRow` は `Dictionary<string, string>` の派生クラスとした。WPFの `Binding` は `Dictionary<string,string>` のインデクサ(`[列名]`)を解決できるため、読み込んだ列名から `DataGridTextColumn` (`Binding="[列名]"`)を動的生成することで、任意の列構成のCSVをDataGridに表示できる
- `CollectionViewSource.GetDefaultView` で取得した `ICollectionView` をDataGridの `ItemsSource` に設定し、ソート・フィルタ・グループ化はすべてこの `ICollectionView` に対して行う。列ヘッダクリックでのソートは各列の `SortMemberPath` を `[列名]` に設定することで標準機能のまま動作する
- 検索テキストボックスの `TextChanged` で `ICollectionView.Filter` に `CsvFilterEngine.Matches` を都度設定している。グループ化列を選ぶ`ComboBox`では `GroupDescriptions` に `PropertyGroupDescription("[列名]")` を設定し、`DataGrid.GroupStyle` でExpanderによるグループヘッダー表示にしている(グループ表示のため `VirtualizingPanel.IsVirtualizing="False"` を設定している)
- エクスポートは `_table.Headers` を保ったまま、現在の `ICollectionView` を列挙した行(フィルタ・ソート・グループ後の内容)を `CsvParser.ToCsvLines` でCSV化して書き出す
- UI Automationで実機を操作し、CSV読み込み、列ヘッダクリックによるソート、検索フィルタ、部署列でのグループ化表示、エクスポート(エクスポートしたファイルの内容がグループ化後の並び順になっていること)を確認済み

## ステータス
- [ ] 未着手
- [ ] 実装中
- [x] 完成
