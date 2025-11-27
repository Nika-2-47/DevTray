using System;
using System.Collections.Generic;
using System.Threading;

namespace ConsoleEffects
{
    public class AquariumEffect : IConsoleEffect
    {
        public string Name => "Aquarium";
        public string Description => "熱帯魚が泳ぐ水槽のエフェクト";

        private readonly Random _random = new Random();
        private readonly List<Fish> _fishes = new List<Fish>();
        private readonly List<Bubble> _bubbles = new List<Bubble>();
        private int _width;
        private int _height;

        // 魚の定義
        private readonly string[] _fishTemplatesRight = {
            "><>",           // Small
            "<°)))><",       // Medium
            "><((((º>",      // Long
            "¸.·´¯`·.´¯`·.¸¸.·´¯`·.¸><(((º>" // Very Long
        };

        private readonly string[] _fishTemplatesLeft = {
            "<><",           // Small
            "><(((°>",       // Medium
            "<º))))><",      // Long
            "<º)))><¸.·´¯`·.´¯`·.¸¸.·´¯`·.¸" // Very Long
        };

        private readonly ConsoleColor[] _fishColors = {
            ConsoleColor.Cyan,
            ConsoleColor.Yellow,
            ConsoleColor.Magenta,
            ConsoleColor.Red,
            ConsoleColor.White
        };

        public void Run()
        {
            Console.Clear();
            Console.CursorVisible = false;
            _width = Console.WindowWidth;
            _height = Console.WindowHeight;

            // 初期化：魚を数匹生成
            for (int i = 0; i < 8; i++)
            {
                SpawnFish();
            }

            // 海底の描画用
            string sand = new string('~', _width);

            try
            {
                while (!Console.KeyAvailable)
                {
                    // 画面サイズが変わっていたら更新
                    if (Console.WindowWidth != _width || Console.WindowHeight != _height)
                    {
                        _width = Console.WindowWidth;
                        _height = Console.WindowHeight;
                        Console.Clear();
                    }

                    // 泡の生成（ランダム）
                    if (_random.Next(10) < 3)
                    {
                        SpawnBubble();
                    }

                    // 更新と描画
                    UpdateAndDraw();

                    // 海底を描画
                    Console.SetCursorPosition(0, _height - 1);
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write(sand.Substring(0, Math.Min(sand.Length, _width)));

                    Thread.Sleep(100);
                }
                Console.ReadKey(true);
            }
            finally
            {
                Console.ResetColor();
                Console.Clear();
                Console.CursorVisible = true;
            }
        }

        private void SpawnFish()
        {
            int type = _random.Next(_fishTemplatesRight.Length);
            bool movingRight = _random.Next(2) == 0;
            
            _fishes.Add(new Fish
            {
                X = _random.Next(5, _width - 20),
                Y = _random.Next(1, _height - 2), // 海底と水面を避ける
                Type = type,
                MovingRight = movingRight,
                Color = _fishColors[_random.Next(_fishColors.Length)],
                Speed = _random.Next(1, 3) // 速度バリエーション
            });
        }

        private void SpawnBubble()
        {
            _bubbles.Add(new Bubble
            {
                X = _random.Next(_width),
                Y = _height - 2,
                Symbol = 'o'
            });
        }

        private void UpdateAndDraw()
        {
            // 魚の処理
            for (int i = 0; i < _fishes.Count; i++)
            {
                var fish = _fishes[i];

                // 前回の位置を消去
                ClearFish(fish);

                // 移動
                if (fish.MovingRight)
                    fish.X += 1; // 横方向の速度は調整が必要かも
                else
                    fish.X -= 1;

                // 画面端での反転
                string shape = fish.MovingRight ? _fishTemplatesRight[fish.Type] : _fishTemplatesLeft[fish.Type];
                
                if (fish.MovingRight && fish.X + shape.Length >= _width)
                {
                    fish.MovingRight = false;
                    fish.X = _width - shape.Length - 1;
                }
                else if (!fish.MovingRight && fish.X <= 0)
                {
                    fish.MovingRight = true;
                    fish.X = 1;
                }

                // ランダムに上下移動
                if (_random.Next(100) < 5)
                {
                    ClearFish(fish); // 移動前に消す
                    if (_random.Next(2) == 0 && fish.Y > 1) fish.Y--;
                    else if (fish.Y < _height - 3) fish.Y++;
                }

                // 描画
                DrawFish(fish);
            }

            // 泡の処理
            for (int i = _bubbles.Count - 1; i >= 0; i--)
            {
                var bubble = _bubbles[i];
                
                // 消去
                DrawPoint(bubble.X, bubble.Y, ' ');

                // 移動
                bubble.Y--;

                // 揺らぎ
                if (_random.Next(2) == 0)
                {
                    bubble.X += _random.Next(-1, 2);
                }

                // 画面外に出たら削除
                if (bubble.Y < 0)
                {
                    _bubbles.RemoveAt(i);
                    continue;
                }

                // 描画
                DrawPoint(bubble.X, bubble.Y, bubble.Symbol, ConsoleColor.DarkCyan);
            }
        }

        private void DrawFish(Fish fish)
        {
            string shape = fish.MovingRight ? _fishTemplatesRight[fish.Type] : _fishTemplatesLeft[fish.Type];
            if (fish.X >= 0 && fish.X + shape.Length < _width && fish.Y >= 0 && fish.Y < _height)
            {
                Console.SetCursorPosition(fish.X, fish.Y);
                Console.ForegroundColor = fish.Color;
                Console.Write(shape);
            }
        }

        private void ClearFish(Fish fish)
        {
            string shape = fish.MovingRight ? _fishTemplatesRight[fish.Type] : _fishTemplatesLeft[fish.Type];
            if (fish.X >= 0 && fish.X + shape.Length < _width && fish.Y >= 0 && fish.Y < _height)
            {
                Console.SetCursorPosition(fish.X, fish.Y);
                Console.Write(new string(' ', shape.Length));
            }
        }

        private void DrawPoint(int x, int y, char c, ConsoleColor color = ConsoleColor.Gray)
        {
            if (x >= 0 && x < _width && y >= 0 && y < _height)
            {
                Console.SetCursorPosition(x, y);
                Console.ForegroundColor = color;
                Console.Write(c);
            }
        }

        private class Fish
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Type { get; set; }
            public bool MovingRight { get; set; }
            public ConsoleColor Color { get; set; }
            public int Speed { get; set; }
        }

        private class Bubble
        {
            public int X { get; set; }
            public int Y { get; set; }
            public char Symbol { get; set; }
        }
    }
}
