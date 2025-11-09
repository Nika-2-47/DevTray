using System;
using System.Collections.Generic;
using System.Threading;

namespace ConsoleEffects;

/// <summary>
/// カラフルな紙吹雪（Confetti）を降らせるシンプルなコンソールエフェクト
/// 任意のキー入力で停止できます。RunForDurationで指定時間だけ実行可能。
/// </summary>
public class ConfettiEffect
{
    private readonly int _delay;
    private readonly ConsoleColor[] _colors;
    private readonly char[] _chars;
    private readonly Random _rand;
    private readonly int _spawnRate; // フレームあたり生成する個数
    private readonly int _maxPieces;

    private class Piece
    {
        public int X;
        public int Y;
        public int PrevX;
        public int PrevY;
        public ConsoleColor Color;
        public char Char;
        public int Speed; // 1なら毎フレーム移動、2なら2フレームに1回 など
        public int Tick; // 内部カウンタ
    }

    public ConfettiEffect(
        int delay = 50,
        int spawnRate = 3,
        int maxPieces = 200,
        ConsoleColor[]? colors = null,
        char[]? chars = null)
    {
        _delay = Math.Max(10, delay);
        _spawnRate = Math.Max(1, spawnRate);
        _maxPieces = Math.Max(10, maxPieces);
        _colors = colors ?? new[] { ConsoleColor.Red, ConsoleColor.Yellow, ConsoleColor.Green, ConsoleColor.Cyan, ConsoleColor.Magenta, ConsoleColor.White };
        _chars = chars ?? new[] { '*', 'o', '•', '·', '+', 'x', '■' };
        _rand = new Random();
    }

    /// <summary>
    /// エフェクトをキー入力まで実行します（Ctrl+Cでも停止）
    /// </summary>
    public void Run()
    {
        RunInternal(null);
    }

    /// <summary>
    /// 指定されたミリ秒だけエフェクトを実行します
    /// </summary>
    /// <param name="duration">実行時間（ミリ秒）</param>
    public void RunForDuration(int duration)
    {
        RunInternal(duration);
    }

    private void RunInternal(int? durationMs)
    {
        Console.CursorVisible = false;
        try
        {
            int width = Math.Max(2, Console.WindowWidth);
            int height = Math.Max(2, Console.WindowHeight);

            var pieces = new List<Piece>();
            var start = DateTime.Now;

            // メインループ
            while (true)
            {
                // 終了時間チェック
                if (durationMs.HasValue && (DateTime.Now - start).TotalMilliseconds >= durationMs.Value)
                    break;

                if (Console.KeyAvailable)
                {
                    // キー消費して終了
                    Console.ReadKey(true);
                    break;
                }

                // 新規生成
                for (int i = 0; i < _spawnRate && pieces.Count < _maxPieces; i++)
                {
                    var p = new Piece
                    {
                        X = _rand.Next(0, width),
                        Y = 0,
                        PrevX = -1,
                        PrevY = -1,
                        Color = _colors[_rand.Next(_colors.Length)],
                        Char = _chars[_rand.Next(_chars.Length)],
                        Speed = _rand.Next(1, 4),
                        Tick = 0
                    };
                    pieces.Add(p);
                }

                // 更新と描画
                foreach (var piece in pieces)
                {
                    // 消す（前フレームの位置）
                    if (piece.PrevX >= 0 && piece.PrevY >= 0 && piece.PrevY < height && piece.PrevX < width && piece.PrevX >= 0)
                    {
                        TryWriteAt(piece.PrevX, piece.PrevY, ' ', Console.BackgroundColor);
                    }

                    piece.Tick++;
                    if (piece.Tick >= piece.Speed)
                    {
                        // 少し横揺れを加える
                        int dx = _rand.Next(-1, 2);
                        piece.X = Math.Clamp(piece.X + dx, 0, width - 1);
                        piece.Y += 1;
                        piece.Tick = 0;
                    }

                    // 画面内のみ描画
                    if (piece.Y >= 0 && piece.Y < height && piece.X >= 0 && piece.X < width)
                    {
                        TryWriteAt(piece.X, piece.Y, piece.Char, piece.Color);
                    }

                    piece.PrevX = piece.X;
                    piece.PrevY = piece.Y;
                }

                // 画面外の破棄
                pieces.RemoveAll(p => p.Y >= height - 0);

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
            // クリアして終了
            Console.Clear();
        }
    }

    private void TryWriteAt(int x, int y, char ch, ConsoleColor color)
    {
        try
        {
            // 範囲チェック
            if (x < 0 || y < 0) return;
            if (y >= Console.WindowHeight) return;
            if (x >= Console.WindowWidth) return;

            Console.SetCursorPosition(x, y);
            Console.ForegroundColor = color;
            Console.Write(ch);
        }
        catch
        {
            // 例外は無視（ウィンドウサイズが変化した等）
        }
    }
}
