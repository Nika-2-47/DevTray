using System;
using System.Threading;

namespace ConsoleEffects;

/// <summary>
/// プラズマのような波打つエフェクトを提供するクラス
/// </summary>
public class PlasmaEffect
{
    /// <summary>
    /// PlasmaEffectを実行します
    /// </summary>
    public void Run()
    {
        Console.CursorVisible = false;
        Console.Clear();
        int width = Console.WindowWidth;
        int height = Console.WindowHeight;
        double t = 0;

        // 輝度を表す文字セット
        char[] density = { ' ', '.', ':', '-', '=', '+', '*', '#', '%', '@' };

        try
        {
            while (!Console.KeyAvailable)
            {
                // ウィンドウサイズが変わったらクリア
                if (Console.WindowWidth != width || Console.WindowHeight != height)
                {
                    width = Console.WindowWidth;
                    height = Console.WindowHeight;
                    Console.Clear();
                }

                Console.SetCursorPosition(0, 0);
                
                for (int y = 0; y < height - 1; y++) // 最後の行はスクロール防止のため避けるか注意が必要
                {
                    for (int x = 0; x < width; x++)
                    {
                        // プラズマ計算
                        double v1 = Math.Sin(x * 0.05 + t);
                        double v2 = Math.Sin(y * 0.05 + t);
                        double v3 = Math.Sin((x + y) * 0.05 + t);
                        double v4 = Math.Sin(Math.Sqrt(x * x + y * y) * 0.05 + t);
                        
                        double v = (v1 + v2 + v3 + v4) / 4.0; // -1.0 ～ 1.0
                        
                        // インデックスにマッピング
                        int index = (int)((v + 1.0) / 2.0 * (density.Length - 1));
                        index = Math.Max(0, Math.Min(index, density.Length - 1));
                        
                        // 色の決定
                        if (index < 2) Console.ForegroundColor = ConsoleColor.DarkBlue;
                        else if (index < 4) Console.ForegroundColor = ConsoleColor.Blue;
                        else if (index < 6) Console.ForegroundColor = ConsoleColor.Cyan;
                        else if (index < 8) Console.ForegroundColor = ConsoleColor.DarkCyan;
                        else Console.ForegroundColor = ConsoleColor.White;

                        Console.Write(density[index]);
                    }
                    // 行末での改行（最終行以外）
                    if (y < height - 2) Console.WriteLine();
                }
                
                t += 0.1;
                // 描画負荷が高い場合はSleepなしでも十分遅い可能性がある
                // Thread.Sleep(10); 
            }
        }
        finally
        {
            Console.ResetColor();
            Console.Clear();
            Console.CursorVisible = true;
            // キー入力を消費
            if (Console.KeyAvailable) Console.ReadKey(true);
        }
    }
}
