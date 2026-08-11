# 天気予報アプリ

## 学習ポイント
HttpClientで外部API呼び出し、async/await

## 概要
地名を入力すると、現在の天気(気温・天候・湿度・風速)を取得して表示するアプリ。

## 実装メモ
- 天気データの取得元には[Open-Meteo](https://open-meteo.com/)を採用した。APIキー登録不要の無料APIのため、完了条件にある「APIキー等の秘匿情報がリポジトリにコミットされていない」は構成上自動的に満たされる(秘匿すべきAPIキー自体が存在しない)
- 地名→緯度経度の[ジオコーディングAPI](https://geocoding-api.open-meteo.com/)と、緯度経度→現在の天気を返す[予報API](https://api.open-meteo.com/)の2段階でHttpClientを使い非同期(`async`/`await`)に呼び出す構成にした
- MVVM基盤(`ObservableObject`)は他アプリと同様のパターンを自前実装。コマンドは非同期処理向けに`AsyncRelayCommand`を新規実装し、実行中は`CanExecute`がfalseを返すことで多重実行(検索ボタンの連打)を防止する
- `WeatherCodeMapper`でOpen-Meteoが返すWMO(世界気象機関)天候コードを日本語の天候名・絵文字アイコンに変換する。純粋な変換ロジックのため、実際のAPIレスポンスに依存せずユニットテストできる
- `OpenMeteoWeatherApiClient`は`HttpClient`をコンストラクタ注入可能にし、テストでは実ネットワーク通信を行わず疑似`HttpMessageHandler`でJSON応答を差し替えて検証した(外部サービスの可用性に依存しない確定的なテスト)
- エラーハンドリング: 地名が見つからない場合は`ErrorMessage`にメッセージを設定。`HttpRequestException`/`TaskCanceledException`/`JsonException`を捕捉し、通信失敗時も同様にエラーメッセージを表示する。再検索時は前回のエラー表示をクリアする
- UI Automationで実機を操作し、以下を確認済み:
  - 実際にOpen-Meteo APIを呼び出し、「Tokyo」検索で地名(東京都)・気温・天候・湿度・風速が正しく表示されること
  - 存在しない地名を検索するとエラーメッセージが表示されること
  - 検索欄が空欄の場合、検索ボタンが無効化されること
- **確認できた制約**: 本実行環境ではこのウィンドウのスクリーンショット取得(`CopyFromScreen`・`PrintWindow`のいずれも)が常に白紙になる現象が発生した。常時表示される検索欄・ボタンも同様に白紙だったため、アプリ側の描画不具合ではなくキャプチャツール側の環境制約と判断し、UI Automationによるプロパティ読み取り(地名・気温・天候・湿度・風速の実際の値を取得)を実機動作確認の代替手段とした

## ステータス
- [ ] 未着手
- [ ] 実装中
- [x] 完成
