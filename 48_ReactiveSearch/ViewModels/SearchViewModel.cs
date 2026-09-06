using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using ReactiveSearch.Services;

namespace ReactiveSearch.ViewModels;

/// <summary>
/// 検索テキストボックスのViewModel。入力のdebounce、非同期の重複チェック
/// (<see cref="INotifyDataErrorInfo"/>によるエラー表示)、検索結果の反映を行う。
/// </summary>
public partial class SearchViewModel : ObservableObject, INotifyDataErrorInfo, IDisposable
{
    private static readonly TimeSpan DebounceDelay = TimeSpan.FromMilliseconds(400);

    private readonly Debouncer _debouncer;
    private readonly SearchResultGuard _guard = new();
    private readonly IDuplicateNameValidator _validator;
    private readonly SearchService _searchService;
    private readonly List<string> _currentErrors = new();
    private CancellationTokenSource? _searchCts;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private bool _isSearching;

    [ObservableProperty]
    private bool _hasSearched;

    [ObservableProperty]
    private bool _hasNoResults;

    [ObservableProperty]
    private string? _errorMessage;

    /// <summary>検索結果一覧。</summary>
    public ObservableCollection<string> SearchResults { get; } = new();

    /// <inheritdoc />
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    /// <inheritdoc />
    public bool HasErrors => _currentErrors.Count > 0;

    public SearchViewModel()
        : this(
            new Debouncer(new DispatcherTimerScheduler(), DebounceDelay),
            new DuplicateNameValidator(new[] { "admin", "root", "test" }, TimeSpan.FromMilliseconds(300)),
            new SearchService(new[]
            {
                "Apple", "Banana", "Cherry", "Grape", "Lemon",
                "Mango", "Melon", "Orange", "Peach", "Strawberry",
            }))
    {
    }

    /// <summary>
    /// テスト等から依存関係を注入するためのコンストラクタ。
    /// </summary>
    public SearchViewModel(Debouncer debouncer, IDuplicateNameValidator validator, SearchService searchService)
    {
        _debouncer = debouncer;
        _validator = validator;
        _searchService = searchService;
    }

    partial void OnSearchTextChanged(string value)
    {
        _debouncer.Trigger(() => _ = RunSearchAsync(value));
    }

    private async Task RunSearchAsync(string query)
    {
        // 世代番号ガード(_guard)は「古い結果を画面に反映しない」ことしか保証しないため、
        // CancellationTokenSourceで実際に古いリクエストの処理自体を打ち切る。
        var previousCts = _searchCts;
        var cts = new CancellationTokenSource();
        _searchCts = cts;
        previousCts?.Cancel();
        previousCts?.Dispose();

        var requestVersion = _guard.BeginRequest();
        IsSearching = true;

        try
        {
            var error = await _validator.ValidateAsync(query, cts.Token);
            if (!_guard.IsCurrent(requestVersion))
            {
                return;
            }

            if (error != null)
            {
                SetErrors(new[] { error });
                SearchResults.Clear();
                HasNoResults = false;
                HasSearched = true;
                return;
            }

            SetErrors(Array.Empty<string>());

            var results = _searchService.Search(query);
            if (!_guard.IsCurrent(requestVersion))
            {
                return;
            }

            SearchResults.Clear();
            foreach (var result in results)
            {
                SearchResults.Add(result);
            }

            HasNoResults = results.Count == 0;
            HasSearched = true;
        }
        catch (OperationCanceledException)
        {
            // 新しい検索に置き換わっただけの正常な打ち切りのため、エラーとしては扱わない。
        }
        catch (Exception ex)
        {
            // Debouncer経由のfire-and-forget呼び出し(_ = RunSearchAsync(value))のため、
            // ここで捕捉しないと未処理のタスク例外としてアプリ全体がクラッシュしうる。
            // 検索処理自体は外部依存の失敗モードを狭く限定できないため、最終防御境界として広く捕捉する。
            if (_guard.IsCurrent(requestVersion))
            {
                ErrorMessage = $"検索中にエラーが発生しました: {ex.Message}";
            }
        }
        finally
        {
            if (_guard.IsCurrent(requestVersion))
            {
                IsSearching = false;
            }
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _debouncer.Dispose();
        _searchCts?.Cancel();
        _searchCts?.Dispose();
    }

    private void SetErrors(IReadOnlyList<string> errors)
    {
        _currentErrors.Clear();
        _currentErrors.AddRange(errors);
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(SearchText)));
        OnPropertyChanged(nameof(HasErrors));
        ErrorMessage = errors.Count > 0 ? errors[0] : null;
    }

    /// <inheritdoc />
    public IEnumerable GetErrors(string? propertyName)
    {
        return propertyName == nameof(SearchText) ? _currentErrors : Array.Empty<string>();
    }
}
