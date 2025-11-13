using System;
using System.Collections.Generic;
using System.Threading;

namespace ConsoleEffects;

/// <summary>
/// 蛇のような動きをするエフェクトを提供するクラス
/// </summary>
public class SnakeEffect
{
    private readonly int _delay;
    private readonly ConsoleColor _color;

    public SnakeEffect(int delay = 100, ConsoleColor color = ConsoleColor.Green)
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
            var snake = new Queue<(int x, int y)>();
            int snakeLength = 10;
            int x = width / 2, y = height / 2;
            int dx = 1, dy = 0;

            while (!Console.KeyAvailable)
            {
                // 蛇の頭を移動
                x += dx;
                y += dy;

                // 画面端で折り返し
                if (x < 0) x = width - 1;
                if (x >= width) x = 0;
                if (y < 0) y = height - 1;
                if (y >= height) y = 0;

                // 蛇の体を更新
                snake.Enqueue((x, y));
                if (snake.Count > snakeLength)
                {
                    var tail = snake.Dequeue();
                    Console.SetCursorPosition(tail.x, tail.y);
                    Console.Write(' ');
                }

                // 蛇を描画
                Console.SetCursorPosition(x, y);
                Console.ForegroundColor = _color;
                Console.Write('O');

                Thread.Sleep(_delay);

                // ランダムに方向を変更
                if (new Random().Next(10) < 2)
                {
                    (dx, dy) = (dy, -dx); // 90度回転
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