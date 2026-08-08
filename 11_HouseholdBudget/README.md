# 家計簿アプリ

## 学習ポイント
MVVM入門、SQLite連携、DataGrid

## 概要
収支(収入・支出)を記録し、一覧表示・カテゴリ別集計を行う家計簿アプリ。SQLiteでデータを永続化し、MVVMパターン(View/ViewModel/Model分離)で実装した。

## 実装メモ
- MVVM基盤(`ObservableObject`/`RelayCommand`)は外部パッケージに依存せず自前実装し、`INotifyPropertyChanged`/`ICommand`の仕組み自体を学習する構成にした
- データアクセスは `ITransactionRepository` インタフェースを介して抽象化し、`MainViewModel` のテストではメモリ上で動く `FakeTransactionRepository` を使用。`SqliteTransactionRepository`(`Microsoft.Data.Sqlite` で生SQLによるCRUD)は実SQLite(一時ファイル)に対する結合テストで別途検証した
- `Microsoft.Data.Sqlite` の既定パッケージ構成には既知の脆弱性(CVE-2025-6965、`SQLitePCLRaw.lib.e_sqlite3` が同梱するSQLiteが古い)があったため、`SQLitePCLRaw.lib.e_sqlite3` を直接3.53.3に固定して警告を解消した
- 金額は `decimal` で保持し、`Transaction.SignedAmount` で収入(+)/支出(-)の符号付き金額に変換して合計・残高計算を行う
- 収入合計/支出合計/差引残高/カテゴリ別集計は `Transactions` から都度計算する算出プロパティとし、`Add`/`Delete`後に明示的に`PropertyChanged`を発火させてUIに反映させている
- DBファイルは `%AppData%\WpfLearningLab.HouseholdBudget\budget.db` に保存し、起動時に `CREATE TABLE IF NOT EXISTS` でテーブルを用意する
- UI Automationでの実機確認中に2つの不具合を発見し修正した:
  1. `AddCommand`実行後に入力欄(カテゴリ・金額・メモ)がクリアされず、直前の値が残っていた(日付・種別は連続入力の利便性のため意図的に保持)
  2. `SelectedTransaction`変更時に`DeleteCommand`の`CanExecuteChanged`を発火させていなかったため、行選択後も削除ボタンが無効化されたままになっていた(`InputAmount`変更時の`AddCommand`と同様の配線漏れ)
  
  いずれもTDDでテストを追加した上で修正し、再度UI Automationで動作を確認済み
- UI Automationで実機を操作し、複数取引の追加(収入/支出)、収支合計・カテゴリ別集計の即時反映、行選択・削除、アプリ再起動後のSQLite永続化(削除済み取引が復元されないことも含む)を確認済み

## ステータス
- [ ] 未着手
- [ ] 実装中
- [x] 完成
