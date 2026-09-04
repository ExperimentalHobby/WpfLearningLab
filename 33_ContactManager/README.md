# 連絡先管理アプリ(DIコンテナ構成)

## 学習ポイント
Microsoft.Extensions.DependencyInjection、Microsoft.Extensions.Hosting(Generic Host)、WPFでのDIコンテナ活用

## 概要
氏名・電話番号・メールアドレスを管理する連絡先管理アプリ(CRUD)。EF Core + SQLiteでの永続化は`19_HabitTracker`等と同じだが、依存解決の方法を「Generic Host + DIコンテナ」に変えた点が本アプリの主眼。

## 実装メモ
- **既存アプリ(手動DI)との対比**: `19_HabitTracker`等はコードビハインド(`MainWindow`のコンストラクタ)で`new HabitTrackerDbContext(...)`のように依存を直接組み立てていた。本アプリは`App.xaml.cs`の`OnStartup`で`Host.CreateDefaultBuilder().ConfigureServices(...)`を使い、`DbContext`/`IContactRepository`/`MainViewModel`/`MainWindow`をすべてDIコンテナに登録し、`_scope.ServiceProvider.GetRequiredService<MainWindow>()`で解決している。依存関係の組み立てが1箇所(`ConfigureServices`)に集約され、`MainViewModel`のコンストラクタは`IContactRepository`を受け取るだけで済む(`new`を書かない)
- **デスクトップアプリのスコープ管理**: ASP.NET Coreは「1リクエスト=1スコープ」だが、WPFデスクトップアプリにはリクエストという単位が無い。今回はアプリのライフタイム全体を1つの`IServiceScope`として扱い、`OnStartup`で生成・`OnExit`で破棄する設計にした(`AddDbContext`が既定でScopedに登録するため、Scopeを明示的に管理しないとルートコンテナから直接解決した際に単一インスタンスの生存期間が曖昧になる)
- `App.xaml`から`StartupUri`を削除した。DIコンテナ経由で`MainWindow`を解決してから`Show()`する必要があるため、WPFの既定の自動起動の仕組みは使えない
- `Contact`エンティティは`INotifyPropertyChanged`を自前実装し、一覧の`SelectedContact`に編集フォームのTextBoxを直接双方向バインドする設計にした(`SelectedContact.Name`のようなドット区切りパスバインディングは、`SelectedContact`自体の変更通知にもきちんと追従する)
- `EfContactRepositoryTests`は`19_HabitTracker`の`EfHabitRepositoryTests`と同じく、一時ファイルへの実SQLite+EF Coreマイグレーションで検証した(モックなし)。氏名の昇順ソートのテストは、SQLiteの既定コレーションが符号位置順であり日本語の五十音順とは一致しないため、曖昧さを避けアルファベット名で検証した

## 動作確認(UI Automation)
- 2件の連絡先を追加→一覧に2件表示されることを確認
- 1件を選択→編集フォームに選択した連絡先の氏名が正しく反映されることを確認(`SelectedContact.Name`バインディング)
- 電話番号を編集して更新→削除→一覧から消えることを確認
- アプリを再起動し、削除されなかった連絡先がSQLiteファイルから正しく読み込まれることを確認(DIコンテナ経由のDbContext/リポジトリが実際に永続化されたデータへアクセスできている証跡)

## ステータス
- [ ] 未着手
- [ ] 実装中
- [x] 完成
