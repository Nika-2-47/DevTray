
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
    static void Main()
    {
        // ロガーファクトリを直接作る
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder
                .AddConsole()
                .AddDebug();
        });


        ILogger logger = loggerFactory.CreateLogger<Program>();

        logger.LogInformation("一度だけログを出力します");
        logger.LogWarning("一度だけログを出力します");
        logger.LogError("一度だけログを出力します");

        // ここでプログラム終了
        Console.WriteLine("処理終了");

        Console.WriteLine("コマンドを入力してください:");
        while (true)
        {
            Console.Write("> ");
            string input = Console.ReadLine()?.Trim().ToLower();

            if (input == "exit")
            {
                Console.WriteLine("終了します。");
                break;
            }
            else if (input == "nightrider")
            {
                RunNightRider();
            }
            else
            {
                Console.WriteLine("不明なコマンドです。'nightrider' または 'exit' を入力してください。");
            }
        }
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
            while (!Console.KeyAvailable) // キー入力があるまでループ
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
            // キー入力をクリア
            while (Console.KeyAvailable) Console.ReadKey(true);
        }
    }
}
