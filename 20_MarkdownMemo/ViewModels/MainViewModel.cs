using System.Collections.ObjectModel;
using MarkdownMemo.Data;
using MarkdownMemo.Models;
using MarkdownMemo.Services;

namespace MarkdownMemo.ViewModels;

/// <summary>
/// Markdownメモアプリのメイン画面のViewModel。メモの保存・一覧・読込・削除とプレビュー生成を担う。
/// </summary>
public class MainViewModel : ObservableObject
{
	private readonly IMemoRepository _repository;
	private readonly IMarkdownToHtmlConverter _converter;

	private string _inputTitle = string.Empty;
	private string _markdownContent = string.Empty;
	private string _previewHtml = string.Empty;
	private MemoSummary? _selectedMemo;

	/// <summary>
	/// ViewModelを初期化し、リポジトリからメモ一覧を読み込む。
	/// </summary>
	/// <param name="repository">メモの永続化を担うリポジトリ。</param>
	/// <param name="converter">Markdown→HTML変換を担うコンバーター。</param>
	public MainViewModel(IMemoRepository repository, IMarkdownToHtmlConverter converter)
	{
		_repository = repository;
		_converter = converter;

		SaveCommand = new RelayCommand(Save, CanSave);
		NewCommand = new RelayCommand(New);
		DeleteCommand = new RelayCommand(Delete, CanDelete);

		LoadMemoList();
		UpdatePreview();
	}

	/// <summary>保存済みメモの一覧(最終更新日時の降順)。</summary>
	public ObservableCollection<MemoSummary> Memos { get; } = [];

	/// <summary>編集中メモのタイトル(識別子)。</summary>
	public string InputTitle
	{
		get => _inputTitle;
		set
		{
			if (SetProperty(ref _inputTitle, value))
			{
				SaveCommand.RaiseCanExecuteChanged();
			}
		}
	}

	/// <summary>編集中のMarkdown本文。</summary>
	public string MarkdownContent
	{
		get => _markdownContent;
		set
		{
			if (SetProperty(ref _markdownContent, value))
			{
				UpdatePreview();
			}
		}
	}

	/// <summary><see cref="MarkdownContent"/>をHTMLに変換したプレビュー内容。</summary>
	public string PreviewHtml
	{
		get => _previewHtml;
		private set => SetProperty(ref _previewHtml, value);
	}

	/// <summary>一覧で選択中のメモ。選択すると編集欄に内容が読み込まれる。</summary>
	public MemoSummary? SelectedMemo
	{
		get => _selectedMemo;
		set
		{
			if (SetProperty(ref _selectedMemo, value))
			{
				if (value is not null)
				{
					InputTitle = value.Title;
					MarkdownContent = _repository.Load(value.Title);
				}

				DeleteCommand.RaiseCanExecuteChanged();
			}
		}
	}

	/// <summary>編集中の内容を保存するコマンド。</summary>
	public RelayCommand SaveCommand { get; }

	/// <summary>編集欄・選択状態をクリアし、新規メモの作成を開始するコマンド。</summary>
	public RelayCommand NewCommand { get; }

	/// <summary>選択中のメモを削除するコマンド。</summary>
	public RelayCommand DeleteCommand { get; }

	private bool CanSave() => !string.IsNullOrWhiteSpace(InputTitle);

	private void Save()
	{
		_repository.Save(InputTitle, MarkdownContent);
		LoadMemoList();
		SelectedMemo = Memos.FirstOrDefault(memo => memo.Title == InputTitle);
	}

	private void New()
	{
		SelectedMemo = null;
		InputTitle = string.Empty;
		MarkdownContent = string.Empty;
	}

	private bool CanDelete() => SelectedMemo is not null;

	private void Delete()
	{
		if (SelectedMemo is null)
		{
			return;
		}

		_repository.Delete(SelectedMemo.Title);
		LoadMemoList();
		New();
	}

	private void LoadMemoList()
	{
		Memos.Clear();
		foreach (var memo in _repository.GetAll())
		{
			Memos.Add(memo);
		}
	}

	private void UpdatePreview() => PreviewHtml = _converter.Convert(MarkdownContent);
}
