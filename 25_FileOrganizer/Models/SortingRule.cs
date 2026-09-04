namespace FileOrganizer.Models;

/// <summary>
/// 拡張子ごとの振り分けルール。
/// </summary>
/// <param name="Extension">対象拡張子(先頭に"."を含む。例: ".jpg")。</param>
/// <param name="DestinationFolderName">移動先フォルダ名(監視フォルダ配下の相対名)。</param>
public record SortingRule(string Extension, string DestinationFolderName);
