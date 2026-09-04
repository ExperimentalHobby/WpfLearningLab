using System.Windows.Input;
using GlobalHotkeyLauncher.Models;

namespace GlobalHotkeyLauncher.Tests;

/// <summary>
/// <see cref="HotKeyCombination"/> の単体テスト。
/// </summary>
public class HotKeyCombinationTests
{
	/// <summary>
	/// パス条件: 修飾キーとキーを指定すると"Ctrl+Alt+Shift+Win+L"の形式で表示文字列が得られること
	/// </summary>
	[Fact]
	public void ToDisplayString_全修飾キーを指定すると順序通りに整形される()
	{
		var combination = new HotKeyCombination(
			ModifierKeys.Control | ModifierKeys.Alt | ModifierKeys.Shift | ModifierKeys.Windows,
			Key.L);

		var result = combination.ToDisplayString();

		Assert.Equal("Ctrl+Alt+Shift+Win+L", result);
	}

	/// <summary>
	/// パス条件: 修飾キーが1つのみの場合もその修飾キー名+キーの形式で表示されること
	/// </summary>
	[Fact]
	public void ToDisplayString_修飾キーが1つの場合はその名前とキーのみ表示される()
	{
		var combination = new HotKeyCombination(ModifierKeys.Alt, Key.F5);

		var result = combination.ToDisplayString();

		Assert.Equal("Alt+F5", result);
	}

	/// <summary>
	/// パス条件: キーが未選択(Key.None)の場合Validateがエラーメッセージを返すこと
	/// </summary>
	[Fact]
	public void Validate_キー未選択の場合エラーになる()
	{
		var combination = new HotKeyCombination(ModifierKeys.Control, Key.None);

		var result = combination.Validate(out var errorMessage);

		Assert.False(result);
		Assert.False(string.IsNullOrEmpty(errorMessage));
	}

	/// <summary>
	/// パス条件: 修飾キーが1つも指定されていない場合Validateがエラーメッセージを返すこと
	/// </summary>
	[Fact]
	public void Validate_修飾キー無しの場合エラーになる()
	{
		var combination = new HotKeyCombination(ModifierKeys.None, Key.L);

		var result = combination.Validate(out var errorMessage);

		Assert.False(result);
		Assert.False(string.IsNullOrEmpty(errorMessage));
	}

	/// <summary>
	/// パス条件: 修飾キー・キーどちらも指定されている場合Validateが成功すること
	/// </summary>
	[Fact]
	public void Validate_修飾キーとキーが揃っていれば成功する()
	{
		var combination = new HotKeyCombination(ModifierKeys.Control, Key.L);

		var result = combination.Validate(out var errorMessage);

		Assert.True(result);
		Assert.Null(errorMessage);
	}

	/// <summary>
	/// パス条件: 同じ修飾キー・キーの組み合わせは値として等価であること(重複判定に利用)
	/// </summary>
	[Fact]
	public void Equals_同じ修飾キーとキーの組み合わせは等価()
	{
		var combination1 = new HotKeyCombination(ModifierKeys.Control | ModifierKeys.Alt, Key.L);
		var combination2 = new HotKeyCombination(ModifierKeys.Control | ModifierKeys.Alt, Key.L);

		Assert.Equal(combination1, combination2);
	}
}
