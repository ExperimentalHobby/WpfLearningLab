# 習慣トラッカー

## 学習ポイント
Entity Framework Core、グラフ表示(達成率)

## 概要
日々の習慣の実施状況を記録し、直近14日間の達成率をグラフで可視化するトラッカーアプリ。本リポジトリで初めてEntity Framework Coreを使用する(これまでのDB連携アプリは`Microsoft.Data.Sqlite`の生ADO.NETだった)。

## 実装メモ
- `Habit`(習慣名)・`HabitLog`(習慣ID・日付・実施有無、`(HabitId, Date)`にユニークインデックス)の2エンティティを`HabitTrackerDbContext`(EF Core)で管理する
- `dotnet ef migrations add InitialCreate`でマイグレーションを生成し`Data/Migrations/`にコミット。アプリ起動時・テストの両方で`DbContext.Database.Migrate()`を呼び、同じマイグレーション経路でDBを初期化する。EF Coreツールがデザインタイムに`DbContext`を生成できるよう`HabitTrackerDbContextFactory`(`IDesignTimeDbContextFactory<T>`)を用意した(本アプリはASP.NET Coreのようなホスト/DIコンテナを持たないため)
- `AchievementRateCalculator`をDB非依存の純粋な静的ロジックとして切り出し、`CalculateRate`(1習慣・期間の達成率)と`CalculateDailySeries`(日別の全習慣達成率、グラフ用)を計算する。ユニットテストはDBを介さずこのロジックだけを検証できる
- グラフは外部チャートライブラリを追加せず、`ItemsControl`+`Border`の高さを達成率に応じてバインドする自作の簡易棒グラフで実装した(`RateToHeightConverter`)
- `MainViewModel`は「今日」の日付を`Func<DateOnly>`で受け取れるようにし、テストでは固定日付を注入して達成率計算を決定的に検証できるようにした
- 当日チェックボックスは`IsChecked`を`OneWay`バインドし、実際の永続化・再計算は`Command`(`ToggleTodayCommand`)経由で行う設計とした。`TogglePattern.Toggle()`はWPFの`Click`ルーテッドイベントを発火させず、`Command`はClick経由で実行されるため、UI Automationでの動作確認は実際のマウスクリック(`mouse_event`)で行った
- UI Automationで実機を操作し、以下を確認済み:
  - 習慣を追加すると一覧に反映され、初期の達成率が0%と表示されること
  - 当日チェックボックスを実際にクリックすると、チェック状態・達成率(直近14日: 7%)・グラフの当日分(50%、全2習慣中1習慣達成)が正しく更新されること
  - 再度クリックするとチェック状態が元に戻ること
  - 習慣の削除が一覧に反映されること
  - アプリを再起動しても、EF Core経由で保存した習慣がマイグレーション済みのDBから正しく復元されること

## ステータス
- [ ] 未着手
- [ ] 実装中
- [x] 完成
