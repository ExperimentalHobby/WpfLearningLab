# ASP.NET Core APIサーバー タスク管理ワークベンチ

## 学習ポイント
ASP.NET Core Minimal APIでのサーバー構築、EF Core(InMemory)、HttpClientでの外部API呼び出し、async/await、クライアント/サーバー構成

## 概要
ASP.NET Core Minimal APIで構築したタスク管理REST APIサーバーと、それを`HttpClient`で利用するWPFクライアント。[55_SelfHostedNoteSync](../55_SelfHostedNoteSync)(HttpListener自作サーバー)との対比で、フレームワークを使った本格的なAPI構成を学ぶ題材。`56_TaskApiWorkbench/Server`がASP.NET Core Minimal APIサーバー、`56_TaskApiWorkbench/Client`がWPFクライアント。

## 実装メモ
- サーバーは`app.MapGroup("/api/tasks")`でルートをグルーピングし、GET(一覧/単体)・POST(作成)・PUT(更新)・PATCH(完了切替)・DELETEの6エンドポイントを`TaskEndpoints.MapTaskEndpoints`に集約した
- データストアはEF Core InMemoryプロバイダ(`TaskDbContext`)。学習用のため永続化(実DB)は対象外
- タイトル未入力時は`Results.ValidationProblem`で400を返す。存在しないIDへのGET/PUT/PATCH/DELETEは404を返す
- サーバーのエンドポイントは`Microsoft.AspNetCore.Mvc.Testing`の`WebApplicationFactory<Program>`を使い、実際にHTTPリクエストを送信する結合テストで検証した
- **見つけて直したバグ1: WebApplicationFactoryのテスト用DB名がリクエストごとに変わる** — `AddDbContext`の`optionsAction`(`options => options.UseInMemoryDatabase(Guid.NewGuid().ToString())`)はスコープ(リクエスト)ごとに再実行されるため、ラムダ内で直接`Guid.NewGuid()`を呼ぶとリクエストのたびに別々のInMemory DBが使われてしまい、作成直後のタスクが「見つからない(404)」という不可解な結果になった。ラムダの外で1回だけ生成した値をキャプチャするよう修正
- **見つけて直したバグ2: `IClassFixture`でテスト間のデータが混ざる** — バグ1の修正でDB名を固定した結果、今度はテストクラス内の全テストが同じDBを共有し、「タスクが無ければ空」のようなテストが他テストの作成データに汚染されて失敗した。`IClassFixture`をやめ、テストメソッドごとに新しい`TaskApiFactory`(＝新しいDB)を生成するように変更
- クライアント(`TaskApiClient`)は[55_SelfHostedNoteSync](../55_SelfHostedNoteSync)と同様、`HttpClient`をコンストラクタ注入可能にし、疑似`HttpMessageHandler`でJSON応答・送信リクエスト(メソッド・URL・ボディ)を検証した
- クライアントのMVVM基盤は55と同じ`ObservableObject`/`AsyncRelayCommand`パターンを踏襲。55の実装で見つかった「`ObservableCollection`要素を新しい参照に置き換えるとListBoxの選択が失われる」「`IsBusy`の変更を他コマンドの`CanExecuteChanged`に通知しないとボタンの有効/無効が固まる」という2つの教訓を最初から反映し、今回は実機確認で同種の不具合は発生しなかった
- UI Automationで実機を操作し、以下を確認済み:
  - サーバー・クライアントを起動し、タスクの追加→一覧反映→選択→編集→更新→完了切替→削除の一連の操作結果をサーバー側API(`curl`)でも突き合わせて確認
  - サーバーを停止した状態で「再読み込み」を押すとエラーメッセージが表示されること

## ステータス
- [ ] 未着手
- [ ] 実装中
- [x] 完成
