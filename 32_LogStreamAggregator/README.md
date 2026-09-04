# ログストリーム集計ツール

## 学習ポイント
System.Threading.Channels、Producer-Consumerパターン、非同期ストリーム処理

## 概要
疑似ログプロデューサが生成するログ行を`System.Threading.Channels`によるProducer-Consumerパイプラインで非同期に受け取り、ログレベル別件数・キーワード出現回数をリアルタイムに集計・表示するツール。

## 実装メモ
- 集計ロジック(`LogAggregator`)はチャネルから完全に独立させた。レベル別件数・キーワード出現回数の更新は同期的な`Add(LogEntry)`のみで完結するため、非同期処理を一切介さず高速・決定的にユニットテストできる
- Producer-Consumer本体(`LogStreamPipeline`)は`Channel.CreateBounded<LogEntry>`をラップし、`ChannelReader<T>.ReadAllAsync()`(`IAsyncEnumerable<T>`)による非同期ストリーム消費を`ConsumeAsync`に閉じ込めた
- バックプレッシャー(学習ポイントの一つ)は、容量1のチャネルに3件書き込むテストで検証した。Consumerを起動しない状態では`ProduceAsync`のTaskが200ms待っても完了しない(`FullMode = BoundedChannelFullMode.Wait`により2件目以降の書き込みが空きを待つ)ことを確認し、Consumerを起動すると初めて完了することを確認した
- View側は`MainViewModel.Start()`でProducerタスク(150ms間隔でダミーログ生成)とConsumerタスク(集計してUIへ反映)を`_ = RunXxxAsync(...)`で起動する`fire-and-forget`方式にした。UIスレッド外で動くConsumerからのプロパティ更新は`IUiDispatcher`(`16_LocalChatApp`等と同じパターン)でマーシャリングしている
- ダミーログ生成(`DummyLogGenerator`)はログレベルとメッセージ本文を独立に抽選している(実運用のログでは相関するのが自然だが、疑似データ生成の簡略化として割り切った)。同一シードの`Random`を渡せば決定的に同じ系列を生成するため、生成ロジック自体もテスト可能にした

## 動作確認(UI Automation)
- 開始→2秒待機で、総件数・レベル別件数(Debug/Info/Warning/Error)・キーワード出現回数(Exception/Timeout/Retry)がいずれも0から増加することをAutomationId経由で確認
- 直近ログ一覧に実際に生成された行(例: `[07:14:41] Info: Unhandled Exception in worker thread`)が表示されることを確認
- 停止後は総件数が増加しなくなり(1秒待っても値が変わらない)、Producer/Consumerが正しく停止することを確認

## ステータス
- [ ] 未着手
- [ ] 実装中
- [x] 完成
