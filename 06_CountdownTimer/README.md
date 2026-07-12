# カウントダウンタイマー

## 学習ポイント
DispatcherTimerの基本

## 概要
指定した時間からカウントダウンし、0になったら通知するタイマーアプリ。

## 実装メモ
- 状態管理(停止/実行中/一時停止/完了)と残り時間の計算を `CountdownEngine` クラスに分離し、xUnitでTDD(Red→Green→Refactor)により実装した。`DispatcherTimer` には依存させず、`MainWindow.xaml.cs` が1秒ごとに `Tick()` を呼び出す構成にしている
- 状態は `CountdownState`(Stopped/Running/Paused/Completed)のenumで管理。「一時停止からの再開」は `Start()` を状態に応じて振る舞いを変える形で共通化した(Stopped/Pausedからのみ開始可能、Runningから0秒以下ではStartしない)
- 入力欄(時・分・秒)はStopped状態のときのみ編集可能。カウントダウン中(Running/Paused)や完了後(Completed)は無効化し、リセットで再び編集可能に戻す
- 完了通知は `MessageBox` に加えて `System.Media.SystemSounds.Exclamation.Play()` でサウンドも鳴らすようにした
- UI Automationで実機を操作し、スタート→一時停止(残り時間が進まないことを確認)→再開→完了(メッセージ表示・入力欄再有効化はリセットまで維持)→リセットの一連の流れを確認済み

## ステータス
- [ ] 未着手
- [ ] 実装中
- [x] 完成
