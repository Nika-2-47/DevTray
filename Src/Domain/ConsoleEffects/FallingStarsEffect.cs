using System;
using System.Threading;

namespace ConsoleEffects;

/// <summary>
/// 星が降るエフェクトを提供するクラス
/// </summary>
public class FallingStarsEffect
{
    private readonly int _delay;

    public FallingStarsEffect(int delay = 100)
    {
        _delay = delay;
    }

    /// <summary>
    /// エフェクトを実行します。
    /// </summary>
    public void Run()
    {
        Console.Clear();
        Console.CursorVisible = false;

        int width = Console.WindowWidth;
        int height = Console.WindowHeight;
        Random random = new Random();

        char[] stars = { '*', '+', '.', 'o', 'x' };
        int[] starPositions = new int[width];

        for (int i = 0; i < width; i++)
        {
            starPositions[i] = random.Next(height);
        }

        try
        {
            while (!Console.KeyAvailable)
            {
                for (int x = 0; x < width; x++)
                {
                    Console.SetCursorPosition(x, starPositions[x]);
                    Console.Write(" ");

                    starPositions[x]++;

                    if (starPositions[x] >= height)
                    {
                        starPositions[x] = 0;
                    }

                    Console.SetCursorPosition(x, starPositions[x]);
                    Console.Write(stars[random.Next(stars.Length)]);
                }

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