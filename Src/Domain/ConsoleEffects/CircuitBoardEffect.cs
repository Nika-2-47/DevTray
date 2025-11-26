using System;
using System.Collections.Generic;
using System.Threading;

namespace ConsoleEffects
{
    /// <summary>
    /// 回路基板のようなパターンを描画するエフェクト
    /// </summary>
    public class CircuitBoardEffect : IConsoleEffect
    {
        public string Name => "Circuit Board";
        public string Description => "回路基板のようなパターンを描画します";

        private readonly int _width;
        private readonly int _height;
        private readonly Random _random;
        private readonly List<CircuitPath> _paths;
        private readonly bool[,] _grid;
        private const int MaxPaths = 20;

        public CircuitBoardEffect()
        {
            _width = Console.WindowWidth;
            _height = Console.WindowHeight;
            _random = new Random();
            _paths = new List<CircuitPath>();
            _grid = new bool[_width, _height];
        }

        public void Run()
        {
            Console.Clear();
            Console.CursorVisible = false;
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Circuit Board Effect - Press any key to exit");

            // 初期パスの生成
            for (int i = 0; i < 5; i++)
            {
                SpawnPath();
            }

            try
            {
                while (!Console.KeyAvailable)
                {
                    // 新しいパスをランダムに追加
                    if (_paths.Count < MaxPaths && _random.Next(10) < 2)
                    {
                        SpawnPath();
                    }

                    // パスの更新と描画
                    for (int i = _paths.Count - 1; i >= 0; i--)
                    {
                        var path = _paths[i];
                        if (!path.IsFinished)
                        {
                            UpdatePath(path);
                        }
                        else
                        {
                            // 終了したパスは一定確率でリストから削除（画面には残る）
                            // または、画面がいっぱいになったらクリアするロジックが必要だが
                            // 今回はシンプルにリストから外すだけにする（描画は残る）
                            _paths.RemoveAt(i);
                        }
                    }

                    Thread.Sleep(50);
                }
                
                // キー入力を消費
                Console.ReadKey(true);
            }
            finally
            {
                Console.ResetColor();
                Console.CursorVisible = true;
                Console.Clear();
            }
        }

        private void SpawnPath()
        {
            int x = _random.Next(_width);
            int y = _random.Next(1, _height); // 1行目はメッセージ用
            
            // 既に描画されている場所には生成しない
            if (_grid[x, y]) return;

            var path = new CircuitPath
            {
                X = x,
                Y = y,
                Direction = (Direction)_random.Next(4),
                Length = 0,
                MaxLength = _random.Next(10, 50),
                Color = ConsoleColor.DarkGreen
            };
            
            _paths.Add(path);
            DrawPoint(x, y, 'o', ConsoleColor.Green); // 開始点（ビア）
            _grid[x, y] = true;
        }

        private void UpdatePath(CircuitPath path)
        {
            // 次の座標を計算
            int nextX = path.X;
            int nextY = path.Y;

            switch (path.Direction)
            {
                case Direction.Up: nextY--; break;
                case Direction.Down: nextY++; break;
                case Direction.Left: nextX--; break;
                case Direction.Right: nextX++; break;
            }

            // 境界チェック & 衝突チェック
            if (nextX < 0 || nextX >= _width || nextY < 1 || nextY >= _height || _grid[nextX, nextY])
            {
                path.IsFinished = true;
                DrawPoint(path.X, path.Y, '●', ConsoleColor.Green); // 終端（パッド）
                return;
            }

            // 描画
            char symbol = GetPathSymbol(path.Direction);
            DrawPoint(nextX, nextY, symbol, path.Color);
            _grid[nextX, nextY] = true;

            // 状態更新
            path.X = nextX;
            path.Y = nextY;
            path.Length++;

            // 方向転換
            if (_random.Next(100) < 10) // 10%の確率で曲がる
            {
                ChangeDirection(path);
                // 曲がり角の描画修正は簡易化のため省略（直角記号を使うとより良い）
                DrawPoint(path.X, path.Y, '+', ConsoleColor.DarkGreen);
            }

            // 寿命チェック
            if (path.Length >= path.MaxLength)
            {
                path.IsFinished = true;
                DrawPoint(path.X, path.Y, '●', ConsoleColor.Green);
            }
        }

        private void ChangeDirection(CircuitPath path)
        {
            // 現在の方向に対して直角に曲がる
            if (path.Direction == Direction.Up || path.Direction == Direction.Down)
            {
                path.Direction = _random.Next(2) == 0 ? Direction.Left : Direction.Right;
            }
            else
            {
                path.Direction = _random.Next(2) == 0 ? Direction.Up : Direction.Down;
            }
        }

        private char GetPathSymbol(Direction dir)
        {
            return (dir == Direction.Up || dir == Direction.Down) ? '│' : '─';
        }

        private void DrawPoint(int x, int y, char c, ConsoleColor color)
        {
            if (x >= 0 && x < _width && y >= 0 && y < _height)
            {
                Console.SetCursorPosition(x, y);
                Console.ForegroundColor = color;
                Console.Write(c);
            }
        }

        private class CircuitPath
        {
            public int X { get; set; }
            public int Y { get; set; }
            public Direction Direction { get; set; }
            public int Length { get; set; }
            public int MaxLength { get; set; }
            public bool IsFinished { get; set; }
            public ConsoleColor Color { get; set; }
        }

        private enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }
    }
}
