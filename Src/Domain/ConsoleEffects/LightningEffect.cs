using System;
using System.Collections.Generic;
using System.Threading;

namespace ConsoleEffects;

/// <summary>
/// 雷の閃光をシミュレートするコンソールエフェクトを提供するクラス
/// </summary>
public class LightningEffect
{
    private readonly int _width;
    private readonly int _height;
    private readonly int _delay;
    private readonly Random _random;
    private readonly ConsoleColor _flashColor;
    private readonly ConsoleColor _backgroundColor;

    public LightningEffect(
        int? width = null,
        int? height = null,
        int delay = 500,
        ConsoleColor flashColor = ConsoleColor.White,
        ConsoleColor backgroundColor = ConsoleColor.Black)
    {
        _width = width ?? Console.WindowWidth;
        _height = height ?? Console.WindowHeight;
        _delay = delay;
        _random = new Random();
        _flashColor = flashColor;
        _backgroundColor = backgroundColor;
    }

    /// <summary>
    /// 雷エフェクトを実行します
    /// </summary>
    public void Run()
    {
        Console.CursorVisible = false;
        Console.Clear();

        try
        {
            while (!Console.KeyAvailable)
            {
                // 背景を一瞬フラッシュ
                Console.BackgroundColor = _flashColor;
                Console.Clear();
                Thread.Sleep(_random.Next(50, 200));

                // 背景を元に戻す
                Console.BackgroundColor = _backgroundColor;
                Console.Clear();
                Thread.Sleep(_delay);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"エフェクト実行中にエラーが発生しました: {ex.Message}");
        }
        finally
        {
            // コンソールを元の状態に戻す
            if (Console.KeyAvailable)
            {
                Console.ReadKey(true);
            }
            Console.ResetColor();
            Console.CursorVisible = true;
            Console.Clear();
        }
    }

    /// <summary>
    /// 雷エフェクトを一定時間実行します
    /// </summary>
    /// <param name="duration">実行時間（ミリ秒）</param>
    public void RunForDuration(int duration)
    {
        Console.CursorVisible = false;
        Console.Clear();

        var startTime = DateTime.Now;

        try
        {
            while ((DateTime.Now - startTime).TotalMilliseconds < duration)
            {
                // 背景を一瞬フラッシュ
                Console.BackgroundColor = _flashColor;
                Console.Clear();
                Thread.Sleep(_random.Next(50, 200));

                // 背景を元に戻す
                Console.BackgroundColor = _backgroundColor;
                Console.Clear();
                Thread.Sleep(_delay);
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
}