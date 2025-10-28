using System;
using System.Threading;

namespace ConsoleEffects;

/// <summary>
/// 回転するローディングスピナーエフェクトを提供するクラス
/// </summary>
public class SpinnerEffect
{
    private readonly int _delay;
    private readonly char[] _spinnerChars;
    private readonly ConsoleColor _spinnerColor;
    private readonly ConsoleColor _backgroundColor;
    private readonly string _message;
    private readonly bool _showMessage;

    // 異なるスピナーパターン
    private static readonly char[][] SpinnerPatterns = 
    {
        // 標準スピナー
        new char[] { '|', '/', '-', '\\' },
        
        // ドット回転
        new char[] { '⣾', '⣽', '⣻', '⢿', '⡿', '⣟', '⣯', '⣷' },
        
        // 円形
        new char[] { '◐', '◓', '◑', '◒' },
        
        // 矢印
        new char[] { '←', '↖', '↑', '↗', '→', '↘', '↓', '↙' },
        
        // バー
        new char[] { '▁', '▃', '▄', '▅', '▆', '▇', '█', '▇', '▆', '▅', '▄', '▃' }
    };

    /// <summary>
    /// SpinnerEffectのインスタンスを初期化します
    /// </summary>
    /// <param name="delay">フレーム間隔（ミリ秒、既定: 100）</param>
    /// <param name="spinnerPattern">スピナーパターン（既定: 0 = 標準）</param>
    /// <param name="spinnerColor">スピナーの色（既定: Yellow）</param>
    /// <param name="backgroundColor">背景色（既定: Black）</param>
    /// <param name="message">表示メッセージ（既定: "Loading..."）</param>
    /// <param name="showMessage">メッセージを表示するか（既定: true）</param>
    public SpinnerEffect(
        int delay = 100,
        int spinnerPattern = 0,
        ConsoleColor spinnerColor = ConsoleColor.Yellow,
        ConsoleColor backgroundColor = ConsoleColor.Black,
        string message = "Loading...",
        bool showMessage = true)
    {
        _delay = delay;
        _spinnerChars = SpinnerPatterns[Math.Max(0, Math.Min(spinnerPattern, SpinnerPatterns.Length - 1))];
        _spinnerColor = spinnerColor;
        _backgroundColor = backgroundColor;
        _message = message;
        _showMessage = showMessage;
    }

    /// <summary>
    /// スピナーエフェクトを実行します
    /// キー入力があるまで継続し、Ctrl+Cで停止できます
    /// </summary>
    public void Run()
    {
        Console.CursorVisible = false;
        Console.BackgroundColor = _backgroundColor;
        Console.Clear();

        int spinnerIndex = 0;
        int centerX = Console.WindowWidth / 2;
        int centerY = Console.WindowHeight / 2;

        try
        {
            while (!Console.KeyAvailable)
            {
                // スピナーを描画
                Console.SetCursorPosition(centerX, centerY);
                Console.ForegroundColor = _spinnerColor;
                Console.Write(_spinnerChars[spinnerIndex]);

                // メッセージを表示
                if (_showMessage && !string.IsNullOrEmpty(_message))
                {
                    int messageX = Math.Max(0, centerX - _message.Length / 2);
                    Console.SetCursorPosition(messageX, centerY + 2);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(_message);
                }

                spinnerIndex = (spinnerIndex + 1) % _spinnerChars.Length;
                Thread.Sleep(_delay);

                // 前のフレームをクリア
                Console.SetCursorPosition(centerX, centerY);
                Console.Write(' ');
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"エフェクト実行中にエラーが発生しました: {ex.Message}");
        }
        finally
        {
            // キー入力を消費
            if (Console.KeyAvailable)
            {
                Console.ReadKey(true);
            }
            
            // コンソールを元の状態に戻す
            Console.ResetColor();
            Console.CursorVisible = true;
            Console.Clear();
        }
    }

    /// <summary>
    /// 指定された時間だけスピナーエフェクトを実行します
    /// </summary>
    /// <param name="duration">実行時間（ミリ秒）</param>
    public void RunForDuration(int duration)
    {
        Console.CursorVisible = false;
        Console.BackgroundColor = _backgroundColor;
        Console.Clear();

        int spinnerIndex = 0;
        int centerX = Console.WindowWidth / 2;
        int centerY = Console.WindowHeight / 2;
        var startTime = DateTime.Now;

        try
        {
            while ((DateTime.Now - startTime).TotalMilliseconds < duration)
            {
                // スピナーを描画
                Console.SetCursorPosition(centerX, centerY);
                Console.ForegroundColor = _spinnerColor;
                Console.Write(_spinnerChars[spinnerIndex]);

                // メッセージを表示
                if (_showMessage && !string.IsNullOrEmpty(_message))
                {
                    int messageX = Math.Max(0, centerX - _message.Length / 2);
                    Console.SetCursorPosition(messageX, centerY + 2);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(_message);
                }

                spinnerIndex = (spinnerIndex + 1) % _spinnerChars.Length;
                Thread.Sleep(_delay);

                // 前のフレームをクリア
                Console.SetCursorPosition(centerX, centerY);
                Console.Write(' ');
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"エフェクト実行中にエラーが発生しました: {ex.Message}");
        }
        finally
        {
            // コンソールを元の状態に戻す
            Console.ResetColor();
            Console.CursorVisible = true;
            Console.Clear();
        }
    }

    /// <summary>
    /// 複数のスピナーを同時に表示する
    /// </summary>
    /// <param name="spinnerCount">スピナーの数（既定: 4）</param>
    public void RunMultiSpinner(int spinnerCount = 4)
    {
        Console.CursorVisible = false;
        Console.BackgroundColor = _backgroundColor;
        Console.Clear();

        var spinners = new int[spinnerCount];
        var colors = new ConsoleColor[] 
        { 
            ConsoleColor.Yellow, ConsoleColor.Cyan, ConsoleColor.Magenta, 
            ConsoleColor.Green, ConsoleColor.Red, ConsoleColor.Blue 
        };

        int centerX = Console.WindowWidth / 2;
        int centerY = Console.WindowHeight / 2;
        int radius = Math.Min(centerX, centerY) / 3;

        try
        {
            while (!Console.KeyAvailable)
            {
                for (int i = 0; i < spinnerCount; i++)
                {
                    // 円形に配置
                    double angle = (2 * Math.PI * i / spinnerCount);
                    int x = centerX + (int)(Math.Cos(angle) * radius);
                    int y = centerY + (int)(Math.Sin(angle) * radius / 2); // Y軸は縦横比を調整

                    // 前のフレームをクリア
                    Console.SetCursorPosition(x, y);
                    Console.Write(' ');

                    // 新しいスピナーを描画
                    Console.SetCursorPosition(x, y);
                    Console.ForegroundColor = colors[i % colors.Length];
                    Console.Write(_spinnerChars[spinners[i]]);

                    spinners[i] = (spinners[i] + 1) % _spinnerChars.Length;
                }

                // 中央にメッセージを表示
                if (_showMessage && !string.IsNullOrEmpty(_message))
                {
                    int messageX = Math.Max(0, centerX - _message.Length / 2);
                    Console.SetCursorPosition(messageX, centerY);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(_message);
                }

                Thread.Sleep(_delay);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"エフェクト実行中にエラーが発生しました: {ex.Message}");
        }
        finally
        {
            // キー入力を消費
            if (Console.KeyAvailable)
            {
                Console.ReadKey(true);
            }
            
            // コンソールを元の状態に戻す
            Console.ResetColor();
            Console.CursorVisible = true;
            Console.Clear();
        }
    }

    /// <summary>
    /// 利用可能なスピナーパターンをデモ表示
    /// </summary>
    public static void ShowPatterns()
    {
        Console.CursorVisible = false;
        Console.Clear();

        Console.WriteLine("=== スピナーパターンデモ ===");
        Console.WriteLine("各パターンを3秒間表示します...\n");

        for (int patternIndex = 0; patternIndex < SpinnerPatterns.Length; patternIndex++)
        {
            var pattern = SpinnerPatterns[patternIndex];
            Console.WriteLine($"パターン {patternIndex}: ");
            
            var spinner = new SpinnerEffect(
                delay: 100, 
                spinnerPattern: patternIndex, 
                message: $"Pattern {patternIndex}",
                showMessage: true
            );
            
            spinner.RunForDuration(3000);
            
            Console.WriteLine($"パターン {patternIndex} 完了\n");
            Thread.Sleep(500);
        }

        Console.WriteLine("全パターンデモ完了！任意のキーを押してください...");
        Console.ReadKey();
        Console.CursorVisible = true;
        Console.Clear();
    }
}