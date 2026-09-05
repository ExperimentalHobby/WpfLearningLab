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
public partial class SearchViewModel : ObservableObject, INotifyDataErrorInfo
{
    private static readonly TimeSpan DebounceDelay = TimeSpan.FromMilliseconds(400);

    private readonly Debouncer _debouncer;
    private readonly SearchResultGuard _guard = new();
    private readonly DuplicateNameValidator _validator;
    private readonly SearchService _searchService;
    private readonly List<string> _currentErrors = new();

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
    public SearchViewModel(Debouncer debouncer, DuplicateNameValidator validator, SearchService searchService)
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
        var requestVersion = _guard.BeginRequest();
        IsSearching = true;

        var error = await _validator.ValidateAsync(query, CancellationToken.None);
        if (!_guard.IsCurrent(requestVersion))
        {
            return;
        }

        if (error != null)
        {
            SetErrors(new[] { error });
            SearchResults.Clear();
            HasNoResults = false;
            IsSearching = false;
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
        IsSearching = false;
        HasSearched = true;
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
