using System;
using System.Threading;

namespace ConsoleEffects
{
    public class SpectrumEffect : IConsoleEffect
    {
        public string Name => "Spectrum";
        public string Description => "オーディオスペクトラム風のエフェクト";

        public void Run()
        {
            Console.CursorVisible = false;
            Console.Clear();

            int width = Console.WindowWidth;
            int height = Console.WindowHeight;
            int barCount = width / 2; // 棒の数（幅2文字につき1本）
            int[] heights = new int[barCount];
            int[] peaks = new int[barCount]; // ピークホールド用
            Random random = new Random();

            // 初期化
            for (int i = 0; i < barCount; i++)
            {
                heights[i] = random.Next(1, height - 2);
                peaks[i] = heights[i];
            }

            while (!Console.KeyAvailable)
            {
                // 描画更新
                for (int i = 0; i < barCount; i++)
                {
                    // 新しい高さをランダムに変動させる（滑らかにするため、急激な変化を抑える）
                    int targetHeight = random.Next(1, height - 2);
                    
                    // 現在の高さからターゲットに向かって少し移動
                    if (heights[i] < targetHeight) heights[i] += random.Next(1, 3);
                    else if (heights[i] > targetHeight) heights[i] -= random.Next(1, 3);

                    // 範囲制限
                    if (heights[i] < 1) heights[i] = 1;
                    if (heights[i] > height - 2) heights[i] = height - 2;

                    // ピーク更新
                    if (heights[i] > peaks[i])
                    {
                        peaks[i] = heights[i];
                    }
                    else if (peaks[i] > 0)
                    {
                        // ピークはゆっくり落ちる
                        if (random.Next(10) > 5) peaks[i]--;
                    }

                    // バーの描画
                    int x = i * 2;
                    int h = heights[i];
                    int p = peaks[i];

                    for (int y = 0; y < height; y++)
                    {
                        // 画面下から描画するため、Y座標を変換
                        int screenY = height - 1 - y;
                        
                        if (screenY < 0 || screenY >= height) continue;

                        Console.SetCursorPosition(x, screenY);

                        if (y < h)
                        {
                            // バー本体
                            ConsoleColor color;
                            double ratio = (double)y / height;
                            
                            if (ratio > 0.8) color = ConsoleColor.Red;
                            else if (ratio > 0.5) color = ConsoleColor.Yellow;
                            else color = ConsoleColor.Green;

                            Console.ForegroundColor = color;
                            Console.Write("█ ");
                        }
                        else if (y == p)
                        {
                            // ピークインジケータ
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write("▬ ");
                        }
                        else
                        {
                            // 空白
                            Console.Write("  ");
                        }
                    }
                }

                Thread.Sleep(50);
            }

            Console.ResetColor();
            Console.Clear();
            Console.CursorVisible = true;
            if (Console.KeyAvailable) Console.ReadKey(true);
        }
    }
}
