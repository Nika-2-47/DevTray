using System;
using System.Threading;

namespace ConsoleEffects;

/// <summary>
/// オーロラ（極光）を模したコンソールエフェクト
/// カラーバンドがゆっくり横に移動してグラデーションを作ります。
/// </summary>
public class AuroraEffect
{
    private readonly int _width;
    private readonly int _height;
    private readonly int _delay;
    private readonly ConsoleColor _backgroundColor;
    private readonly ConsoleColor[] _palette;
    private readonly Random _random;

    public AuroraEffect(int? width = null, int? height = null, int delay = 120, ConsoleColor backgroundColor = ConsoleColor.Black)
    {
        _width = width ?? Console.WindowWidth;
        _height = height ?? Console.WindowHeight;
        _delay = delay;
        _backgroundColor = backgroundColor;
        _random = new Random();

        // オーロラっぽい色パレット
        _palette = new[]
        {
            ConsoleColor.DarkBlue,
            ConsoleColor.Blue,
            ConsoleColor.Cyan,
            ConsoleColor.Green,
            ConsoleColor.DarkGreen,
            ConsoleColor.Magenta,
            ConsoleColor.DarkMagenta
        };
    }

    /// <summary>
    /// エフェクトをキー入力まで実行
    /// </summary>
    public void Run()
    {
        Console.CursorVisible = false;
        Console.Clear();
        var phase = _random.NextDouble() * Math.PI * 2;

        try
        {
            while (!Console.KeyAvailable)
            {
                DrawFrame(phase);
                phase += 0.06; // ゆっくり動かす
                Thread.Sleep(_delay);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"エフェクト実行中にエラーが発生しました: {ex.Message}");
        }
        finally
        {
            if (Console.KeyAvailable) Console.ReadKey(true);
            Console.ResetColor();
            Console.CursorVisible = true;
            Console.Clear();
        }
    }

    /// <summary>
    /// 指定時間だけ実行
    /// </summary>
    public void RunForDuration(int durationMs)
    {
        Console.CursorVisible = false;
        Console.Clear();
        var start = DateTime.Now;
        var phase = _random.NextDouble() * Math.PI * 2;

        try
        {
            while ((DateTime.Now - start).TotalMilliseconds < durationMs)
            {
                DrawFrame(phase);
                phase += 0.06;
                Thread.Sleep(_delay);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"エフェクト実行中にエラーが発生しました: {ex.Message}");
        }
        finally
        {
            Console.ResetColor();
            Console.CursorVisible = true;
            Console.Clear();
        }
    }

    private void DrawFrame(double phase)
    {
        // 背景色設定
        Console.BackgroundColor = _backgroundColor;
        Console.Clear();

        // 各行ごとに横方向のグラデーションを描画
        for (int y = 0; y < _height; y++)
        {
            // 行ごとに位相をわずかに変えると、より自然に見える
            double rowPhase = phase + (y - _height / 2.0) * 0.02;

            for (int x = 0; x < _width; x++)
            {
                // 波形と位相でパレットインデックスを決定
                double t = Math.Sin((x / (double)_width) * Math.PI * 2.0 + rowPhase);
                // -1..1 を 0..1 に変換
                double u = (t + 1.0) / 2.0;
                // パレットの中で色を選ぶ
                int index = (int)Math.Floor(u * (_palette.Length - 1));
                index = Math.Clamp(index, 0, _palette.Length - 1);

                Console.ForegroundColor = _palette[index];
                // 背景色を用いた塗りつぶしよりも、前景文字で表現した方がコンソールの互換性が高い
                // 濃淡っぽく見せるために空白かドットを選ぶ
                char ch = ChooseCharForIntensity(u, x + y);

                // カーソル移動と描画
                try
                {
                    Console.SetCursorPosition(x, y);
                    Console.Write(ch);
                }
                catch
                {
                    // ウィンドウサイズが変わったタイミングで例外が出る場合があるので無視
                }
            }
        }
    }

    private char ChooseCharForIntensity(double u, int seed)
    {
        // u が大きいほど明るく（より濃い表示）する
        if (u > 0.85) return '@';
        if (u > 0.7) return 'O';
        if (u > 0.5) return 'o';
        if (u > 0.3) return '.';
        // まれにひとつだけドットを出すことでテクスチャ感を出す
        var rnd = (_random.Next() ^ seed) & 31;
        if (rnd == 0) return '.';
        return ' ';
    }
}
