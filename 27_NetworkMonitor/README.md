# ネットワーク帯域モニター

## 学習ポイント
PerformanceCounter、リアルタイムグラフ更新

## 概要
`PerformanceCounter`(`Network Interface`カテゴリ)でネットワークインターフェースの送受信帯域(Bytes Sent/sec・Received/sec)をリアルタイムに計測し、折れ線グラフで可視化するモニターアプリ。直近60件のみ保持しスクロール表示する。

## 実装メモ
- `INetworkBandwidthSampler`/`PerformanceCounterNetworkBandwidthSampler`が実の`System.Diagnostics.PerformanceCounter`をラップする。この環境で`PerformanceCounter`が実際に動作することは事前のスパイク確認で分かっていたため、モックを使わず実カウンターに対してテストした(値が例外を投げず0以上を返すことを検証)。`net10.0-windows`ではNuGetパッケージを追加しなくても`System.Diagnostics.PerformanceCounter`型が利用できた(明示的に`PackageReference`を追加すると`NU1510`警告が出たため削除した)
- `BandwidthHistory`(直近n件のみ保持するリングバッファ的な純粋ロジック)、`BandwidthChartModelBuilder`(履歴一覧から送信/受信2系列の`LineSeries`を持つ`OxyPlot.PlotModel`を組み立てる純粋ロジック)を`26_ChartVisualization`と同じ方針で切り出し、UIなしで単体テストした
- リアルタイム更新は`24_MusicPlayer`の`_positionTimer`と同じ方針で、`MainViewModel`自身はタイマーを持たず、View側の`DispatcherTimer`(1秒間隔)が`MainViewModel.Sample()`を呼び出す設計にした。これにより`MainViewModel`はフェイクの`INetworkBandwidthSampler`に対して`Sample()`を手動で呼ぶことで、リアルタイム更新ロジックを同期的に単体テストできる
- `26_ChartVisualization`で見つかった「`BarSeries`はCategoryAxisがY軸に無いと実描画時に例外を投げる」問題を踏まえ、今回は`LineSeries`+`LinearAxis`のみを使う構成にしたため、同様の軸配置の不整合は発生しなかった(UI Automationで実際に描画させて確認済み)
- UI Automationで実機を操作し、以下を確認済み:
  - 起動時、実際のネットワークインターフェース一覧(例: 「TP-Link Wireless USB Adapter」「Realtek Gaming 2.5GbE Family Controller」)がComboBoxに表示されること
  - インターフェース未選択の場合は「監視開始」ボタンが無効化され、選択すると有効化されること
  - 「監視開始」を押すと数秒おきにグラフが更新され(このマシンでは実トラフィックがほぼ無いため値は概ね0付近で推移)、例外なく描画されること。「監視停止」を押すとボタンの有効/無効が正しく切り替わること
  - 初回起動時、`PerformanceCounterCategory`の読み込みに数秒かかることがある(ウィンドウ表示が遅延する)ため、動作確認スクリプトはリトライ付きで待機するようにした

## ステータス
- [ ] 未着手
- [ ] 実装中
- [x] 完成
