using System;
using System.Collections.Generic;
using System.Threading;

namespace ConsoleEffects;

/// <summary>
/// 星空や宇宙空間を模したエフェクトを提供するクラス
/// </summary>
public class StarfieldEffect
{
    private readonly int _width;
    private readonly int _height;
    private readonly int _delay;
    private readonly Random _random;
    private readonly char[] _starChars;
    private readonly ConsoleColor[] _starColors;
    private readonly ConsoleColor _backgroundColor;
    private readonly double _speed;
    private readonly int _maxStars;

    // 星の文字セット
    private static readonly char[] DefaultStarChars = 
    {
        '*', '·', '•', '∘', '○', '◦', '⋆', '✦', '✧', '★', '☆', '✨'
    };

    // 星の色セット
    private static readonly ConsoleColor[] DefaultStarColors = 
    {
        ConsoleColor.White,
        ConsoleColor.Yellow,
        ConsoleColor.Cyan,
        ConsoleColor.Blue,
        ConsoleColor.Magenta,
        ConsoleColor.Red
    };

    /// <summary>
    /// 星を表すクラス
    /// </summary>
    private class Star
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; } // 奥行き（距離）
        public double Speed { get; set; }
        public char Character { get; set; }
        public ConsoleColor Color { get; set; }
        public double Brightness { get; set; }
        public bool IsTwinkling { get; set; }
        public int TwinklePhase { get; set; }

        public Star(int maxWidth, int maxHeight, char[] starChars, ConsoleColor[] starColors, Random random)
        {
            Reset(maxWidth, maxHeight, starChars, starColors, random);
        }

        public void Reset(int maxWidth, int maxHeight, char[] starChars, ConsoleColor[] starColors, Random random)
        {
            X = random.NextDouble() * maxWidth;
            Y = random.NextDouble() * maxHeight;
            Z = random.NextDouble() * 100 + 1; // 1-101の距離
            Speed = 0.1 + random.NextDouble() * 0.5;
            Character = starChars[random.Next(starChars.Length)];
            Color = starColors[random.Next(starColors.Length)];
            Brightness = random.NextDouble();
            IsTwinkling = random.NextDouble() < 0.3; // 30%の星がきらめく
            TwinklePhase = random.Next(0, 20);
        }

        public void Update(double globalSpeed, int maxWidth, int maxHeight, char[] starChars, ConsoleColor[] starColors, Random random)
        {
            // Z軸方向に移動（手前に向かって）
            Z -= Speed * globalSpeed;
            
            // きらめき効果
            if (IsTwinkling)
            {
                TwinklePhase = (TwinklePhase + 1) % 20;
                Brightness = 0.3 + 0.7 * (Math.Sin(TwinklePhase * Math.PI / 10) + 1) / 2;
            }

            // 画面外に出たら再生成
            if (Z <= 0)
            {
                Reset(maxWidth, maxHeight, starChars, starColors, random);
                Z = 100;
            }
        }

        public (int screenX, int screenY, char displayChar) GetScreenPosition(int centerX, int centerY)
        {
            // 3D投影計算
            double perspective = 50.0 / Z;
            int screenX = centerX + (int)((X - centerX) * perspective);
            int screenY = centerY + (int)((Y - centerY) * perspective);
            
            // 距離による文字の変更
            char displayChar = Z > 80 ? '.' : Z > 50 ? '•' : Z > 20 ? '*' : Character;
            
            return (screenX, screenY, displayChar);
        }
    }

    /// <summary>
    /// StarfieldEffectのインスタンスを初期化します
    /// </summary>
    /// <param name="width">画面の幅（既定: コンソール幅）</param>
    /// <param name="height">画面の高さ（既定: コンソール高さ）</param>
    /// <param name="delay">フレーム間隔（ミリ秒、既定: 50）</param>
    /// <param name="starChars">星の文字セット（既定: DefaultStarChars）</param>
    /// <param name="starColors">星の色セット（既定: DefaultStarColors）</param>
    /// <param name="backgroundColor">背景色（既定: Black）</param>
    /// <param name="speed">移動速度（既定: 1.0）</param>
    /// <param name="maxStars">最大星数（既定: 100）</param>
    public StarfieldEffect(
        int? width = null,
        int? height = null,
        int delay = 50,
        char[]? starChars = null,
        ConsoleColor[]? starColors = null,
        ConsoleColor backgroundColor = ConsoleColor.Black,
        double speed = 1.0,
        int maxStars = 100)
    {
        _width = width ?? Console.WindowWidth;
        _height = height ?? Console.WindowHeight;
        _delay = delay;
        _random = new Random();
        _starChars = starChars ?? DefaultStarChars;
        _starColors = starColors ?? DefaultStarColors;
        _backgroundColor = backgroundColor;
        _speed = speed;
        _maxStars = maxStars;
    }

    /// <summary>
    /// 星空エフェクトを実行します
    /// キー入力があるまで継続し、Ctrl+Cで停止できます
    /// </summary>
    public void Run()
    {
        Console.CursorVisible = false;
        Console.BackgroundColor = _backgroundColor;
        Console.Clear();

        var stars = new Star[_maxStars];
        var screen = new char[_width, _height];
        var screenColors = new ConsoleColor[_width, _height];
        
        int centerX = _width / 2;
        int centerY = _height / 2;

        // 星を初期化
        for (int i = 0; i < _maxStars; i++)
        {
            stars[i] = new Star(_width, _height, _starChars, _starColors, _random);
        }

        try
        {
            while (!Console.KeyAvailable)
            {
                // 画面をクリア
                for (int x = 0; x < _width; x++)
                {
                    for (int y = 0; y < _height; y++)
                    {
                        screen[x, y] = ' ';
                        screenColors[x, y] = _backgroundColor;
                    }
                }

                // 星を更新して描画
                foreach (var star in stars)
                {
                    star.Update(_speed, _width, _height, _starChars, _starColors, _random);
                    
                    var (screenX, screenY, displayChar) = star.GetScreenPosition(centerX, centerY);
                    
                    if (screenX >= 0 && screenX < _width && screenY >= 0 && screenY < _height)
                    {
                        // 明度に応じて表示
                        if (star.Brightness > 0.1)
                        {
                            screen[screenX, screenY] = displayChar;
                            screenColors[screenX, screenY] = star.Color;
                        }
                    }
                }

                // 画面を描画
                for (int y = 0; y < _height; y++)
                {
                    for (int x = 0; x < _width; x++)
                    {
                        if (screen[x, y] != ' ')
                        {
                            Console.SetCursorPosition(x, y);
                            Console.ForegroundColor = screenColors[x, y];
                            Console.Write(screen[x, y]);
                        }
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
            // キー入力を消費
            if (Console.KeyAvailable)
            {
                Console.ReadKey(true);
            }
            
            // コンソールを元の状態に戻す
            Console.ResetColor();
            Console.CursorVisible = true;
            Console.Clear();
        }
    }

    /// <summary>
    /// 指定された時間だけ星空エフェクトを実行します
    /// </summary>
    /// <param name="duration">実行時間（ミリ秒）</param>
    public void RunForDuration(int duration)
    {
        Console.CursorVisible = false;
        Console.BackgroundColor = _backgroundColor;
        Console.Clear();

        var stars = new Star[_maxStars];
        var screen = new char[_width, _height];
        var screenColors = new ConsoleColor[_width, _height];
        
        int centerX = _width / 2;
        int centerY = _height / 2;
        var startTime = DateTime.Now;

        // 星を初期化
        for (int i = 0; i < _maxStars; i++)
        {
            stars[i] = new Star(_width, _height, _starChars, _starColors, _random);
        }

        try
        {
            while ((DateTime.Now - startTime).TotalMilliseconds < duration)
            {
                // 画面をクリア
                for (int x = 0; x < _width; x++)
                {
                    for (int y = 0; y < _height; y++)
                    {
                        screen[x, y] = ' ';
                        screenColors[x, y] = _backgroundColor;
                    }
                }

                // 星を更新して描画
                foreach (var star in stars)
                {
                    star.Update(_speed, _width, _height, _starChars, _starColors, _random);
                    
                    var (screenX, screenY, displayChar) = star.GetScreenPosition(centerX, centerY);
                    
                    if (screenX >= 0 && screenX < _width && screenY >= 0 && screenY < _height)
                    {
                        if (star.Brightness > 0.1)
                        {
                            screen[screenX, screenY] = displayChar;
                            screenColors[screenX, screenY] = star.Color;
                        }
                    }
                }

                // 画面を描画
                for (int y = 0; y < _height; y++)
                {
                    for (int x = 0; x < _width; x++)
                    {
                        if (screen[x, y] != ' ')
                        {
                            Console.SetCursorPosition(x, y);
                            Console.ForegroundColor = screenColors[x, y];
                            Console.Write(screen[x, y]);
                        }
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
            Console.ResetColor();
            Console.CursorVisible = true;
            Console.Clear();
        }
    }

    /// <summary>
    /// ワープ速度の星空エフェクト
    /// </summary>
    public static void RunWarpSpeed()
    {
        var starfield = new StarfieldEffect(
            delay: 30,
            speed: 3.0,
            maxStars: 150
        );
        
        Console.WriteLine("ワープ速度の星空エフェクトを開始します。任意のキーで停止してください...");
        starfield.Run();
    }

    /// <summary>
    /// 静かな星空エフェクト
    /// </summary>
    public static void RunQuietSpace()
    {
        var starfield = new StarfieldEffect(
            delay: 100,
            speed: 0.3,
            maxStars: 80
        );
        
        Console.WriteLine("静かな星空エフェクトを開始します。任意のキーで停止してください...");
        starfield.Run();
    }
}