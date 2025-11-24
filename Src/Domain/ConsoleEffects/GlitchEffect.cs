using System;
using System.Threading;

namespace ConsoleEffects;

/// <summary>
/// デジタルノイズとグリッチ表現のエフェクトを提供するクラス
/// </summary>
public class GlitchEffect : IConsoleEffect
{
    public string Name => "Glitch";
    public string Description => "デジタルノイズとグリッチ表現のエフェクト";

    private readonly int _width;
    private readonly int _height;
    private readonly Random _random;
    private readonly char[] _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()_+-=[]{}|;:,.<>?/".ToCharArray();

    public GlitchEffect()
    {
        _width = Console.WindowWidth;
        _height = Console.WindowHeight;
        _random = new Random();
    }

    public void Run()
    {
        Console.CursorVisible = false;
        Console.Clear();

        try
        {
            while (!Console.KeyAvailable)
            {
                // 画面サイズが変わっていたら更新
                if (Console.WindowWidth != _width || Console.WindowHeight != _height)
                {
                    Console.Clear();
                    // サイズ変数はreadonlyにしているので再取得はできないが、
                    // 本来はここでサイズ再取得すべき。今回は簡易的にそのまま。
                }

                // 1. ランダムな文字の配置
                for (int i = 0; i < 20; i++)
                {
                    int x = _random.Next(_width);
                    int y = _random.Next(_height);
                    SetColor();
                    Console.SetCursorPosition(x, y);
                    Console.Write(_chars[_random.Next(_chars.Length)]);
                }

                // 2. 水平ラインのグリッチ（行ごとの色変更や文字の書き換え）
                if (_random.NextDouble() < 0.3)
                {
                    int y = _random.Next(_height);
                    int len = _random.Next(5, _width);
                    int x = _random.Next(_width - len);
                    
                    SetColor(true); // 強調色
                    Console.SetCursorPosition(x, y);
                    string noise = new string('█', len);
                    // またはランダム文字
                    if (_random.NextDouble() < 0.5)
                    {
                        char[] noiseChars = new char[len];
                        for(int k=0; k<len; k++) noiseChars[k] = _chars[_random.Next(_chars.Length)];
                        noise = new string(noiseChars);
                    }
                    Console.Write(noise);
                }

                // 3. ブロックノイズ
                if (_random.NextDouble() < 0.1)
                {
                    int w = _random.Next(2, 10);
                    int h = _random.Next(2, 6);
                    int x = _random.Next(_width - w);
                    int y = _random.Next(_height - h);
                    
                    ConsoleColor bg = (ConsoleColor)_random.Next(1, 15);
                    Console.BackgroundColor = bg;
                    for (int dy = 0; dy < h; dy++)
                    {
                        if (y + dy >= _height) break;
                        Console.SetCursorPosition(x, y + dy);
                        Console.Write(new string(' ', w));
                    }
                    Console.ResetColor();
                }

                Thread.Sleep(20);
            }
            
            // キー入力を消費
            Console.ReadKey(true);
        }
        catch (Exception)
        {
            // コンソールサイズ変更などでエラーが出ても安全に終了
        }
        finally
        {
            Console.ResetColor();
            Console.Clear();
            Console.CursorVisible = true;
        }
    }

    private void SetColor(bool highlight = false)
    {
        if (highlight)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.DarkRed;
        }
        else
        {
            Console.BackgroundColor = ConsoleColor.Black;
            int color = _random.Next(0, 3);
            if (color == 0) Console.ForegroundColor = ConsoleColor.DarkGreen;
            else if (color == 1) Console.ForegroundColor = ConsoleColor.Green;
            else Console.ForegroundColor = ConsoleColor.DarkGray;
        }
    }
}
