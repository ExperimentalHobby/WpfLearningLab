using System.Collections.ObjectModel;
using HouseholdBudget.Data;
using HouseholdBudget.Models;

namespace HouseholdBudget.ViewModels;

/// <summary>
/// カテゴリ別集計の1行分(カテゴリ名と金額合計)。
/// </summary>
/// <param name="Category">カテゴリ名。</param>
/// <param name="Amount">当該カテゴリの金額合計。</param>
public record CategoryAmount(string Category, decimal Amount);

/// <summary>
/// 家計簿アプリのメイン画面のViewModel。取引の一覧表示・追加・削除・集計を担う。
/// </summary>
public class MainViewModel : ObservableObject
{
	private readonly ITransactionRepository _repository;

	private DateTime _inputDate = DateTime.Today;
	private TransactionType _inputType = TransactionType.Expense;
	private string _inputCategory = string.Empty;
	private string _inputAmount = string.Empty;
	private string _inputMemo = string.Empty;
	private Transaction? _selectedTransaction;

	/// <summary>
	/// ViewModelを初期化し、リポジトリから取引一覧を読み込む。
	/// </summary>
	/// <param name="repository">取引データの永続化を担うリポジトリ。</param>
	public MainViewModel(ITransactionRepository repository)
	{
		_repository = repository;
		Transactions = new ObservableCollection<Transaction>(_repository.GetAll());
		AddCommand = new RelayCommand(Add, CanAdd);
		DeleteCommand = new RelayCommand(Delete, CanDelete);
	}

	/// <summary>
	/// 表示中の取引一覧。
	/// </summary>
	public ObservableCollection<Transaction> Transactions { get; }

	/// <summary>種別選択コンボボックス用の全選択肢。</summary>
	public IReadOnlyList<TransactionType> TransactionTypeOptions { get; } = Enum.GetValues<TransactionType>();

	/// <summary>入力フォーム: 取引日。</summary>
	public DateTime InputDate
	{
		get => _inputDate;
		set => SetProperty(ref _inputDate, value);
	}

	/// <summary>入力フォーム: 種別(収入/支出)。</summary>
	public TransactionType InputType
	{
		get => _inputType;
		set => SetProperty(ref _inputType, value);
	}

	/// <summary>入力フォーム: カテゴリ名。</summary>
	public string InputCategory
	{
		get => _inputCategory;
		set
		{
			if (SetProperty(ref _inputCategory, value))
			{
				AddCommand.RaiseCanExecuteChanged();
			}
		}
	}

	/// <summary>入力フォーム: 金額(文字列。数値変換前のテキストボックス直結値)。</summary>
	public string InputAmount
	{
		get => _inputAmount;
		set
		{
			if (SetProperty(ref _inputAmount, value))
			{
				AddCommand.RaiseCanExecuteChanged();
			}
		}
	}

	/// <summary>入力フォーム: メモ。</summary>
	public string InputMemo
	{
		get => _inputMemo;
		set => SetProperty(ref _inputMemo, value);
	}

	/// <summary>収入合計。</summary>
	public decimal TotalIncome => Transactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);

	/// <summary>支出合計。</summary>
	public decimal TotalExpense => Transactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);

	/// <summary>差引残高(収入合計-支出合計)。</summary>
	public decimal Balance => Transactions.Sum(t => t.SignedAmount);

	/// <summary>カテゴリ別の金額合計(表示用の符号付き金額で集計)。</summary>
	public IReadOnlyList<CategoryAmount> CategorySummary =>
		Transactions
			.GroupBy(t => t.Category)
			.Select(g => new CategoryAmount(g.Key, g.Sum(t => t.SignedAmount)))
			.ToList();

	/// <summary>DataGridで選択中の取引。</summary>
	public Transaction? SelectedTransaction
	{
		get => _selectedTransaction;
		set
		{
			if (SetProperty(ref _selectedTransaction, value))
			{
				DeleteCommand.RaiseCanExecuteChanged();
			}
		}
	}

	/// <summary>
	/// 入力フォームの内容を取引として追加するコマンド。
	/// </summary>
	public RelayCommand AddCommand { get; }

	/// <summary>
	/// 選択中の取引を削除するコマンド。
	/// </summary>
	public RelayCommand DeleteCommand { get; }

	private bool CanAdd() =>
		!string.IsNullOrWhiteSpace(InputCategory) &&
		decimal.TryParse(InputAmount, out var amount) && amount > 0;

	private void Add()
	{
		var transaction = new Transaction
		{
			Date = InputDate,
			Type = InputType,
			Category = InputCategory,
			Amount = decimal.Parse(InputAmount),
			Memo = InputMemo,
		};

		_repository.Add(transaction);
		Transactions.Add(transaction);
		OnSummaryChanged();

		InputCategory = string.Empty;
		InputAmount = string.Empty;
		InputMemo = string.Empty;
	}

	private bool CanDelete() => SelectedTransaction is not null;

	private void Delete()
	{
		if (SelectedTransaction is null)
		{
			return;
		}

		_repository.Delete(SelectedTransaction.Id);
		Transactions.Remove(SelectedTransaction);
		SelectedTransaction = null;
		OnSummaryChanged();
	}

	private void OnSummaryChanged()
	{
		OnPropertyChanged(nameof(TotalIncome));
		OnPropertyChanged(nameof(TotalExpense));
		OnPropertyChanged(nameof(Balance));
		OnPropertyChanged(nameof(CategorySummary));
	}
}
