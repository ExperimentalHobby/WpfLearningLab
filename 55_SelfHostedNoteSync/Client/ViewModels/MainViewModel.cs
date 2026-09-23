using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text.Json;
using SelfHostedNoteSync.Client.Models;
using SelfHostedNoteSync.Client.Services;

namespace SelfHostedNoteSync.Client.ViewModels;

/// <summary>
/// メモ同期クライアントのメイン画面のViewModel。メモの一覧表示・追加・更新・削除・エラー処理を担う。
/// </summary>
public class MainViewModel : ObservableObject
{
	private readonly INoteApiClient _apiClient;

	private Note? _selectedNote;
	private string _titleInput = string.Empty;
	private string _contentInput = string.Empty;
	private bool _isBusy;
	private string _errorMessage = string.Empty;

	/// <summary>
	/// ViewModelを初期化する。
	/// </summary>
	/// <param name="apiClient">メモの取得・作成・更新・削除に使うクライアント。</param>
	public MainViewModel(INoteApiClient apiClient)
	{
		_apiClient = apiClient;
		LoadCommand = new AsyncRelayCommand(LoadAsync, () => !IsBusy);
		AddCommand = new AsyncRelayCommand(AddAsync, CanAdd);
		UpdateCommand = new AsyncRelayCommand(UpdateAsync, CanUpdateOrDelete);
		DeleteCommand = new AsyncRelayCommand(DeleteAsync, CanUpdateOrDelete);
	}

	/// <summary>取得済みのメモ一覧。</summary>
	public ObservableCollection<Note> Notes { get; } = [];

	/// <summary>一覧で選択中のメモ。未選択の場合は<see langword="null"/>。</summary>
	public Note? SelectedNote
	{
		get => _selectedNote;
		set
		{
			if (SetProperty(ref _selectedNote, value))
			{
				TitleInput = value?.Title ?? string.Empty;
				ContentInput = value?.Content ?? string.Empty;
				UpdateCommand.RaiseCanExecuteChanged();
				DeleteCommand.RaiseCanExecuteChanged();
			}
		}
	}

	/// <summary>メモ追加・更新フォームのタイトル入力欄。</summary>
	public string TitleInput
	{
		get => _titleInput;
		set
		{
			if (SetProperty(ref _titleInput, value))
			{
				AddCommand.RaiseCanExecuteChanged();
				UpdateCommand.RaiseCanExecuteChanged();
			}
		}
	}

	/// <summary>メモ追加・更新フォームの本文入力欄。</summary>
	public string ContentInput
	{
		get => _contentInput;
		set => SetProperty(ref _contentInput, value);
	}

	/// <summary>通信中かどうか。</summary>
	public bool IsBusy
	{
		get => _isBusy;
		private set
		{
			if (SetProperty(ref _isBusy, value))
			{
				// IsBusyは全コマンドのCanExecuteが参照する共有状態。
				// ここで各コマンドに通知しないと、他コマンド実行中の変更通知に相乗りして
				// IsBusy=trueのタイミングで最後にCanExecuteChangedが発火したコマンドは、
				// falseに戻った後もWPFのButton.IsEnabledが古い判定のまま固まってしまう。
				LoadCommand.RaiseCanExecuteChanged();
				AddCommand.RaiseCanExecuteChanged();
				UpdateCommand.RaiseCanExecuteChanged();
				DeleteCommand.RaiseCanExecuteChanged();
			}
		}
	}

	/// <summary>エラーメッセージ。エラーが無い場合は空文字。</summary>
	public string ErrorMessage
	{
		get => _errorMessage;
		private set => SetProperty(ref _errorMessage, value);
	}

	/// <summary>メモ一覧を再取得するコマンド。</summary>
	public AsyncRelayCommand LoadCommand { get; }

	/// <summary>入力欄の内容でメモを新規作成するコマンド。</summary>
	public AsyncRelayCommand AddCommand { get; }

	/// <summary>選択中のメモを入力欄の内容で更新するコマンド。</summary>
	public AsyncRelayCommand UpdateCommand { get; }

	/// <summary>選択中のメモを削除するコマンド。</summary>
	public AsyncRelayCommand DeleteCommand { get; }

	private bool CanAdd() => !IsBusy && !string.IsNullOrWhiteSpace(TitleInput);

	private bool CanUpdateOrDelete() => !IsBusy && SelectedNote is not null;

	private async Task LoadAsync()
	{
		IsBusy = true;
		ErrorMessage = string.Empty;
		try
		{
			var notes = await _apiClient.GetNotesAsync();
			Notes.Clear();
			foreach (var note in notes)
			{
				Notes.Add(note);
			}
		}
		catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
		{
			ErrorMessage = "メモ一覧の取得に失敗しました。サーバーが起動しているか確認してください。";
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task AddAsync()
	{
		IsBusy = true;
		ErrorMessage = string.Empty;
		try
		{
			var created = await _apiClient.CreateNoteAsync(TitleInput, ContentInput);
			Notes.Add(created);
			TitleInput = string.Empty;
			ContentInput = string.Empty;
		}
		catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException or InvalidOperationException)
		{
			ErrorMessage = "メモの作成に失敗しました。サーバーが起動しているか確認してください。";
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task UpdateAsync()
	{
		if (SelectedNote is null)
		{
			return;
		}

		IsBusy = true;
		ErrorMessage = string.Empty;
		try
		{
			var updated = await _apiClient.UpdateNoteAsync(SelectedNote.Id, TitleInput, ContentInput);
			var index = Notes.IndexOf(SelectedNote);
			if (index >= 0)
			{
				Notes[index] = updated;

				// Notes[index]の置き換えでListBoxのSelectedItemバインディングが
				// 古い参照を見失い選択解除してしまうため、明示的に選択を維持する。
				SelectedNote = updated;
			}
		}
		catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException or InvalidOperationException)
		{
			ErrorMessage = "メモの更新に失敗しました。サーバーが起動しているか確認してください。";
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task DeleteAsync()
	{
		if (SelectedNote is null)
		{
			return;
		}

		IsBusy = true;
		ErrorMessage = string.Empty;
		try
		{
			await _apiClient.DeleteNoteAsync(SelectedNote.Id);
			Notes.Remove(SelectedNote);
			SelectedNote = null;
		}
		catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
		{
			ErrorMessage = "メモの削除に失敗しました。サーバーが起動しているか確認してください。";
		}
		finally
		{
			IsBusy = false;
		}
	}
}
