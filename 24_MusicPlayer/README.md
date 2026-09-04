# 音楽プレイヤー

## 学習ポイント
MediaElement、プレイリスト管理、非同期ファイル読込

## 概要
音楽ファイルを再生し、プレイリストを管理できるミュージックプレイヤー。

## 実装メモ
- 再生は`MediaElement`(WPF標準)を使用。命令的なAPI(Play/Pause/Stop、Position)のためViewModelから`IMediaPlayerController`抽象越しに操作し、実際の`MediaElement`ラップ実装(`MediaElementController`)はView層(コードビハインド)に閉じ込めた
- 曲送り・前送り・リピート・シャッフルの「次/前に再生すべきindexはどれか」を`PlaylistNavigator`という純粋な静的ロジックとして切り出し、リピートOFF/1曲/全曲・シャッフルON/OFFの組み合わせを単体テストした
- プレイリスト読み込みは`IAudioFileScanner`(対象フォルダ内の.mp3/.wav/.wmaファイルを`Task.Run`で非同期列挙)+`IFolderPicker`(既存アプリと同じ`OpenFolderDialog`抽象)、`LoadFolderCommand`は`AsyncRelayCommand`とした
- 再生位置はMediaElementが位置変化イベントを持たないため、コードビハインドの`DispatcherTimer`(250ms間隔)で`IMediaPlayerController.Position`をポーリングし、`MainViewModel.ReportPosition`でViewModelへ反映する。シークバー操作(ユーザー起点の変更)は`Position`プロパティのsetter経由で`IMediaPlayerController.Position`へ書き戻す、という2方向の更新経路を用意し、無限ループを防いだ
- **実機のUI Automation検証で発見した不具合**: `SelectTrackCommand`実行(曲の選択・再生開始)で`CurrentTrack`が変化しても、`PlayPauseCommand`/`StopCommand`の`CanExecuteChanged`を発火していなかったため、WPFの`Button`側で`IsEnabled`が再評価されず「再生/一時停止」「停止」ボタンが無効化されたままになる不具合があった。単体テストで`CanExecute()`を直接呼ぶだけでは(常に最新状態を返してしまうため)検出できず、実際に`Button`をUI Automationで操作して初めて判明した。`CanExecuteChanged`の発火有無を検証するテストを追加した上で修正した
- UI Automationでは`OpenFolderDialog`が本サンドボックス環境で表示されなかった(`21_PaintTool`の`SaveFileDialog`と同様の環境制約)。プレイリストへの読み込み経路自体は`MainViewModel`の単体テストで検証済みのため、実機での再生機能検証は環境変数経由でプレイリストを直接シードするデバッグコードを一時的に追加して実施し、検証後に削除した
- UI Automationで実機を操作し、以下を確認済み:
  - 曲を選択すると実際に`MediaElement`で再生が始まり、再生位置(Position)が時間経過とともに実際に進むこと
  - 一時停止で再生位置の進行が止まり、再度再生で再開すること
  - 「次へ」「前へ」で曲が切り替わること、停止でPlayer.Stopが呼ばれること
  - リピートモードの切り替え(Off→All)、シャッフルの切り替え(実際のマウスクリックで確認、`TogglePattern.Toggle()`は`Click`を発火させないため)
  - プレイリストの並び替え(↑/↓)・削除が反映されること

## ステータス
- [ ] 未着手
- [ ] 実装中
- [x] 完成
