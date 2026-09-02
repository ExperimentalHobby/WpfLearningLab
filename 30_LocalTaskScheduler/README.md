# ローカルタスクスケジューラ

## 学習ポイント
バックグラウンドサービス的な処理、Windows通知(Toast)連携

## 概要
指定時刻(1回)または一定間隔(繰り返し)でタスクを実行し、Windowsトースト通知で知らせるローカルタスクスケジューラ。バックグラウンドの`System.Threading.Timer`でスケジュールを監視する。タスクトレイ常駐化(アプリ終了中もスケジュールを継続する仕組み)は完了条件に含まれないため、今回はスコープ外とした。

## 実装メモ
- タスクの期日判定を`TaskDueEvaluator`(タスク一覧+現在時刻から実行すべきタスクを判定する純粋ロジック)として切り出し、UIなし・実タイマー無しで決定的に単体テストした
- `IBackgroundTicker`/`ThreadingTimerTicker`が実の`System.Threading.Timer`をラップし、一定間隔ごとに`Ticked`イベントを発火する(学習ポイントの「バックグラウンドサービス的な処理」を実際のスレッドプールタイマーで実現)。実タイマー+タイムアウト付きの非同期テストで実際に発火することを検証した。イベントは背景スレッドで発火するため、`16_LocalChatApp`/`25_FileOrganizer`と同じ`IUiDispatcher`抽象でUIスレッドへマーシャリングした
- `MainViewModel`はタイマー自体を持たず、View側の`ThreadingTimerTicker`が`CheckDueTasks(DateTime.Now)`を呼び出す設計にした(`24_MusicPlayer`の`_positionTimer`、`27_NetworkMonitor`の`_sampleTimer`と同じ方針)。これによりフェイクの現在時刻を渡してOnce/Interval双方の実行ロジックを同期的にテストできた
- `ScheduledTask`は永続化を持たないアプリのため、`19_HabitTracker`の`HabitItem`のようなDBエンティティのUIラッパーではなく、`Models`名前空間に直接置いて`INotifyPropertyChanged`を自前実装した(`LastExecutedAt`/`IsEnabled`の変更をUIへ通知するため)。`Services`が`ViewModels`に依存する逆転を避ける目的
- **ハマった点(トースト通知ライブラリのTFM解決)**: `Microsoft.Toolkit.Uwp.Notifications`の`ToastContentBuilder.Show()`拡張メソッドはWindows Runtime依存のため、他アプリと同じ`net10.0-windows`(バージョン無し)ではNuGetがWindows非依存の`net5.0`アセットを選んでしまい`Show()`が見つからずビルドエラーになった。このアプリのみ`net10.0-windows10.0.19041.0`(Windows SDKバージョン付き)を指定することで解決した。あわせて、この構成で推移的に解決される`System.Drawing.Common 4.7.0`に既知の脆弱性(GHSA-rxg9-xrhp-64gj)があったため、`9.0.0`へ明示的に上書きした(SDKの「プラットフォーム提供パッケージのため不要」という誤検知警告(NU1510)のみ抑制)
- **既知の制限(トースト通知の表示確認)**: 実機のUI Automationで`CheckDueTasks`が実際に呼ばれタスクが実行されること(`ExecutionLog`への記録)は確認できたが、`ToastNotifier.Show()`自体は例外を投げずに完了するにもかかわらず、この環境ではトースト通知のバナーが画面上に描画されることを確認できなかった。当初アンパッケージアプリ向けのAUMID/COMサーバー登録(`DesktopNotificationManagerCompat.RegisterAumidAndComServer`)が必要かと考え実装したが、ライブラリの警告メッセージにより「Win32アプリではStart menuショートカットや登録が不要になった」ことが判明したため撤回した。`21_PaintTool`の`SaveFileDialog`等と同様、この環境ではOS側のUI要素(今回はシェル通知)が描画されないケースがあると考えられる

## ステータス
- [ ] 未着手
- [ ] 実装中
- [x] 完成
