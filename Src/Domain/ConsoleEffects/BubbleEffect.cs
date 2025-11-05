using System;
using System.Collections.Generic;
using System.Threading;

namespace ConsoleEffects;

/// <summary>
/// 泡が上昇するアニメーションを表示するコンソールエフェクトを提供するクラス
/// </summary>
public class BubbleEffect
{
    private readonly int _width;
    private readonly int _height;
    private readonly int _delay;
    private readonly Random _random;
    private readonly char[] _bubbleChars;
    private readonly ConsoleColor _bubbleColor;
    private readonly ConsoleColor _backgroundColor;

    public BubbleEffect(
        int? width = null,
        int? height = null,
        int delay = 100,
        char[]? bubbleChars = null,
        ConsoleColor bubbleColor = ConsoleColor.Cyan,
        ConsoleColor backgroundColor = ConsoleColor.Black)
    {
        _width = width ?? Console.WindowWidth;
        _height = height ?? Console.WindowHeight;
        _delay = delay;
        _random = new Random();
        _bubbleChars = bubbleChars ?? new[] { 'o', 'O', '@', '*' };
        _bubbleColor = bubbleColor;
        _backgroundColor = backgroundColor;
    }

    /// <summary>
    /// 泡エフェクトを実行します
    /// </summary>
    public void Run()
    {
        Console.CursorVisible = false;
        Console.Clear();

        var bubbles = new List<Bubble>();

        try
        {
            while (!Console.KeyAvailable)
            {
                // 新しい泡を生成
                if (_random.NextDouble() < 0.3)
                {
                    bubbles.Add(new Bubble(_random.Next(_width), _height, _bubbleChars[_random.Next(_bubbleChars.Length)]));
                }

                // 泡を更新
                for (int i = bubbles.Count - 1; i >= 0; i--)
                {
                    var bubble = bubbles[i];
                    bubble.Update();

                    if (!bubble.IsActive)
                    {
                        bubbles.RemoveAt(i);
                    }
                }

                // 画面を描画
                Console.BackgroundColor = _backgroundColor;
                Console.Clear();
                Console.ForegroundColor = _bubbleColor;

                foreach (var bubble in bubbles)
                {
                    if (bubble.Y >= 0 && bubble.Y < _height)
                    {
                        Console.SetCursorPosition(bubble.X, bubble.Y);
                        Console.Write(bubble.Character);
                    }
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

    private class Bubble
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public char Character { get; private set; }
        public bool IsActive { get; private set; }

        public Bubble(int x, int y, char character)
        {
            X = x;
            Y = y;
            Character = character;
            IsActive = true;
        }

        public void Update()
        {
            Y--;
            if (Y < 0)
            {
                IsActive = false;
            }
        }
    }
}