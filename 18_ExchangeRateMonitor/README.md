# 株価/為替モニター

## 学習ポイント
API連携、定期更新(Timer+async)

## 概要
指定した通貨ペアの為替レートを定期的にAPIから取得して表示するモニターアプリ。

株価APIの多くはAPIキー登録が必須で、公開リポジトリでのキー管理が課題になるため、本アプリでは為替(FX)のみを対象とし、APIキー不要の[Frankfurter API](https://api.frankfurter.app)(ECB公表レート)を利用する。

## 実装メモ
- `FrankfurterExchangeRateApiClient`が`HttpClient`で`GET /latest?from={base}&to={quote}`を呼び出し、レスポンスJSONの`rates.{quote}`からレートを取得する。指定した通貨のレートが含まれない場合(存在しない通貨コード等)は`InvalidOperationException`をスローする
- `WatchedPairItem`はレート更新のたびに直前の`CurrentRate`を`PreviousRate`に退避し、新しい値と比較して`Trend`(Up/Down/Unchanged/初回はUnknown)を判定する。一覧では`DataTrigger`でTrendに応じてレート文字色を変える(上昇=緑、下落=赤)
- `MainViewModel.RefreshAllCommand`は登録済み全銘柄を順番に取得する。1銘柄あたり最大3回までリトライ(`HttpRequestException`/`TaskCanceledException`/`JsonException`/`InvalidOperationException`を対象)し、全て失敗した場合のみその銘柄に`ErrorMessage`を設定して次の銘柄へ処理を継続する。これにより一部銘柄のAPI障害が他銘柄の更新やアプリ全体の動作に影響しない
- リトライ間隔はコンストラクタで注入可能にし(既定1秒)、ユニットテストでは`TimeSpan.Zero`を渡してリトライ処理を高速に検証できるようにした
- 定期更新は`MainWindow`が保持する`DispatcherTimer`(既定30秒間隔)から`RefreshAllCommand`を呼び出すだけの薄い配線とし、ViewModel側は`DispatcherTimer`に依存しないことでユニットテスト可能にした。手動更新用の「今すぐ更新」ボタンも用意している
- 通貨コードの入力チェックは3文字の英字であることのみを検証し、実在する通貨コードかどうかはAPI呼び出しの成否(=エラー表示)に委ねている
- UI Automationで実機を操作し、以下を確認済み:
  - 通貨ペア(USD/JPY, EUR/JPY, 存在しないXXX/YYY)を登録すると一覧に反映されること
  - 「今すぐ更新」実行で実際にFrankfurter APIからレートを取得し、正常な2銘柄にはレートが表示され、存在しない銘柄はリトライ後にエラーメッセージが表示されること(この間アプリはクラッシュしない)
  - 最終更新日時が更新されること
  - 銘柄の削除が一覧に反映されること
  - 削除後に再度更新してもクラッシュせず、正常にレートが再取得されること

## ステータス
- [ ] 未着手
- [ ] 実装中
- [x] 完成
