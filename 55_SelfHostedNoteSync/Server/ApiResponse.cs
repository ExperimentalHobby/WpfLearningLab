namespace SelfHostedNoteSync.Server;

/// <summary>
/// <see cref="NoteApiHandler"/> が返すHTTPレスポンスの内容。
/// </summary>
/// <param name="StatusCode">HTTPステータスコード。</param>
/// <param name="Body">レスポンスボディ(JSON文字列)。ボディがない場合は<see langword="null"/>。</param>
public sealed record ApiResponse(int StatusCode, string? Body);
