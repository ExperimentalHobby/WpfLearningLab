# WPF Learning Lab

C#(.NET 10) / WPF の学習用アプリを42個作成するプロジェクト集です。

## 進め方
1. 01〜10: XAML基本+イベント処理
2. 11〜20: MVVMパターン+外部リソース連携(API/DB)
3. 21〜30: 描画・並行処理・アーキテクチャ設計
4. 31〜42: 並行処理・カスタムUI・アーキテクチャ・デバイス連携等の応用技術

## アプリ一覧・進捗チェックリスト

- [x] [01_Calculator](01_Calculator/README.md) - 電卓アプリ
- [x] [02_UnitConverter](02_UnitConverter/README.md) - 単位変換ツール
- [x] [03_ToDoList](03_ToDoList/README.md) - ToDoリスト
- [x] [04_NotepadClone](04_NotepadClone/README.md) - メモ帳クローン
- [x] [05_BmiCalculator](05_BmiCalculator/README.md) - BMI計算機
- [x] [06_CountdownTimer](06_CountdownTimer/README.md) - カウントダウンタイマー
- [x] [07_ColorPalette](07_ColorPalette/README.md) - 色選択パレット
- [x] [08_RockPaperScissors](08_RockPaperScissors/README.md) - じゃんけんゲーム
- [x] [09_MiniDictionary](09_MiniDictionary/README.md) - 簡易電子辞書
- [x] [10_StickyNotes](10_StickyNotes/README.md) - 付箋(Sticky Notes)アプリ
- [x] [11_HouseholdBudget](11_HouseholdBudget/README.md) - 家計簿アプリ
- [x] [12_KanbanTaskManager](12_KanbanTaskManager/README.md) - タスク管理(カンバン風)
- [x] [13_WeatherApp](13_WeatherApp/README.md) - 天気予報アプリ
- [x] [14_RssReader](14_RssReader/README.md) - RSSリーダー
- [x] [15_ImageViewer](15_ImageViewer/README.md) - 画像ビューア
- [x] [16_LocalChatApp](16_LocalChatApp/README.md) - 簡易チャットアプリ(ローカルSocket)
- [x] [17_PasswordManager](17_PasswordManager/README.md) - パスワード管理ツール
- [x] [18_ExchangeRateMonitor](18_ExchangeRateMonitor/README.md) - 株価/為替モニター
- [x] [19_HabitTracker](19_HabitTracker/README.md) - 習慣トラッカー
- [x] [20_MarkdownMemo](20_MarkdownMemo/README.md) - 簡易CMS風メモアプリ
- [ ] [21_PaintTool](21_PaintTool/README.md) - お絵かきツール(ペイント風)
- [ ] [22_GameOfLife](22_GameOfLife/README.md) - ライフゲーム(Conway's Game of Life)
- [ ] [23_MazeSolverVisualizer](23_MazeSolverVisualizer/README.md) - 迷路生成&探索ビジュアライザ
- [ ] [24_MusicPlayer](24_MusicPlayer/README.md) - 音楽プレイヤー
- [ ] [25_FileOrganizer](25_FileOrganizer/README.md) - ファイル整理ツール
- [ ] [26_ChartVisualization](26_ChartVisualization/README.md) - 簡易グラフ描画(データ可視化)
- [ ] [27_NetworkMonitor](27_NetworkMonitor/README.md) - ネットワーク帯域モニター
- [ ] [28_PluginNoteApp](28_PluginNoteApp/README.md) - プラグイン対応メモアプリ
- [ ] [29_MiniCodeEditor](29_MiniCodeEditor/README.md) - 簡易IDE/コードエディタ
- [ ] [30_LocalTaskScheduler](30_LocalTaskScheduler/README.md) - ローカルタスクスケジューラ
- [ ] [31_ParallelImageProcessor](31_ParallelImageProcessor/README.md) - 並列画像バッチ処理ツール
- [ ] [32_LogStreamAggregator](32_LogStreamAggregator/README.md) - ログストリーム集計ツール
- [ ] [33_ContactManager](33_ContactManager/README.md) - 連絡先管理アプリ(DIコンテナ構成)
- [ ] [34_CustomGaugeControl](34_CustomGaugeControl/README.md) - 自作ゲージコントロール
- [ ] [35_CustomMarkupExtension](35_CustomMarkupExtension/README.md) - カスタムMarkupExtensionサンプル集
- [ ] [36_AnimatedDashboard](36_AnimatedDashboard/README.md) - アニメーションダッシュボード
- [ ] [37_Simple3DViewer](37_Simple3DViewer/README.md) - 簡易3Dモデルビューア
- [ ] [38_DragDropFileTagger](38_DragDropFileTagger/README.md) - ドラッグ&ドロップ ファイルタグ付けツール
- [ ] [39_VirtualizedLogViewer](39_VirtualizedLogViewer/README.md) - 大量ログビューア(仮想化)
- [ ] [40_SystemTrayUtility](40_SystemTrayUtility/README.md) - システムトレイ常駐ユーティリティ
- [ ] [41_GlobalHotkeyLauncher](41_GlobalHotkeyLauncher/README.md) - グローバルホットキーランチャー
- [ ] [42_AccessibleNoteApp](42_AccessibleNoteApp/README.md) - アクセシビリティ対応メモアプリ

## フォルダ構成
各フォルダに独立した .csproj を配置し、ソリューションファイル `WpfLearningLab.slnx`(.NET 10 / Visual Studio 2026 のXML形式ソリューション)で全体を管理しています。
