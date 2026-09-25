namespace SelfHostedNoteSync.Server;

/// <summary>
/// <see cref="NoteApiHandler"/> に渡すHTTPリクエストの内容。
/// <see cref="System.Net.HttpListener"/> 固有の型に依存せずルーティングをテストできるようにするための抽象化。
/// </summary>
/// <param name="Method">HTTPメソッド("GET"等)。</param>
/// <param name="Path">リクエストパス("/notes/1"等)。</param>
/// <param name="Body">リクエストボディ(JSON文字列)。ボディがない場合は<see langword="null"/>。</param>
public sealed record ApiRequest(string Method, string Path, string? Body);
