using ReactiveSearch.Services;
using ReactiveSearch.ViewModels;

namespace ReactiveSearch.Tests;

/// <summary>
/// <see cref="SearchViewModel"/> のテスト。
/// </summary>
public class SearchViewModelTests
{
    private static SearchViewModel CreateViewModel(FakeScheduler scheduler, FakeDuplicateNameValidator validator) =>
        new(
            new Debouncer(scheduler, TimeSpan.FromMilliseconds(300)),
            validator,
            new SearchService(new[] { "Apple", "Banana" }));

    /// <summary>
    /// パス条件: 検証処理が例外をスローしても、クラッシュせず最終的にIsSearchingがfalseに戻ること
    /// (fire-and-forgetの検索処理に例外処理が無く、未処理タスク例外でクラッシュしうる不具合の回帰テスト)。
    /// </summary>
    [Fact]
    public async Task SearchText変更時_検証が例外をスローしてもIsSearchingは最終的にfalseに戻る()
    {
        var scheduler = new FakeScheduler();
        var validator = new FakeDuplicateNameValidator { ExceptionToThrow = new InvalidOperationException("boom") };
        var viewModel = CreateViewModel(scheduler, validator);

        var exception = Record.Exception(() =>
        {
            viewModel.SearchText = "a";
            scheduler.Calls[0].Action();
        });

        Assert.Null(exception);
        // 例外は内部でawaitされる非同期処理から発生するため、完了を待つ。
        for (var i = 0; i < 20 && viewModel.IsSearching; i++)
        {
            await Task.Delay(20);
        }

        Assert.False(viewModel.IsSearching);
    }

    /// <summary>
    /// パス条件: 新しい検索が開始されると、直前の検証呼び出しに渡されたCancellationTokenが
    /// キャンセルされること(CancellationToken.Noneを渡していたため実際にはキャンセルされず、
    /// 古いリクエストの処理が走り続けていた不具合の回帰テスト)。
    /// </summary>
    [Fact]
    public void SearchText変更時_直前の検証のCancellationTokenがキャンセルされる()
    {
        var scheduler = new FakeScheduler();
        var validator = new FakeDuplicateNameValidator();
        var viewModel = CreateViewModel(scheduler, validator);

        viewModel.SearchText = "a";
        scheduler.Calls[0].Action();
        var firstToken = validator.LastToken;
        Assert.False(firstToken.IsCancellationRequested);

        viewModel.SearchText = "ap";
        scheduler.Calls[1].Action();

        Assert.True(firstToken.IsCancellationRequested);
    }
}
