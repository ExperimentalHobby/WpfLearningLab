# 大量ログビューア(仮想化)

## 学習ポイント
VirtualizingStackPanel、UI仮想化、大量データのパフォーマンス最適化

## 概要
数千〜数十万行規模のダミーログを生成し、UI仮想化(`VirtualizingStackPanel`)を活用して軽快にスクロール表示するビューア。キーワード・ログレベルによる絞り込み、行番号ジャンプに対応する。

## 実装メモ
- `ListView`の`ItemsPanel`に`VirtualizingStackPanel`を指定し、`VirtualizingPanel.IsVirtualizing`/`VirtualizationMode="Recycling"`/`ScrollViewer.CanContentScroll="True"`を設定した。`CanContentScroll`を`False`のままにすると仮想化がほぼ無効化される(ピクセル単位スクロールに切り替わり全件実体化されやすくなる)ため、明示的に`True`を指定する必要があった
- 大量データの生成(`DummyLogFileGenerator`/`MainViewModel.GenerateAsync`)と絞り込み(`LogLineFilter`)は`Task.Run`でバックグラウンドスレッドに逃がし、UIスレッドをブロックしないようにした
- 大量行のファイル読み込み(`LogFileLoader`)は`File.ReadLines`(遅延評価のストリーミングAPI)を使い、`File.ReadAllLines`のようにファイル全体を一度にメモリへ読み込まない設計にした
- **パフォーマンス実測(仮想化の効果)**: `VisualTreeHelper`でUI仮想化パネル配下に実際に生成されている`ListViewItem`/`DataItem`要素数を数える機能を実装し、実機で比較した。3,000件のデータに対し、**仮想化ON時は実体化コンテナ数19個**(画面に収まる行数程度)、**仮想化OFF時は3,000個全件が実体化**されることを確認した。仮想化により、スクロール位置に関わらず実際にUI要素として生成されるのは常に画面に映る分だけであることが数値で裏付けられた
- **ハマった点(`IsVirtualizing`を実行中に切り替えても遡って反映されない)**: 一度仮想化ONの状態で描画された`ListView`に対し、実行中に`VirtualizingPanel.IsVirtualizing`を`False`へバインディング経由で切り替えても、既に実体化済み/破棄済みだったコンテナ数はすぐには変化しなかった(切り替え後すぐ数えても19のままだった)。生成前にOFFへ切り替えてから生成した場合は正しく全件(3,000個)実体化されることを確認できたため、`IsVirtualizing`は実行時の動的な切り替えよりも、データ投入前に確定させておくプロパティとして扱うべきだと分かった
- **ハマった点(実機バグ: ジャンプボタンが有効化されない)**: `JumpToLineCommand`の`CanExecute`は`DisplayedLines.Count > 0`だが、`DisplayedLines`更新後に`RaiseCanExecuteChanged()`を呼び忘れていたため、生成直後にUI Automationからジャンプボタンを押すと`ElementNotEnabledException`で失敗した(WPFの`CommandManager`による自動再クエリはキーボード/マウス操作等のタイミングに依存するため、プログラム的な更新直後は必ずしも即座に反映されない)。`DisplayedLines`更新時に明示的に`RaiseCanExecuteChanged()`を呼ぶよう修正し、回帰テストを追加した
- 行番号ジャンプは、フィルタ適用中は元の行番号と表示位置が一致しないため、単純なインデックス指定ではなく「表示中のリストから一致する`LogLine.LineNumber`を探す」方式(`LineJumpCalculator.FindDisplayIndex`)にした

## 動作確認(UI Automation)
- 3,000〜10,000件のダミーログを生成し、数十msで完了することを確認
- 仮想化ON/OFFそれぞれで生成し、実体化コンテナ数が19個 vs 全件(3,000個)になることを確認
- キーワード・ログレベルでの絞り込みが正しい件数に絞り込まれることを確認
- 10,000件生成後、9999行目へジャンプすると該当行が画面内に表示されることを確認(スクロール後も実体化コンテナ数は19個のまま=仮想化を維持したままジャンプできている)

## ステータス
- [ ] 未着手
- [ ] 実装中
- [x] 完成
