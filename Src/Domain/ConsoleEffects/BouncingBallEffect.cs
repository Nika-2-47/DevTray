using System;
using System.Threading;

namespace ConsoleEffects;

/// <summary>
/// ボールが反射するエフェクトを提供するクラス
/// </summary>
public class BouncingBallEffect
{
    private readonly int _delay;

    public BouncingBallEffect(int delay = 50)
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

        int ballX = width / 2;
        int ballY = height / 2;
        int velocityX = 1;
        int velocityY = 1;

        try
        {
            while (!Console.KeyAvailable)
            {
                Console.SetCursorPosition(ballX, ballY);
                Console.Write("O");

                Thread.Sleep(_delay);

                Console.SetCursorPosition(ballX, ballY);
                Console.Write(" ");

                ballX += velocityX;
                ballY += velocityY;

                if (ballX <= 0 || ballX >= width - 1)
                {
                    velocityX = -velocityX;
                }

                if (ballY <= 0 || ballY >= height - 1)
                {
                    velocityY = -velocityY;
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