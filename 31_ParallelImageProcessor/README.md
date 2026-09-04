# 並列画像バッチ処理ツール

## 学習ポイント
並列処理(Task.WhenAll/Parallel.ForEach)、キャンセル(CancellationToken)、進捗報告(IProgress<T>)

## 概要
指定フォルダ内の複数画像(png/jpg/jpeg/bmp)に対して、リサイズ・グレースケール化を並列実行するバッチ処理ツール。処理中の進捗をリアルタイムに表示し、実行中の処理を中断できる。

## 実装メモ
- 画像処理コアは`System.Windows.Media.Imaging`ではなく`System.Drawing`(GDI+)の`Bitmap`/`Graphics`/`ColorMatrix`を採用した。呼び出しごとに独立したインスタンスを扱う限りスレッドセーフに並列実行でき、STA制約のあるWPFの`BitmapSource`系よりユニットテストで素直に検証できるため
- 並列実行は`Parallel.ForEachAsync`(.NET 6+)を使用。`ParallelOptions.CancellationToken`/`MaxDegreeOfParallelism`を渡すだけでTask.WhenAll/Parallel.ForEach + CancellationTokenを自然に実現できた
- 進捗報告は`IProgress<BatchProgress>`(`Progress<T>`)を使用。`Progress<T>`はコンストラクタ呼び出し時にキャプチャした`SynchronizationContext`へ自動でポストされるため、View側で追加のマーシャリング処理(`IUiDispatcher`等)が不要だった
- **ハマった点(実機バグ: InvalidCastException)**: `SourceFolder`/`DestinationFolder`のsetterで`StartCommand`を`(RelayCommand)`にキャストしていたが、実際には`AsyncRelayCommand`型で生成しており、フォルダ選択ボタンを押した瞬間に`InvalidCastException`でアプリがクラッシュした。単体テストでは`ImageBatchProcessor`のロジックしか検証しておらず、`MainViewModel`のプロパティsetterとコマンドの実際の型の組み合わせは検証していなかったため発見できなかった。UI Automationで実際にボタンをクリックして初めて発見し、`(AsyncRelayCommand)`への修正と合わせて`MainViewModelTests`(フォルダ選択→StartCommand.CanExecuteの遷移、実処理の完了確認)を追加して再発防止とした
- **既知の制限(スクリーンショットによる目視確認)**: 今回、UI Automationでのプロパティ検証(`SourceFolderText`/`ResultSummaryText`等のAutomationId経由の値取得、出力フォルダのファイル数)によって処理が正しく行われていることは確認できたが、`Graphics.CopyFromScreen`/`PrintWindow`によるスクリーンショット取得は、起動した本アプリのウィンドウ領域とは無関係な別ウィンドウ(このセッションの別プロセスの画面)を捉えてしまい、目視確認には使えなかった。UI Automationのプロパティ取得(`AutomationElement.Current`)は対象プロセスのオートメーションツリーを直接参照するため影響を受けず、機能検証はこちらを主とした

## 動作確認
- 5枚のテスト画像(実ファイル)をフォルダに用意し、UI Automationでフォルダ選択→リサイズ/グレースケールオプション設定→開始→完了までを実行
- `ResultSummaryText`が「完了: 成功 5件 / 失敗 0件」を表示し、保存先フォルダに5件のファイルが生成されることを確認
- 単体テストでは50枚のダミー画像を対象に、1件処理完了時点でキャンセルすると全件処理前に中断されること(`OperationCanceledException`/出力ファイル数<50)を検証

## ステータス
- [ ] 未着手
- [ ] 実装中
- [x] 完成
