using System;
using System.Threading;

namespace ConsoleEffects;

/// <summary>
/// ナイトライダー風のコンソールエフェクトを提供するクラス
/// </summary>
public class NightRiderEffect
{
    private readonly int _width;
    private readonly int _trail;
    private readonly int _delay;
    private readonly char _lightChar;
    private readonly ConsoleColor _baseColor;
    private readonly ConsoleColor _mainColor;

    /// <summary>
    /// NightRiderEffectのインスタンスを初期化します
    /// </summary>
    /// <param name="width">光が動く幅（既定: 30）</param>
    /// <param name="trail">残像の長さ（既定: 5）</param>
    /// <param name="delay">フレーム間隔（ミリ秒、既定: 50）</param>
    /// <param name="lightChar">光の文字（既定: ●）</param>
    /// <param name="baseColor">残像の色（既定: DarkRed）</param>
    /// <param name="mainColor">メインライトの色（既定: Red）</param>
    public NightRiderEffect(
        int width = 30,
        int trail = 5,
        int delay = 50,
        char lightChar = '●',
        ConsoleColor baseColor = ConsoleColor.DarkRed,
        ConsoleColor mainColor = ConsoleColor.Red)
    {
        _width = width;
        _trail = trail;
        _delay = delay;
        _lightChar = lightChar;
        _baseColor = baseColor;
        _mainColor = mainColor;
    }

    /// <summary>
    /// ナイトライダーエフェクトを実行します
    /// キー入力があるまで継続し、Ctrl+Cで停止できます
    /// </summary>
    public void Run()
    {
        int pos = 0;
        int dir = 1;

        Console.CursorVisible = false;
        Console.WriteLine("ナイトライダー起動中... Ctrl+Cで停止");

        try
        {
            while (!Console.KeyAvailable) // キー入力があるまでループ
            {
                for (int i = 0; i < _width; i++)
                {
                    int d = Math.Abs(i - pos);

                    if (d == 0)
                    {
                        Console.ForegroundColor = _mainColor;
                        Console.Write(_lightChar);
                    }
                    else if (d <= _trail)
                    {
                        Console.ForegroundColor = _baseColor;
                        Console.Write(_lightChar);
                    }
                    else
                    {
                        Console.Write(' ');
                    }
                }

                Console.SetCursorPosition(0, Console.CursorTop);
                pos += dir;
                if (pos >= _width - 1 || pos <= 0)
                    dir *= -1;

                Thread.Sleep(_delay);
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

    /// <summary>
    /// カスタマイズされたナイトライダーエフェクトを静的メソッドで実行します
    /// </summary>
    public static void RunCustom(
        int width = 30,
        int trail = 5,
        int delay = 50,
        char lightChar = '●',
        ConsoleColor baseColor = ConsoleColor.DarkRed,
        ConsoleColor mainColor = ConsoleColor.Red)
    {
        var effect = new NightRiderEffect(width, trail, delay, lightChar, baseColor, mainColor);
        effect.Run();
    }
}