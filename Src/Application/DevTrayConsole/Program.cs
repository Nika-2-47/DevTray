﻿
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ConsoleEffects;
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
		Console.WriteLine("使用コマンド: nightrider | matrix [green|blue|red] | snow | wave [multi] | spinner [pattern|multi|demo] | fire [blue|green] | stars [warp|quiet] | httpserver [port] [root] | exit | help");

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
				Console.WriteLine("nightrider - ナイトライダー風エフェクト（任意のキーで停止）");
				Console.WriteLine("matrix [green|blue|red] - Matrix風レインエフェクト（任意のキーで停止）");
				Console.WriteLine("snow - 雪が降るエフェクト（任意のキーで停止）");
				Console.WriteLine("wave [multi] - 波のアニメーションエフェクト（multiで複数波表示、任意のキーで停止）");
				Console.WriteLine("spinner [0-4|multi|demo] - ローディングスピナーエフェクト（数字でパターン選択、任意のキーで停止）");
				Console.WriteLine("fire [blue|green] - 炎のエフェクト（blueで青い炎、greenで緑の炎、任意のキーで停止）");
				Console.WriteLine("stars [warp|quiet] - 星空エフェクト（warpでワープ速度、quietで静かな星空、任意のキーで停止）");
				Console.WriteLine("exit - アプリ終了");
			}
			else if (cmd == "nightrider")
			{
				var nightRider = new NightRiderEffect();
				nightRider.Run();
			}
			else if (cmd == "matrix")
			{
				string variant = parts.Length >= 2 ? parts[1].ToLower() : "green";
				
				logger.LogInformation("Matrix Rain エフェクトを起動します - バリエーション: {variant}", variant);
				
				switch (variant)
				{
					case "blue":
						MatrixRainEffect.RunBlueMatrix();
						break;
					case "red":
						MatrixRainEffect.RunRedMatrix();
						break;
					case "green":
					default:
						var matrix = new MatrixRainEffect();
						matrix.Run();
						break;
				}
			}
			else if (cmd == "snow")
			{
				logger.LogInformation("Snow エフェクトを起動します");
				
				var snow = new SnowEffect();
				snow.Run();
			}
			else if (cmd == "wave")
			{
				string variant = parts.Length >= 2 ? parts[1].ToLower() : "single";
				
				logger.LogInformation("Wave エフェクトを起動します - バリエーション: {variant}", variant);
				
				var wave = new WaveEffect(delay: 50, waveColor: ConsoleColor.Cyan, amplitude: 4.0);
				
				if (variant == "multi")
				{
					Console.WriteLine("複数波エフェクトを開始します。任意のキーで停止してください...");
					wave.RunMultiWave(3);
				}
				else
				{
					Console.WriteLine("波エフェクトを開始します。任意のキーで停止してください...");
					wave.Run();
				}
			}
			else if (cmd == "spinner")
			{
				string variant = parts.Length >= 2 ? parts[1].ToLower() : "0";
				
				logger.LogInformation("Spinner エフェクトを起動します - バリエーション: {variant}", variant);
				
				if (variant == "demo")
				{
					Console.WriteLine("全スピナーパターンをデモ表示します...");
					SpinnerEffect.ShowPatterns();
				}
				else if (variant == "multi")
				{
					var spinner = new SpinnerEffect(delay: 80, message: "Multi Spinner Demo");
					Console.WriteLine("複数スピナーエフェクトを開始します。任意のキーで停止してください...");
					spinner.RunMultiSpinner(6);
				}
				else if (int.TryParse(variant, out int pattern) && pattern >= 0 && pattern <= 4)
				{
					var spinner = new SpinnerEffect(
						delay: 100, 
						spinnerPattern: pattern, 
						message: $"Spinner Pattern {pattern}"
					);
					Console.WriteLine($"スピナーパターン {pattern} を開始します。任意のキーで停止してください...");
					spinner.Run();
				}
				else
				{
					var spinner = new SpinnerEffect(delay: 100, message: "Loading...");
					Console.WriteLine("標準スピナーエフェクトを開始します。任意のキーで停止してください...");
					spinner.Run();
				}
			}
			else if (cmd == "fire")
			{
				string variant = parts.Length >= 2 ? parts[1].ToLower() : "red";
				
				logger.LogInformation("Fire エフェクトを起動します - バリエーション: {variant}", variant);
				
				switch (variant)
				{
					case "blue":
						FireEffect.RunBlueFlame();
						break;
					case "green":
						FireEffect.RunGreenFlame();
						break;
					case "red":
					default:
						var fire = new FireEffect(delay: 50);
						Console.WriteLine("炎エフェクトを開始します。任意のキーで停止してください...");
						fire.Run();
						break;
				}
			}
			else if (cmd == "stars")
			{
				string variant = parts.Length >= 2 ? parts[1].ToLower() : "normal";
				
				logger.LogInformation("Stars エフェクトを起動します - バリエーション: {variant}", variant);
				
				switch (variant)
				{
					case "warp":
						StarfieldEffect.RunWarpSpeed();
						break;
					case "quiet":
						StarfieldEffect.RunQuietSpace();
						break;
					case "normal":
					default:
						var stars = new StarfieldEffect(delay: 60, speed: 1.0);
						Console.WriteLine("星空エフェクトを開始します。任意のキーで停止してください...");
						stars.Run();
						break;
				}
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
