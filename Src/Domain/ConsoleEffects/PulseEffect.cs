using System;
using System.Threading;

namespace ConsoleEffects;

/// <summary>
/// パルスエフェクトを提供するクラス
/// </summary>
public class PulseEffect
{
    private readonly int _delay;
    private readonly ConsoleColor _color;

    public PulseEffect(int delay = 100, ConsoleColor color = ConsoleColor.Cyan)
    {
        _delay = delay;
        _color = color;
    }

    /// <summary>
    /// エフェクトを実行します。
    /// </summary>
    public void Run()
    {
        Console.Clear();
        Console.CursorVisible = false;

        try
        {
            int width = Console.WindowWidth;
            int height = Console.WindowHeight;
            int centerX = width / 2;
            int centerY = height / 2;

            int radius = 1;
            bool expanding = true;

            while (!Console.KeyAvailable)
            {
                Console.Clear();

                for (int y = -radius; y <= radius; y++)
                {
                    for (int x = -radius; x <= radius; x++)
                    {
                        if (x * x + y * y <= radius * radius)
                        {
                            int drawX = centerX + x;
                            int drawY = centerY + y;

                            if (drawX >= 0 && drawX < width && drawY >= 0 && drawY < height)
                            {
                                Console.SetCursorPosition(drawX, drawY);
                                Console.ForegroundColor = _color;
                                Console.Write("*");
                            }
                        }
                    }
                }

                Thread.Sleep(_delay);

                if (expanding)
                {
                    radius++;
                    if (radius > Math.Min(width, height) / 4)
                    {
                        expanding = false;
                    }
                }
                else
                {
                    radius--;
                    if (radius <= 1)
                    {
                        expanding = true;
                    }
                }
            }
        }
        finally
        {
            Console.ResetColor();
            Console.CursorVisible = true;
        }
    }
}