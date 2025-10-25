
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
// dotnet add package Microsoft.Extensions.Hosting





using System;
using System.Threading;

class Program
{
	static async Task<int> Main(string[] args)
	{
		using var loggerFactory = LoggerFactory.Create(builder =>
		{
			builder
				.AddConsole()
				.AddDebug();
		});

		ILogger logger = loggerFactory.CreateLogger<Program>();
		logger.LogInformation("DevTray 起動");
        logger.LogInformation("一度だけログを出力します");
        logger.LogWarning("一度だけログを出力します");
        logger.LogError("一度だけログを出力します");

		Console.WriteLine("=== DevTray ツールコレクション ===");
		Console.WriteLine("使用コマンド: nightrider | httpserver [port] [root] | exit | help");

		while (true)
		{
			Console.Write("> ");
			var line = Console.ReadLine() ?? string.Empty;
			var input = line.Trim();
			if (string.IsNullOrEmpty(input)) continue;

			var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
			var cmd = parts[0].ToLowerInvariant();

			if (cmd == "exit")
			{
				Console.WriteLine("終了します。");
				break;
			}
			else if (cmd == "help")
			{
				Console.WriteLine("httpserver [port] [root] - 簡易HTTPサーバーを起動します（既定: 8080, カレントディレクトリ）");
				Console.WriteLine("nightrider - コンソールのエフェクトを表示します（Ctrl+Cで停止）");
				Console.WriteLine("exit - アプリ終了");
			}
			else if (cmd == "nightrider")
			{
				RunNightRider();
			}
			else if (cmd == "httpserver")
			{
				int port = 8080;
				string root = Directory.GetCurrentDirectory();
				if (parts.Length >= 2 && int.TryParse(parts[1], out var p)) port = p;
				if (parts.Length >= 3) root = parts[2];

				logger.LogInformation("HTTP サーバーを起動します - ポート:{port}, ルート:{root}", port, root);
				await RunSimpleHttpServer(port, root);
			}
			else
			{
				Console.WriteLine("不明なコマンドです。help を参照してください。");
			}
		}

		logger.LogInformation("DevTray 終了");
		return 0;
	}

	static void RunNightRider()
	{
		int width = 30;        // 光が動く幅
		int trail = 5;         // 残像の長さ
		int delay = 50;        // フレーム間隔(ms)
		char lightChar = '●';  // 光の文字
		ConsoleColor baseColor = ConsoleColor.DarkRed;
		ConsoleColor mainColor = ConsoleColor.Red;

		int pos = 0;
		int dir = 1;

		Console.CursorVisible = false;
		Console.WriteLine("ナイトライダー起動中... Ctrl+Cで停止");

		try
		{
			while (!Console.KeyAvailable)
			{
				for (int i = 0; i < width; i++)
				{
					int d = Math.Abs(i - pos);

					if (d == 0)
					{
						Console.ForegroundColor = mainColor;
						Console.Write(lightChar);
					}
					else if (d <= trail)
					{
						Console.ForegroundColor = baseColor;
						Console.Write(lightChar);
					}
					else
					{
						Console.Write(' ');
					}
				}

				Console.SetCursorPosition(0, Console.CursorTop);
				pos += dir;
				if (pos >= width - 1 || pos <= 0)
					dir *= -1;

				Thread.Sleep(delay);
			}
		}
		catch (ThreadAbortException)
		{
			// 無視
		}
		finally
		{
			Console.ResetColor();
			Console.CursorVisible = true;
			Console.WriteLine("\nナイトライダー停止");
			while (Console.KeyAvailable) Console.ReadKey(true);
		}
	}

	static async Task RunSimpleHttpServer(int port, string root)
	{
		if (!Directory.Exists(root))
		{
			Console.WriteLine("指定されたルートディレクトリが存在しません: " + root);
			return;
		}

		using var listener = new HttpListener();
		var prefix = $"http://localhost:{port}/";
		listener.Prefixes.Add(prefix);
		try
		{
			listener.Start();
		}
		catch (HttpListenerException hlex)
		{
			Console.WriteLine("HttpListener の開始に失敗しました: " + hlex.Message);
			Console.WriteLine("管理者権限または別のアプリがポートを使用している可能性があります。");
			return;
		}

		Console.WriteLine($"Listening on {prefix} — ブラウザでアクセスできます。停止するには Enter を押してください。");

		var cts = new CancellationTokenSource();

		var serveTask = Task.Run(async () =>
		{
			while (listener.IsListening && !cts.Token.IsCancellationRequested)
			{
				try
				{
					var ctx = await listener.GetContextAsync().ConfigureAwait(false);
					_ = Task.Run(() => HandleRequest(ctx, root));
				}
				catch (HttpListenerException) { break; }
				catch (ObjectDisposedException) { break; }
			}
		}, cts.Token);

		// ユーザが Enter を押すと停止
		Console.ReadLine();
		cts.Cancel();
		listener.Stop();
		try { await serveTask.ConfigureAwait(false); } catch { }
		Console.WriteLine("HTTP サーバーを停止しました。");
	}

	static void HandleRequest(HttpListenerContext ctx, string root)
	{
		try
		{
			var req = ctx.Request;
			var resp = ctx.Response;

			string urlPath = Uri.UnescapeDataString(req.Url?.AbsolutePath.TrimStart('/') ?? string.Empty);
			if (string.IsNullOrEmpty(urlPath)) urlPath = "index.html";

			var filePath = Path.Combine(root, urlPath.Replace('/', Path.DirectorySeparatorChar));

			if (Directory.Exists(filePath))
			{
				filePath = Path.Combine(filePath, "index.html");
			}

			if (!File.Exists(filePath))
			{
				resp.StatusCode = 404;
				using var sw = new StreamWriter(resp.OutputStream, Encoding.UTF8);
				sw.WriteLine("404 Not Found");
				resp.Close();
				return;
			}

			var bytes = File.ReadAllBytes(filePath);
			resp.ContentLength64 = bytes.Length;
			resp.ContentType = GetContentTypeByExtension(Path.GetExtension(filePath));
			resp.OutputStream.Write(bytes, 0, bytes.Length);
			resp.OutputStream.Close();
		}
		catch
		{
			try { ctx.Response.StatusCode = 500; ctx.Response.Close(); } catch { }
		}
	}

	static string GetContentTypeByExtension(string ext)
	{
		return ext.ToLowerInvariant() switch
		{
			".html" => "text/html; charset=utf-8",
			".htm" => "text/html; charset=utf-8",
			".css" => "text/css",
			".js" => "application/javascript",
			".json" => "application/json",
			".png" => "image/png",
			".jpg" or ".jpeg" => "image/jpeg",
			".gif" => "image/gif",
			".svg" => "image/svg+xml",
			_ => "application/octet-stream",
		};
	}
}
