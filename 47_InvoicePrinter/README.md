# 請求書プレビュー・印刷アプリ

## 学習ポイント
FlowDocument / DocumentViewer、PrintDialogによる印刷、ページ分割・レイアウト制御

## 概要
明細入力からFlowDocumentを動的に組み立て、プレビュー・印刷・XPS保存ができる請求書アプリ。

## 実装メモ
- 小計・消費税・合計の金額計算を `InvoiceCalculator` に、`FlowDocument` の組み立てを `FlowDocumentBuilder` に分離し、xUnitでTDD(Red→Green→Refactor)で実装した。金額計算は `decimal` を使用し、消費税(10%)は円未満切り捨てにしている
- 明細は `Models/InvoiceLine`(品目/数量/単価)。編集用の `DataGrid` にバインドし、「行追加」「行削除」ボタンで増減できる
- プレビューには `FlowDocumentPageViewer` を使用した。`FlowDocumentScrollViewer` と異なりページ単位で表示され、標準のページ送りツールバー(`n of m`表示、前へ/次へ)を持つため、明細行数が多い場合の自動改ページが視覚的に確認できる
- 「印刷」「XPS保存」はそれぞれ専用に新しい `FlowDocument` インスタンスを都度 `FlowDocumentBuilder.Build(...)` で生成して使う。1つの `FlowDocument` は同時に複数のビューア/ページネータへバインドできない(プレビューの `FlowDocumentPageViewer` が既に保持しているドキュメントをそのまま印刷用の `DocumentPaginator` として使い回すと例外になる)ため
- XPS保存は `XpsDocument` + `XpsDocument.CreateXpsDocumentWriter` で `FlowDocument` の `DocumentPaginator` を書き出す。保存先に同名ファイルが既に存在する場合は事前に削除してから書き込む(`XpsDocument` は既存ファイルへの追記を想定した挙動になるため)
- UI Automationで実機を操作し、プレビュー表示、明細を60行以上に増やした際に自動改ページされること(`1 of 5`ページ)、PrintDialogが開き印刷を実行できること、XPS保存でファイルが生成されることを確認済み

## ステータス
- [ ] 未着手
- [ ] 実装中
- [x] 完成
