using ParallelImageProcessor.Services;

namespace ParallelImageProcessor.Tests.Fakes;

/// <summary>
/// 実際のダイアログを開かず、あらかじめ設定したパスを返す<see cref="IFolderPicker"/>のフェイク。
/// </summary>
internal class FakeFolderPicker : IFolderPicker
{
	public string? ResultPath { get; set; }

	public string? PickFolder() => ResultPath;
}
