using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text.Json;
using TaskApiWorkbench.Client.Models;
using TaskApiWorkbench.Client.Services;

namespace TaskApiWorkbench.Client.ViewModels;

/// <summary>
/// タスク管理クライアントのメイン画面のViewModel。タスクの一覧表示・追加・更新・完了切り替え・削除・エラー処理を担う。
/// </summary>
public class MainViewModel : ObservableObject
{
	private readonly ITaskApiClient _apiClient;

	private TaskItem? _selectedTask;
	private string _titleInput = string.Empty;
	private string _descriptionInput = string.Empty;
	private bool _isBusy;
	private string _errorMessage = string.Empty;

	/// <summary>
	/// ViewModelを初期化する。
	/// </summary>
	/// <param name="apiClient">タスクの取得・作成・更新・削除に使うクライアント。</param>
	public MainViewModel(ITaskApiClient apiClient)
	{
		_apiClient = apiClient;
		LoadCommand = new AsyncRelayCommand(LoadAsync, () => !IsBusy);
		AddCommand = new AsyncRelayCommand(AddAsync, CanAdd);
		UpdateCommand = new AsyncRelayCommand(UpdateAsync, CanUpdateOrDelete);
		ToggleCompleteCommand = new AsyncRelayCommand(ToggleCompleteAsync, CanUpdateOrDelete);
		DeleteCommand = new AsyncRelayCommand(DeleteAsync, CanUpdateOrDelete);
	}

	/// <summary>取得済みのタスク一覧。</summary>
	public ObservableCollection<TaskItem> Tasks { get; } = [];

	/// <summary>一覧で選択中のタスク。未選択の場合は<see langword="null"/>。</summary>
	public TaskItem? SelectedTask
	{
		get => _selectedTask;
		set
		{
			if (SetProperty(ref _selectedTask, value))
			{
				TitleInput = value?.Title ?? string.Empty;
				DescriptionInput = value?.Description ?? string.Empty;
				UpdateCommand.RaiseCanExecuteChanged();
				ToggleCompleteCommand.RaiseCanExecuteChanged();
				DeleteCommand.RaiseCanExecuteChanged();
			}
		}
	}

	/// <summary>タスク追加・更新フォームのタイトル入力欄。</summary>
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

	/// <summary>タスク追加・更新フォームの説明入力欄。</summary>
	public string DescriptionInput
	{
		get => _descriptionInput;
		set => SetProperty(ref _descriptionInput, value);
	}

	/// <summary>通信中かどうか。</summary>
	public bool IsBusy
	{
		get => _isBusy;
		private set
		{
			if (SetProperty(ref _isBusy, value))
			{
				// IsBusyは全コマンドのCanExecuteが参照する共有状態。ここで各コマンドに通知しないと、
				// 他コマンド実行中の変更通知に相乗りしてIsBusy=trueのタイミングで最後に
				// CanExecuteChangedが発火したコマンドは、falseに戻った後もWPFのButton.IsEnabledが
				// 古い判定のまま固まってしまう。
				LoadCommand.RaiseCanExecuteChanged();
				AddCommand.RaiseCanExecuteChanged();
				UpdateCommand.RaiseCanExecuteChanged();
				ToggleCompleteCommand.RaiseCanExecuteChanged();
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

	/// <summary>タスク一覧を再取得するコマンド。</summary>
	public AsyncRelayCommand LoadCommand { get; }

	/// <summary>入力欄の内容でタスクを新規作成するコマンド。</summary>
	public AsyncRelayCommand AddCommand { get; }

	/// <summary>選択中のタスクを入力欄の内容で更新するコマンド。</summary>
	public AsyncRelayCommand UpdateCommand { get; }

	/// <summary>選択中のタスクの完了状態を切り替えるコマンド。</summary>
	public AsyncRelayCommand ToggleCompleteCommand { get; }

	/// <summary>選択中のタスクを削除するコマンド。</summary>
	public AsyncRelayCommand DeleteCommand { get; }

	private bool CanAdd() => !IsBusy && !string.IsNullOrWhiteSpace(TitleInput);

	private bool CanUpdateOrDelete() => !IsBusy && SelectedTask is not null;

	private async Task LoadAsync()
	{
		IsBusy = true;
		ErrorMessage = string.Empty;
		try
		{
			var tasks = await _apiClient.GetTasksAsync();
			Tasks.Clear();
			foreach (var task in tasks)
			{
				Tasks.Add(task);
			}
		}
		catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
		{
			ErrorMessage = "タスク一覧の取得に失敗しました。サーバーが起動しているか確認してください。";
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
			var created = await _apiClient.CreateTaskAsync(TitleInput, DescriptionInput);
			Tasks.Add(created);
			TitleInput = string.Empty;
			DescriptionInput = string.Empty;
		}
		catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException or InvalidOperationException)
		{
			ErrorMessage = "タスクの作成に失敗しました。サーバーが起動しているか確認してください。";
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task UpdateAsync()
	{
		if (SelectedTask is null)
		{
			return;
		}

		IsBusy = true;
		ErrorMessage = string.Empty;
		try
		{
			var updated = await _apiClient.UpdateTaskAsync(SelectedTask.Id, TitleInput, DescriptionInput);
			ReplaceInList(updated);
		}
		catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException or InvalidOperationException)
		{
			ErrorMessage = "タスクの更新に失敗しました。サーバーが起動しているか確認してください。";
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task ToggleCompleteAsync()
	{
		if (SelectedTask is null)
		{
			return;
		}

		IsBusy = true;
		ErrorMessage = string.Empty;
		try
		{
			var toggled = await _apiClient.ToggleCompleteAsync(SelectedTask.Id);
			ReplaceInList(toggled);
		}
		catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException or InvalidOperationException)
		{
			ErrorMessage = "完了状態の切り替えに失敗しました。サーバーが起動しているか確認してください。";
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task DeleteAsync()
	{
		if (SelectedTask is null)
		{
			return;
		}

		IsBusy = true;
		ErrorMessage = string.Empty;
		try
		{
			await _apiClient.DeleteTaskAsync(SelectedTask.Id);
			Tasks.Remove(SelectedTask);
			SelectedTask = null;
		}
		catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
		{
			ErrorMessage = "タスクの削除に失敗しました。サーバーが起動しているか確認してください。";
		}
		finally
		{
			IsBusy = false;
		}
	}

	private void ReplaceInList(TaskItem updated)
	{
		var index = Tasks.IndexOf(SelectedTask!);
		if (index >= 0)
		{
			Tasks[index] = updated;

			// Tasks[index]の置き換えでListBoxのSelectedItemバインディングが
			// 古い参照を見失い選択解除してしまうため、明示的に選択を維持する。
			SelectedTask = updated;
		}
	}
}
