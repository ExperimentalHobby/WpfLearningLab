# 複数ステップ設定ウィザード

## 学習ポイント
Frame/Page/NavigationServiceによる画面遷移、ページ間での入力状態の受け渡し、戻る/進む/キャンセルのナビゲーション制御

## 概要
Frame + Page + NavigationService による3ステップ(基本情報→詳細設定→確認)の設定ウィザードアプリ。

## 実装メモ
- 各ステップの入力検証ロジックを `WizardValidationEngine` クラスに分離し、xUnitでTDD(Red→Green→Refactor)により実装した
- 入力値は `Models/WizardState` に集約し、`MainWindow` がインスタンスを1つ保持して各Pageのコンストラクタへ渡すことで、ページ間をまたいだ状態保持を実現している
- Step1(氏名・メールアドレス)・Step2(部署・通知有無・コメント)はそれぞれ「次へ」ボタン押下時に `WizardValidationEngine` で検証し、不正な場合はページ遷移せずエラーメッセージを表示する。メールアドレスの形式チェックは正規表現ではなく `@` と `.` の位置関係を見る簡易チェックにしている
- 「戻る」は `NavigationService.GoBack()` を使用。WPFのFrameナビゲーションはオブジェクト単位でNavigateしたページインスタンスをジャーナルに保持するため、GoBackで戻ると同一インスタンスに戻り、ComboBoxの選択状態やTextBoxの入力値がそのまま保持される
- Step3の「キャンセル」は `WizardState.Reset()` で状態を初期化した上でStep1へ新規Navigateし、ウィザードを最初からやり直せるようにしている(履歴には残らないため、キャンセル後に戻るボタンは表示されない設計)
- UI Automationで実機を操作し、Step1/Step2の未入力・不正入力時のエラー表示、正しい入力での遷移、Step3のサマリ表示、戻るボタンでの入力保持、キャンセルによるリセットを確認済み

## ステータス
- [ ] 未着手
- [ ] 実装中
- [x] 完成
