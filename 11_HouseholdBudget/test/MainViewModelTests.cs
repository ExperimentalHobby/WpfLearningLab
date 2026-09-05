using HouseholdBudget.Models;
using HouseholdBudget.ViewModels;

namespace HouseholdBudget.Tests;

/// <summary>
/// <see cref="MainViewModel"/> の単体テスト。
/// </summary>
public class MainViewModelTests
{
	/// <summary>
	/// パス条件: 生成時にリポジトリの取引一覧がTransactionsに反映されること
	/// </summary>
	[Fact]
	public void コンストラクタ_リポジトリの取引一覧をTransactionsに読み込む()
	{
		var repository = new FakeTransactionRepository(
		[
			new Transaction { Date = new DateTime(2026, 8, 1), Type = TransactionType.Income, Category = "給与", Amount = 300000m },
			new Transaction { Date = new DateTime(2026, 8, 2), Type = TransactionType.Expense, Category = "食費", Amount = 1500m },
		]);

		var viewModel = new MainViewModel(repository);

		Assert.Equal(2, viewModel.Transactions.Count);
	}

	/// <summary>
	/// パス条件: 入力欄に有効な値を設定してAddCommandを実行すると、取引がリポジトリとTransactionsの両方に追加されること
	/// </summary>
	[Fact]
	public void AddCommand_有効な入力で実行すると取引が追加される()
	{
		var repository = new FakeTransactionRepository();
		var viewModel = new MainViewModel(repository)
		{
			InputDate = new DateTime(2026, 8, 8),
			InputType = TransactionType.Expense,
			InputCategory = "食費",
			InputAmount = "1500",
			InputMemo = "スーパー",
		};

		viewModel.AddCommand.Execute(null);

		Assert.Single(viewModel.Transactions);
		Assert.Single(repository.GetAll());
		Assert.Equal("食費", viewModel.Transactions[0].Category);
	}

	/// <summary>
	/// パス条件: 金額が0以下(または未入力)の場合、AddCommandが実行不可になること
	/// </summary>
	[Theory]
	[InlineData("0")]
	[InlineData("-100")]
	[InlineData("")]
	[InlineData("abc")]
	public void AddCommand_金額が不正な場合CanExecuteがfalseになる(string amount)
	{
		var repository = new FakeTransactionRepository();
		var viewModel = new MainViewModel(repository)
		{
			InputCategory = "食費",
			InputAmount = amount,
		};

		var canExecute = viewModel.AddCommand.CanExecute(null);

		Assert.False(canExecute);
	}

	/// <summary>
	/// パス条件: 金額は有効でもカテゴリが空(または空白のみ)の場合、AddCommandが実行不可になること
	/// </summary>
	[Theory]
	[InlineData("")]
	[InlineData("   ")]
	public void AddCommand_カテゴリが空の場合CanExecuteがfalseになる(string category)
	{
		var repository = new FakeTransactionRepository();
		var viewModel = new MainViewModel(repository)
		{
			InputCategory = category,
			InputAmount = "1500",
		};

		var canExecute = viewModel.AddCommand.CanExecute(null);

		Assert.False(canExecute);
	}

	/// <summary>
	/// パス条件: 取引を選択してDeleteCommandを実行すると、リポジトリとTransactionsの両方から削除されること
	/// </summary>
	[Fact]
	public void DeleteCommand_選択中の取引を実行すると削除される()
	{
		var repository = new FakeTransactionRepository(
		[
			new Transaction { Date = new DateTime(2026, 8, 1), Type = TransactionType.Income, Category = "給与", Amount = 300000m },
		]);
		var viewModel = new MainViewModel(repository)
		{
			SelectedTransaction = repository.GetAll()[0],
		};

		viewModel.DeleteCommand.Execute(null);

		Assert.Empty(viewModel.Transactions);
		Assert.Empty(repository.GetAll());
	}

	/// <summary>
	/// パス条件: 収入・支出が混在する取引一覧から、収入合計・支出合計・差引残高が正しく計算されること
	/// </summary>
	[Fact]
	public void 収支合計_収入と支出から正しく計算される()
	{
		var repository = new FakeTransactionRepository(
		[
			new Transaction { Date = new DateTime(2026, 8, 1), Type = TransactionType.Income, Category = "給与", Amount = 300000m },
			new Transaction { Date = new DateTime(2026, 8, 2), Type = TransactionType.Expense, Category = "食費", Amount = 1500m },
			new Transaction { Date = new DateTime(2026, 8, 3), Type = TransactionType.Expense, Category = "交通費", Amount = 500m },
		]);

		var viewModel = new MainViewModel(repository);

		Assert.Equal(300000m, viewModel.TotalIncome);
		Assert.Equal(2000m, viewModel.TotalExpense);
		Assert.Equal(298000m, viewModel.Balance);
	}

	/// <summary>
	/// パス条件: 同一カテゴリの取引が複数ある場合、カテゴリ別集計で符号付き金額が合算されること
	/// (docコメント通り、支出は負として合算される)
	/// </summary>
	[Fact]
	public void カテゴリ別集計_同一カテゴリの金額が符号付きで合算される()
	{
		var repository = new FakeTransactionRepository(
		[
			new Transaction { Date = new DateTime(2026, 8, 1), Type = TransactionType.Expense, Category = "食費", Amount = 1000m },
			new Transaction { Date = new DateTime(2026, 8, 2), Type = TransactionType.Expense, Category = "食費", Amount = 500m },
			new Transaction { Date = new DateTime(2026, 8, 3), Type = TransactionType.Expense, Category = "交通費", Amount = 300m },
		]);

		var viewModel = new MainViewModel(repository);

		Assert.Equal(-1500m, viewModel.CategorySummary.Single(c => c.Category == "食費").Amount);
		Assert.Equal(-300m, viewModel.CategorySummary.Single(c => c.Category == "交通費").Amount);
	}

	/// <summary>
	/// パス条件: 同一カテゴリに収入と支出が混在する場合、単純加算ではなく符号付きで
	/// 相殺された正味の金額が集計されること
	/// (収入5000・支出2000が同カテゴリの場合、単純加算の7000ではなく差額の3000になる)
	/// </summary>
	[Fact]
	public void カテゴリ別集計_収入と支出が同一カテゴリに混在する場合正味の金額が集計される()
	{
		var repository = new FakeTransactionRepository(
		[
			new Transaction { Date = new DateTime(2026, 8, 1), Type = TransactionType.Income, Category = "小遣い", Amount = 5000m },
			new Transaction { Date = new DateTime(2026, 8, 2), Type = TransactionType.Expense, Category = "小遣い", Amount = 2000m },
		]);

		var viewModel = new MainViewModel(repository);

		Assert.Equal(3000m, viewModel.CategorySummary.Single(c => c.Category == "小遣い").Amount);
	}

	/// <summary>
	/// パス条件: AddCommand実行後、集計プロパティ(TotalIncome/TotalExpense/Balance/CategorySummary)のPropertyChangedが発火すること
	/// </summary>
	[Fact]
	public void AddCommand_実行後に集計プロパティのPropertyChangedが発火する()
	{
		var repository = new FakeTransactionRepository();
		var viewModel = new MainViewModel(repository)
		{
			InputCategory = "食費",
			InputAmount = "1000",
		};
		var raisedPropertyNames = new List<string>();
		viewModel.PropertyChanged += (_, e) => raisedPropertyNames.Add(e.PropertyName ?? string.Empty);

		viewModel.AddCommand.Execute(null);

		Assert.Contains(nameof(MainViewModel.TotalIncome), raisedPropertyNames);
		Assert.Contains(nameof(MainViewModel.TotalExpense), raisedPropertyNames);
		Assert.Contains(nameof(MainViewModel.Balance), raisedPropertyNames);
		Assert.Contains(nameof(MainViewModel.CategorySummary), raisedPropertyNames);
	}

	/// <summary>
	/// パス条件: InputAmountを変更すると、AddCommandのCanExecuteChangedが発火し、ボタンの有効/無効がUIに追従すること
	/// </summary>
	[Fact]
	public void InputAmount_変更するとAddCommandのCanExecuteChangedが発火する()
	{
		var repository = new FakeTransactionRepository();
		var viewModel = new MainViewModel(repository);
		var raised = false;
		viewModel.AddCommand.CanExecuteChanged += (_, _) => raised = true;

		viewModel.InputAmount = "1000";

		Assert.True(raised);
	}

	/// <summary>
	/// パス条件: SelectedTransactionを変更すると、DeleteCommandのCanExecuteChangedが発火し、ボタンの有効/無効がUIに追従すること
	/// </summary>
	[Fact]
	public void SelectedTransaction_変更するとDeleteCommandのCanExecuteChangedが発火する()
	{
		var repository = new FakeTransactionRepository(
		[
			new Transaction { Date = new DateTime(2026, 8, 1), Type = TransactionType.Income, Category = "給与", Amount = 300000m },
		]);
		var viewModel = new MainViewModel(repository);
		var raised = false;
		viewModel.DeleteCommand.CanExecuteChanged += (_, _) => raised = true;

		viewModel.SelectedTransaction = repository.GetAll()[0];

		Assert.True(raised);
	}

	/// <summary>
	/// パス条件: AddCommand実行後、入力欄(カテゴリ・金額・メモ)がクリアされること(日付・種別は連続入力の利便性のため保持する)
	/// </summary>
	[Fact]
	public void AddCommand_実行後に入力欄がクリアされる()
	{
		var repository = new FakeTransactionRepository();
		var viewModel = new MainViewModel(repository)
		{
			InputCategory = "食費",
			InputAmount = "1500",
			InputMemo = "スーパー",
		};

		viewModel.AddCommand.Execute(null);

		Assert.Equal(string.Empty, viewModel.InputCategory);
		Assert.Equal(string.Empty, viewModel.InputAmount);
		Assert.Equal(string.Empty, viewModel.InputMemo);
	}
}
