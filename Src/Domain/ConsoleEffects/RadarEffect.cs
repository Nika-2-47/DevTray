using System;
using System.Collections.Generic;
using System.Threading;

namespace ConsoleEffects
{
    public class RadarEffect : IConsoleEffect
    {
        public string Name => "Radar";
        public string Description => "レーダー画面のようなエフェクト";

        private struct Target
        {
            public int X;
            public int Y;
            public int Life; // 残り表示時間
        }

        public void Run()
        {
            Console.CursorVisible = false;
            Console.Clear();

            int width = Console.WindowWidth;
            int height = Console.WindowHeight;
            int centerX = width / 2;
            int centerY = height / 2;
            
            // コンソールの文字アスペクト比を考慮（横幅を広く使う）
            int radiusY = Math.Min(height, width / 2) / 2 - 2;
            int radiusX = radiusY * 2;

            double angle = 0;
            List<Target> targets = new List<Target>();
            Random random = new Random();

            // 前回の走査線を消去するためのキャッシュ
            List<(int x, int y)> previousLine = new List<(int x, int y)>();

            while (!Console.KeyAvailable)
            {
                // 1. 前回の走査線を消去（ターゲットがある場所は消さない）
                foreach (var point in previousLine)
                {
                    bool isTarget = false;
                    foreach (var t in targets)
                    {
                        if (t.X == point.x && t.Y == point.y)
                        {
                            isTarget = true;
                            break;
                        }
                    }
                    
                    if (!isTarget && point.x >= 0 && point.x < width && point.y >= 0 && point.y < height)
                    {
                        Console.SetCursorPosition(point.x, point.y);
                        Console.Write(" ");
                    }
                }
                previousLine.Clear();

                // 2. ターゲットの更新と描画
                for (int i = targets.Count - 1; i >= 0; i--)
                {
                    var t = targets[i];
                    t.Life--;
                    
                    if (t.Life <= 0)
                    {
                        Console.SetCursorPosition(t.X, t.Y);
                        Console.Write(" ");
                        targets.RemoveAt(i);
                    }
                    else
                    {
                        if (t.X >= 0 && t.X < width && t.Y >= 0 && t.Y < height)
                        {
                            Console.SetCursorPosition(t.X, t.Y);
                            Console.ForegroundColor = GetColorForLife(t.Life);
                            Console.Write(GetCharForLife(t.Life));
                            
                            // 構造体の値を更新してリストに戻す
                            targets[i] = t;
                        }
                    }
                }

                // 3. 新しい走査線の計算と描画
                angle += 0.1;
                if (angle >= Math.PI * 2) angle -= Math.PI * 2;

                double cos = Math.Cos(angle);
                double sin = Math.Sin(angle);

                // 中心から外側へ線を描画
                for (int r = 0; r < radiusY; r++) // Y方向の半径を基準にステップ
                {
                    // アスペクト比補正して座標計算
                    int lx = centerX + (int)(cos * r * 2); 
                    int ly = centerY + (int)(sin * r);

                    if (lx >= 0 && lx < width && ly >= 0 && ly < height)
                    {
                        // ターゲットがない場所なら走査線を描画
                        bool isTarget = false;
                        foreach (var t in targets)
                        {
                            if (t.X == lx && t.Y == ly)
                            {
                                isTarget = true;
                                break;
                            }
                        }

                        if (!isTarget)
                        {
                            Console.SetCursorPosition(lx, ly);
                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            Console.Write(".");
                            previousLine.Add((lx, ly));
                        }
                    }
                }

                // 4. 走査線上のランダムな位置にターゲット発生
                if (random.Next(100) < 15) // 出現確率
                {
                    double distRatio = 0.2 + (random.NextDouble() * 0.8); // 中心付近には出さない
                    int r = (int)(radiusY * distRatio);
                    
                    int tx = centerX + (int)(cos * r * 2);
                    int ty = centerY + (int)(sin * r);

                    // 既存のターゲットと被らないか簡易チェック
                    bool exists = false;
                    foreach(var t in targets) {
                        if (Math.Abs(t.X - tx) < 2 && Math.Abs(t.Y - ty) < 1) {
                            exists = true; break;
                        }
                    }

                    if (!exists && tx >= 0 && tx < width && ty >= 0 && ty < height)
                    {
                        targets.Add(new Target { X = tx, Y = ty, Life = 60 }); // Lifeサイクル
                    }
                }

                // 中心点
                Console.SetCursorPosition(centerX, centerY);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("+");

                // 枠の描画（たまに再描画して消えかけを防ぐ、または初回のみ描画でもいいが、走査線で消える可能性があるので）
                // 今回は枠描画は省略するか、簡易的に四隅だけ描くなど。
                // 負荷軽減のため省略。

                Thread.Sleep(30);
            }

            Console.ResetColor();
            Console.Clear();
            Console.CursorVisible = true;
            if (Console.KeyAvailable) Console.ReadKey(true);
        }

        private ConsoleColor GetColorForLife(int life)
        {
            if (life > 50) return ConsoleColor.White;
            if (life > 30) return ConsoleColor.Green;
            return ConsoleColor.DarkGreen;
        }

        private char GetCharForLife(int life)
        {
            if (life > 50) return '■';
            if (life > 30) return 'x';
            return '.';
        }
    }
}
