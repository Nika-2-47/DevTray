using System;
using System.Threading;

namespace ConsoleEffects
{
    public class SearchlightEffect : IConsoleEffect
    {
        public string Name => "Searchlight";
        public string Description => "暗闇を照らすサーチライトエフェクト";

        public void Run()
        {
            Console.CursorVisible = false;
            Console.Clear();

            int width = Console.WindowWidth;
            int height = Console.WindowHeight;
            
            // 背景用の文字を生成
            char[,] background = new char[width, height];
            Random random = new Random();
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789#@$%&*";
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    background[x, y] = chars[random.Next(chars.Length)];
                }
            }

            double t = 0;
            
            while (!Console.KeyAvailable)
            {
                // サーチライトの中心座標を計算（リサージュ図形的な動き）
                int centerX = (int)(width / 2 + (width / 2 - 5) * Math.Sin(t * 0.5));
                int centerY = (int)(height / 2 + (height / 2 - 3) * Math.Cos(t * 0.3));
                
                // 半径（横長補正）
                int radiusY = 6;
                int radiusX = radiusY * 2;

                // 描画（全画面書き換えは重いので、変化がある部分だけ...と言いたいが、
                // サーチライトが動くので全画面再描画に近い処理が必要。
                // チラつき防止のため、バッファリングしたいが、System.Consoleでは難しい。
                // ここでは簡易的に、カーソル移動を最小限にするアプローチをとるか、
                // あるいはStringBuilderで一括書き出しを行う。
                
                // 今回はStringBuilderで一括書き出しを行う方式でチラつきを抑える
                System.Text.StringBuilder buffer = new System.Text.StringBuilder();
                
                // カーソルを左上に戻す
                Console.SetCursorPosition(0, 0);

                for (int y = 0; y < height - 1; y++) // 最後の行は書き込むとスクロールするので避ける
                {
                    for (int x = 0; x < width; x++)
                    {
                        // 中心からの距離判定
                        double dx = (double)(x - centerX) / radiusX;
                        double dy = (double)(y - centerY) / radiusY;
                        double distSq = dx * dx + dy * dy;

                        if (distSq <= 1.0)
                        {
                            // ライトの中
                            // エスケープシーケンスを使って色を変える手もあるが、
                            // Windowsの標準コンソールだと文字化けする可能性があるので
                            // ここでは一括書き出しは諦めて、逐次描画にするか...
                            // いや、逐次描画は遅い。
                            // 妥協案：ライトの中だけ描画し、外は描画しない（黒背景の場合）
                            // しかし今回は「暗闇にある文字を照らす」なので、
                            // 外側はDarkGray、内側はWhiteにしたい。
                        }
                    }
                }
                
                // 逐次描画方式（重いが確実）
                for (int y = 0; y < height - 1; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        double dx = (double)(x - centerX) / radiusX;
                        double dy = (double)(y - centerY) / radiusY;
                        double distSq = dx * dx + dy * dy;

                        Console.SetCursorPosition(x, y);
                        
                        if (distSq <= 1.0)
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write(background[x, y]);
                        }
                        else if (distSq <= 1.5) // ぼんやりした周辺
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.Write(background[x, y]);
                        }
                        else
                        {
                            // 完全な暗闇にするならスペース
                            // Console.Write(" ");
                            
                            // うっすら見えるならDarkBlueとか
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.Write(" ");
                        }
                    }
                }

                t += 0.2;
                Thread.Sleep(50);
            }

            Console.ResetColor();
            Console.Clear();
            Console.CursorVisible = true;
            if (Console.KeyAvailable) Console.ReadKey(true);
        }
    }
}
