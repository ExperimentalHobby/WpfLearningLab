using System.Windows;
using System.Windows.Controls;

namespace ToDoList;

/// <summary>
/// ToDoリストのメインウィンドウ。
/// タスクの追加・編集・削除・完了切り替えを <see cref="ToDoListEngine"/> に委譲するだけの薄いコードビハインド。
/// </summary>
public partial class MainWindow : Window
{
	private readonly ToDoListEngine _engine = new();

	/// <summary>
	/// ウィンドウを初期化する。
	/// </summary>
	public MainWindow()
	{
		InitializeComponent();
		RefreshTaskList();
	}

	/// <summary>
	/// 「追加」ボタン押下時の処理。入力欄のタイトルでタスクを追加する。
	/// </summary>
	private void AddButton_Click(object sender, RoutedEventArgs e)
	{
		_engine.AddTask(NewTaskTextBox.Text);
		NewTaskTextBox.Clear();
		RefreshTaskList();
	}

	/// <summary>
	/// 「削除」ボタン押下時の処理。選択中のタスクを削除する。未選択の場合は何もしない。
	/// </summary>
	private void DeleteButton_Click(object sender, RoutedEventArgs e)
	{
		if (TaskListBox.SelectedItem is not ToDoTask task)
		{
			return;
		}

		_engine.RemoveTask(task.Id);
		EditTaskTextBox.Clear();
		RefreshTaskList();
	}

	/// <summary>
	/// 「更新」ボタン押下時の処理。選択中のタスクのタイトルを編集欄の内容で更新する。未選択の場合は何もしない。
	/// </summary>
	private void UpdateButton_Click(object sender, RoutedEventArgs e)
	{
		if (TaskListBox.SelectedItem is not ToDoTask task)
		{
			return;
		}

		_engine.EditTitle(task.Id, EditTaskTextBox.Text);
		RefreshTaskList();
	}

	/// <summary>
	/// 一覧内の完了チェックボックスがクリックされたときの処理。該当タスクの完了状態を切り替える。
	/// </summary>
	private void DoneCheckBox_Click(object sender, RoutedEventArgs e)
	{
		if (sender is CheckBox { Tag: int id })
		{
			_engine.ToggleDone(id);
			RefreshTaskList();
		}
	}

	/// <summary>
	/// 一覧の選択タスクが変わったときの処理。編集欄に選択タスクのタイトルを反映する。
	/// </summary>
	private void TaskListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		EditTaskTextBox.Text = TaskListBox.SelectedItem is ToDoTask task ? task.Title : string.Empty;
	}

	/// <summary>
	/// 一覧の表示を <see cref="ToDoListEngine.Tasks"/> の現在の内容に合わせて更新する。
	/// 更新前に選択されていたタスクがまだ存在する場合は選択状態を維持する。
	/// </summary>
	private void RefreshTaskList()
	{
		var selectedId = (TaskListBox.SelectedItem as ToDoTask)?.Id;
		TaskListBox.ItemsSource = null;
		TaskListBox.ItemsSource = _engine.Tasks;
		if (selectedId is int id)
		{
			TaskListBox.SelectedItem = _engine.Tasks.FirstOrDefault(t => t.Id == id);
		}
	}
}
