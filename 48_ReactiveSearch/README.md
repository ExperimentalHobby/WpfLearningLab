# リアクティブ検索アプリ

## 学習ポイント
CommunityToolkit.Mvvm(Source Generator)、入力のdebounce制御、INotifyDataErrorInfoによる非同期バリデーション

## 概要
検索テキストボックスへの入力からdebounceで検索を実行し、予約語の非同期重複チェック(INotifyDataErrorInfo)と、古い検索結果を無視するキャンセル制御を行うサンプルアプリ。

## 実装メモ
- `CommunityToolkit.Mvvm` の `ObservableObject` + `[ObservableProperty]` でViewModelを構築した。`SearchText` の変更検知はソースジェネレータが生成する `partial void OnSearchTextChanged(string value)` フックで行っている
- debounce・キャンセル・非同期バリデーション・検索処理はいずれも時間や外部依存を抽象化してTDD対象に分離した(xUnitでRed→Green→Refactor)
  - `IScheduler`/`DispatcherTimerScheduler`: `DispatcherTimer` を薄くラップし、`Schedule(delay, action)` の戻り値`IDisposable`を`Dispose`すると発火をキャンセルできるようにした
  - `Debouncer`: `IScheduler` を受け取り、`Trigger` が呼ばれるたびに直前のスケジュールをキャンセルして最後の呼び出しだけを有効にする。テストは実際の時間経過を待たず、Fakeの`IScheduler`で「直前の呼び出し分がキャンセルされたか」を検証している
  - `SearchResultGuard`: リクエストのたびに世代番号を発行し、非同期処理完了時にその番号が最新かどうかで「結果を反映してよいか」を判定する。新しい検索が始まった後に古い検索が完了しても結果は無視される(実質的なキャンセル)
  - `DuplicateNameValidator`: 予約語(`admin`/`root`/`test`)との重複を`Task.Delay`でサーバー往復を模して非同期にチェックする
- `SearchViewModel` は `INotifyDataErrorInfo` を自前で実装し、`SearchText` にエラーがある間は `ErrorsChanged` を発火してWPFの標準の検証UI(赤枠)を表示させつつ、`ErrorMessage`/`HasErrors` プロパティも公開してエラーメッセージ表示・該当なし表示の判定に使っている
- 検索結果0件と非同期バリデーションエラーは状態が競合しないよう、エラー時は `HasNoResults` を立てないようにしている
- UI Automationで実機を操作し、以下を確認済み: (1) 入力直後は結果が出ず、debounce遅延後に検索結果が反映されること、(2) 予約語(`admin`)を入力すると赤枠+エラーメッセージが表示されること、(3) 該当候補がない場合に専用メッセージが表示されること、(4) `a`→`ap`→`app`→`appl`→`apple` と高速に連続入力しても、最終的に最後の入力(`apple`)の結果だけが反映されること(古い検索結果が無視される)

## ステータス
- [ ] 未着手
- [ ] 実装中
- [x] 完成
