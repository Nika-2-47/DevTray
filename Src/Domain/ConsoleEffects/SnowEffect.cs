using System;
using System.Threading;

namespace ConsoleEffects;

/// <summary>
/// 雪が降るようなコンソールエフェクトを提供するクラス
/// </summary>
public class SnowEffect
{
    private readonly int _width;
    private readonly int _height;
    private readonly int _delay;
    private readonly Random _random;
    private readonly char[] _snowChars;
    private readonly ConsoleColor _snowColor;
    private readonly ConsoleColor _backgroundColor;
    private readonly int _maxFlakes;

    // 雪片の文字セット
    private static readonly char[] DefaultSnowChars = 
    {
        '*', '❄', '❅', '❆', '⋄', '◦', '∘', '○', '·', '•'
    };

    /// <summary>
    /// 雪片の状態を表すクラス
    /// </summary>
    private class Snowflake
    {
        public double X { get; set; }
        public int Y { get; set; }
        public char Character { get; set; }
        public double Speed { get; set; }
        public double Drift { get; set; } // 横方向の移動
        public bool IsActive { get; set; }

        public Snowflake(int maxWidth, char[] snowChars, Random random)
        {
            X = random.NextDouble() * maxWidth;
            Y = -random.Next(1, 10); // ランダムな開始遅延
            Character = snowChars[random.Next(snowChars.Length)];
            Speed = 0.1 + random.NextDouble() * 0.3; // 0.1 - 0.4の速度
            Drift = (random.NextDouble() - 0.5) * 0.1; // -0.05 - 0.05の横移動
            IsActive = true;
        }

        public void Reset(int maxWidth, char[] snowChars, Random random)
        {
            X = random.NextDouble() * maxWidth;
            Y = -random.Next(1, 10);
            Character = snowChars[random.Next(snowChars.Length)];
            Speed = 0.1 + random.NextDouble() * 0.3;
            Drift = (random.NextDouble() - 0.5) * 0.1;
            IsActive = true;
        }
    }

    /// <summary>
    /// SnowEffectのインスタンスを初期化します
    /// </summary>
    /// <param name="width">画面の幅（既定: コンソール幅）</param>
    /// <param name="height">画面の高さ（既定: コンソール高さ）</param>
    /// <param name="delay">フレーム間隔（ミリ秒、既定: 100）</param>
    /// <param name="snowChars">雪片の文字セット（既定: DefaultSnowChars）</param>
    /// <param name="snowColor">雪の色（既定: White）</param>
    /// <param name="backgroundColor">背景色（既定: Black）</param>
    /// <param name="maxFlakes">最大雪片数（既定: 50）</param>
    public SnowEffect(
        int? width = null,
        int? height = null,
        int delay = 100,
        char[]? snowChars = null,
        ConsoleColor snowColor = ConsoleColor.White,
        ConsoleColor backgroundColor = ConsoleColor.Black,
        int maxFlakes = 50)
    {
        _width = width ?? Console.WindowWidth;
        _height = height ?? Console.WindowHeight;
        _delay = delay;
        _random = new Random();
        _snowChars = snowChars ?? DefaultSnowChars;
        _snowColor = snowColor;
        _backgroundColor = backgroundColor;
        _maxFlakes = maxFlakes;
    }

    /// <summary>
    /// 雪エフェクトを実行します
    /// キー入力があるまで継続し、Ctrl+Cで停止できます
    /// </summary>
    public void Run()
    {
        Console.CursorVisible = false;
        Console.BackgroundColor = _backgroundColor;
        Console.Clear();

        var snowflakes = new Snowflake[_maxFlakes];
        
        // 雪片を初期化
        for (int i = 0; i < _maxFlakes; i++)
        {
            snowflakes[i] = new Snowflake(_width, _snowChars, _random);
            // 初期位置をランダムに配置
            snowflakes[i].Y = _random.Next(-_height, 0);
        }

        try
        {
            while (!Console.KeyAvailable)
            {
                // 画面をクリア（前フレームの雪片を消去）
                Console.SetCursorPosition(0, 0);
                Console.BackgroundColor = _backgroundColor;
                
                // 各雪片の位置を更新
                for (int i = 0; i < _maxFlakes; i++)
                {
                    var flake = snowflakes[i];
                    
                    if (flake.IsActive)
                    {
                        // 前の位置を消去
                        if (flake.Y >= 0 && flake.Y < _height && 
                            (int)flake.X >= 0 && (int)flake.X < _width)
                        {
                            Console.SetCursorPosition((int)flake.X, flake.Y);
                            Console.Write(' ');
                        }

                        // 新しい位置を計算
                        flake.Y += (int)(flake.Speed * 10);
                        flake.X += flake.Drift;

                        // 画面外に出たら再生成
                        if (flake.Y >= _height || flake.X < 0 || flake.X >= _width)
                        {
                            flake.Reset(_width, _snowChars, _random);
                        }

                        // 新しい位置に雪片を描画
                        if (flake.Y >= 0 && flake.Y < _height && 
                            (int)flake.X >= 0 && (int)flake.X < _width)
                        {
                            Console.SetCursorPosition((int)flake.X, flake.Y);
                            Console.ForegroundColor = _snowColor;
                            Console.Write(flake.Character);
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
    /// 指定された時間だけ雪エフェクトを実行します
    /// </summary>
    /// <param name="duration">実行時間（ミリ秒）</param>
    public void RunForDuration(int duration)
    {
        Console.CursorVisible = false;
        Console.BackgroundColor = _backgroundColor;
        Console.Clear();

        var snowflakes = new Snowflake[_maxFlakes];
        
        // 雪片を初期化
        for (int i = 0; i < _maxFlakes; i++)
        {
            snowflakes[i] = new Snowflake(_width, _snowChars, _random);
            // 初期位置をランダムに配置
            snowflakes[i].Y = _random.Next(-_height, 0);
        }

        var startTime = DateTime.Now;

        try
        {
            while ((DateTime.Now - startTime).TotalMilliseconds < duration)
            {
                // 画面をクリア（前フレームの雪片を消去）
                Console.SetCursorPosition(0, 0);
                Console.BackgroundColor = _backgroundColor;
                
                // 各雪片の位置を更新
                for (int i = 0; i < _maxFlakes; i++)
                {
                    var flake = snowflakes[i];
                    
                    if (flake.IsActive)
                    {
                        // 前の位置を消去
                        if (flake.Y >= 0 && flake.Y < _height && 
                            (int)flake.X >= 0 && (int)flake.X < _width)
                        {
                            Console.SetCursorPosition((int)flake.X, flake.Y);
                            Console.Write(' ');
                        }

                        // 新しい位置を計算
                        flake.Y += (int)(flake.Speed * 10);
                        flake.X += flake.Drift;

                        // 画面外に出たら再生成
                        if (flake.Y >= _height || flake.X < 0 || flake.X >= _width)
                        {
                            flake.Reset(_width, _snowChars, _random);
                        }

                        // 新しい位置に雪片を描画
                        if (flake.Y >= 0 && flake.Y < _height && 
                            (int)flake.X >= 0 && (int)flake.X < _width)
                        {
                            Console.SetCursorPosition((int)flake.X, flake.Y);
                            Console.ForegroundColor = _snowColor;
                            Console.Write(flake.Character);
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
}