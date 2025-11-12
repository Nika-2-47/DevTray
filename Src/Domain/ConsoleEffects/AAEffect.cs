using System;
using System.Threading;

namespace ConsoleEffects;

/// <summary>
/// ASCIIアートを表示するエフェクトクラス
/// </summary>
public class AAEffect
{
    private readonly int _delay;
    private readonly string[] _frames;

    public AAEffect(int delay = 500)
    {
        _delay = delay;
        _frames = new[]
        {
            @"  (\_/)",
            @"  (o.o)",
            @"  (> <)"
        };
    }

    /// <summary>
    /// ASCIIアートアニメーションを実行します。
    /// </summary>
    public void Run()
    {
        Console.Clear();
        Console.CursorVisible = false;

        try
        {
            int frameIndex = 0;
            while (!Console.KeyAvailable)
            {
                Console.SetCursorPosition(0, 0);
                Console.WriteLine(_frames[frameIndex]);
                frameIndex = (frameIndex + 1) % _frames.Length;
                Thread.Sleep(_delay);
            }
        }
        finally
        {
            Console.ResetColor();
            Console.CursorVisible = true;
        }
    }
}