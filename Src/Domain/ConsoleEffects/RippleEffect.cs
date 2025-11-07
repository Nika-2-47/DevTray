using System;
using System.Threading;

namespace ConsoleEffects;

/// <summary>
/// 水面の波紋を模したコンソールエフェクトを表示するクラス。
/// </summary>
public class RippleEffect
{
    private readonly int _width;
    private readonly int _height;
    private readonly int _delay;
    private readonly ConsoleColor _rippleColor;
    private readonly ConsoleColor _backgroundColor;
    private readonly Random _random;

    public RippleEffect(int? width = null, int? height = null, int delay = 100, ConsoleColor rippleColor = ConsoleColor.Cyan, ConsoleColor backgroundColor = ConsoleColor.Black)
    {
        _width = width ?? Console.WindowWidth;
        _height = height ?? Console.WindowHeight;
        _delay = delay;
        _rippleColor = rippleColor;
        _backgroundColor = backgroundColor;
        _random = new Random();
    }

    /// <summary>
    /// 波紋エフェクトを実行します。
    /// </summary>
    public void Run()
    {
        Console.CursorVisible = false;
        Console.Clear();

        try
        {
            while (!Console.KeyAvailable)
            {
                DrawRipple();
                Thread.Sleep(_delay);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"エフェクト実行中にエラーが発生しました: {ex.Message}");
        }
        finally
        {
            if (Console.KeyAvailable) Console.ReadKey(true);
            Console.ResetColor();
            Console.CursorVisible = true;
            Console.Clear();
        }
    }

    private void DrawRipple()
    {
        Console.BackgroundColor = _backgroundColor;
        Console.Clear();

        int centerX = _random.Next(_width);
        int centerY = _random.Next(_height);

        for (int radius = 0; radius < Math.Max(_width, _height) / 2; radius++)
        {
            for (int angle = 0; angle < 360; angle += 10)
            {
                int x = centerX + (int)(radius * Math.Cos(angle * Math.PI / 180));
                int y = centerY + (int)(radius * Math.Sin(angle * Math.PI / 180));

                if (x >= 0 && x < _width && y >= 0 && y < _height)
                {
                    Console.SetCursorPosition(x, y);
                    Console.ForegroundColor = _rippleColor;
                    Console.Write('.');
                }
            }

            Thread.Sleep(50);
        }
    }
}