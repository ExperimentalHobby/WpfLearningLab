using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace ClaudeChatClient.Services;

/// <summary>
/// Claude Messages APIのSSE(Server-Sent Events)ストリームを解析し、
/// テキストの差分(<c>content_block_delta</c>のテキスト)を逐次取り出す純粋ロジック。
/// </summary>
public static class SseStreamParser
{
	/// <summary>
	/// SSEストリームを解析し、テキスト差分を順に列挙する。
	/// <c>message_stop</c>イベントを受信すると列挙を終了し、エラーイベントを受信すると
	/// <see cref="ClaudeApiException"/>をスローする。
	/// </summary>
	/// <param name="reader">SSEテキストを読み取る<see cref="TextReader"/>。</param>
	/// <param name="cancellationToken">キャンセル用トークン。</param>
	public static async IAsyncEnumerable<string> ParseTextDeltasAsync(
		TextReader reader,
		[EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var dataBuilder = new StringBuilder();

		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var line = await reader.ReadLineAsync(cancellationToken).ConfigureAwait(false);
			var isEndOfStream = line is null;

			// 空行(イベント区切り)、またはストリーム終端に達した場合、蓄積済みのデータを処理する。
			// ストリーム終端直前のイベントは、末尾に空行がないままEOFに達することがあるため、
			// isEndOfStreamの場合もここで拾う必要がある。
			if (isEndOfStream || line!.Length == 0)
			{
				if (dataBuilder.Length > 0)
				{
					var json = dataBuilder.ToString();
					dataBuilder.Clear();

					CheckForError(json);

					var text = ExtractTextDelta(json);
					if (text is not null)
					{
						yield return text;
					}

					if (IsMessageStop(json))
					{
						yield break;
					}
				}

				if (isEndOfStream)
				{
					yield break;
				}

				continue;
			}

			if (line.StartsWith("data:", StringComparison.Ordinal))
			{
				if (dataBuilder.Length > 0)
				{
					dataBuilder.Append('\n');
				}

				dataBuilder.Append(line["data:".Length..].TrimStart());
			}
			// "event:" 等、data以外の行はテキスト差分の抽出には不要なので読み飛ばす。
		}
	}

	private static string? ExtractTextDelta(string json)
	{
		using var doc = JsonDocument.Parse(json);
		var root = doc.RootElement;

		if (root.TryGetProperty("type", out var typeProp) && typeProp.GetString() == "content_block_delta"
			&& root.TryGetProperty("delta", out var delta)
			&& delta.TryGetProperty("type", out var deltaType) && deltaType.GetString() == "text_delta"
			&& delta.TryGetProperty("text", out var textProp))
		{
			return textProp.GetString();
		}

		return null;
	}

	private static bool IsMessageStop(string json)
	{
		using var doc = JsonDocument.Parse(json);
		return doc.RootElement.TryGetProperty("type", out var typeProp) && typeProp.GetString() == "message_stop";
	}

	private static void CheckForError(string json)
	{
		using var doc = JsonDocument.Parse(json);
		var root = doc.RootElement;

		if (root.TryGetProperty("type", out var typeProp) && typeProp.GetString() == "error"
			&& root.TryGetProperty("error", out var error))
		{
			var message = error.TryGetProperty("message", out var msgProp) ? msgProp.GetString() : null;
			throw new ClaudeApiException(message ?? "Claude APIでエラーが発生しました。");
		}
	}
}
