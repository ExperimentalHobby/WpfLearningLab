# メモリリーク検証ラボ

## 学習ポイント
イベント購読とメモリリークの関係、WeakEventManager/弱イベントパターン、メモリ使用量の計測・確認方法

## 概要
イベント購読解除漏れによるメモリリークを意図的に再現し、`WeakEventManager`で修正した版と比較・確認できるアプリ。

## 実装メモ
- **リーク再現(Bad版)**: アプリ全体で共有される長寿命の`EventPublisher`が`SomethingChanged`イベントを持つ。
  短命な`LeakySubscriberViewModel`はコンストラクタで`publisher.SomethingChanged += OnChanged`と購読するが、
  解除処理を持たない。イベント購読は「購読される側(Publisher)が購読する側(Subscriber)を強参照で保持する」
  仕組みのため、UI側の参照を切ってもPublisherが握り続け、GCされない
- **修正版(Good版)**: `WeakSubscriberViewModel`は`WeakEventManager<EventPublisher, EventArgs>.AddHandler(publisher,
  nameof(EventPublisher.SomethingChanged), OnChanged)`で購読する。`WeakEventManager`はPublisher側に弱参照
  テーブルとして購読者を保持するため、UI側の参照を切ればGC対象になる
- **計測**: `LeakTracker`が`Track(object)`で対象を`WeakReference`のリストに登録し、`TotalCount`(登録総数)と
  `CountAlive()`(`WeakReference.IsAlive`がtrueの数)を提供する。追跡自体は弱参照なので、追跡することがGCの
  妨げにはならない
- **GC依存のテスト**: `CountAlive_AfterReferenceReleasedAndGc_BecomesZero`等、実際にGCの挙動を検証するテストは
  対象へのローカル参照をテストメソッド本体から`[MethodImpl(MethodImplOptions.NoInlining)]`の別メソッドに
  切り出し、JITによる変数の生存期間延長の影響を避けている。`GC.Collect()` → `GC.WaitForPendingFinalizers()`
  → `GC.Collect()`の3段構えで確実に到達不能オブジェクトを回収させる
- **UI**: モード切替(Bad版/Good版のRadioButton)、「生成」(購読者を10件生成)、「参照解放」(一覧側の強参照リストを
  Clearして参照を切る)、「GC実行」(強制GCして再計測)の3ボタンと、TotalCount/AliveCountの表示

## 動作確認(UI Automation)
- Bad版でGenerate(Total/Alive=10)→参照解放→GC実行を行っても`AliveCount`が10のまま変化しないことを確認
  (強参照購読によりPublisherが購読者を保持し続け、リークが再現されている)
- 続けてGood版に切り替えてGenerate(Total=20)→参照解放→GC実行を行うと`AliveCount`が10まで減ることを確認
  (先にBad版で生成した10件は生存し続け、Good版で生成した10件のみが正しく解放された)
- 上記をスクリーンショットで実機確認済み

## ステータス
- [ ] 未着手
- [ ] 実装中
- [x] 完成
