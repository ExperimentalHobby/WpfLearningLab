using System.Text;

namespace SelfHostedNoteSync.Server;

internal static class Program
{
	private const string Prefix = "http://localhost:5055/";

	private static async Task Main(string[] args)
	{
		Console.OutputEncoding = Encoding.UTF8;

		using var server = new NoteHttpServer(new NoteApiHandler(new NoteStore()), Prefix);
		server.Start();

		Console.WriteLine($"55_SelfHostedNoteSync サーバーを起動しました: {Prefix}");
		Console.WriteLine("終了するには Ctrl+C を押してください。");

		await Task.Delay(Timeout.Infinite);
	}
}
