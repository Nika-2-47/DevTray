using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
// dotnet add package Microsoft.Extensions.Hosting


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



