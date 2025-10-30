using System;
using System.Collections.Generic;
using System.Threading;

namespace ConsoleEffects;

/// <summary>
/// 雨が降るアニメーションを表示するコンソールエフェクトを提供するクラス
/// </summary>
public class RainEffect
{
    private readonly int _width;
    private readonly int _height;
    private readonly int _delay;
    private readonly Random _random;
    private readonly char[] _rainChars;
    private readonly char[] _splashChars;
    private readonly ConsoleColor _rainColor;
    private readonly ConsoleColor _splashColor;
    private readonly ConsoleColor _backgroundColor;
    private readonly double _intensity;
    private readonly int _maxDrops;

    // 雨粒の文字セット
    private static readonly char[] DefaultRainChars = 
    {
        '|', '¦', '│', '║', ':', ';', '!', '.'
    };

    // 水しぶきの文字セット
    private static readonly char[] DefaultSplashChars = 
    {
        '°', '˚', '·', '∘', '○', '*', '~', '-'
    };

    /// <summary>
    /// 雨粒を表すクラス
    /// </summary>
    private class RainDrop
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Speed { get; set; }
        public double Wind { get; set; }
        public char Character { get; set; }
        public int Length { get; set; } // 雨粒の軌跡の長さ
        public bool IsActive { get; set; }

        public RainDrop(int maxWidth, char[] rainChars, Random random)
        {
            Reset(maxWidth, rainChars, random);
        }

        public void Reset(int maxWidth, char[] rainChars, Random random)
        {
            X = random.NextDouble() * maxWidth;
            Y = -random.Next(1, 10); // ランダムな開始遅延
            Speed = 0.5 + random.NextDouble() * 1.0; // 0.5-1.5の速度
            Wind = (random.NextDouble() - 0.5) * 0.2; // -0.1から0.1の風
            Character = rainChars[random.Next(rainChars.Length)];
            Length = random.Next(1, 4); // 1-3の軌跡の長さ
            IsActive = true;
        }

        public void Update()
        {
            if (!IsActive) return;

            Y += Speed;
            X += Wind;

            if (X < 0 || X >= 100) // 画面外に出たら非アクティブ
            {
                IsActive = false;
            }
        }
    }

    /// <summary>
    /// 水しぶきを表すクラス
    /// </summary>
    private class Splash
    {
        public double X { get; set; }
        public double Y { get; set; }
        public char Character { get; set; }
        public int Life { get; set; }
        public int MaxLife { get; set; }
        public bool IsActive { get; set; }

        public Splash(double x, double y, char[] splashChars, Random random)
        {
            X = x + (random.NextDouble() - 0.5) * 2; // 着地点周辺に散らばる
            Y = y;
            Character = splashChars[random.Next(splashChars.Length)];
            MaxLife = random.Next(3, 8);
            Life = MaxLife;
            IsActive = true;
        }

        public void Update()
        {
            if (!IsActive) return;

            Life--;
            if (Life <= 0)
            {
                IsActive = false;
            }
        }
    }

    /// <summary>
    /// RainEffectのインスタンスを初期化します
    /// </summary>
    /// <param name="width">画面の幅（既定: コンソール幅）</param>
    /// <param name="height">画面の高さ（既定: コンソール高さ）</param>
    /// <param name="delay">フレーム間隔（ミリ秒、既定: 80）</param>
    /// <param name="rainChars">雨粒の文字セット（既定: DefaultRainChars）</param>
    /// <param name="splashChars">水しぶきの文字セット（既定: DefaultSplashChars）</param>
    /// <param name="rainColor">雨の色（既定: Blue）</param>
    /// <param name="splashColor">水しぶきの色（既定: Cyan）</param>
    /// <param name="backgroundColor">背景色（既定: Black）</param>
    /// <param name="intensity">雨の強度（既定: 1.0）</param>
    /// <param name="maxDrops">最大雨粒数（既定: 60）</param>
    public RainEffect(
        int? width = null,
        int? height = null,
        int delay = 80,
        char[]? rainChars = null,
        char[]? splashChars = null,
        ConsoleColor rainColor = ConsoleColor.Blue,
        ConsoleColor splashColor = ConsoleColor.Cyan,
        ConsoleColor backgroundColor = ConsoleColor.Black,
        double intensity = 1.0,
        int maxDrops = 60)
    {
        _width = width ?? Console.WindowWidth;
        _height = height ?? Console.WindowHeight;
        _delay = delay;
        _random = new Random();
        _rainChars = rainChars ?? DefaultRainChars;
        _splashChars = splashChars ?? DefaultSplashChars;
        _rainColor = rainColor;
        _splashColor = splashColor;
        _backgroundColor = backgroundColor;
        _intensity = intensity;
        _maxDrops = maxDrops;
    }

    /// <summary>
    /// 雨エフェクトを実行します
    /// キー入力があるまで継続し、Ctrl+Cで停止できます
    /// </summary>
    public void Run()
    {
        Console.CursorVisible = false;
        Console.BackgroundColor = _backgroundColor;
        Console.Clear();

        var rainDrops = new List<RainDrop>();
        var splashes = new List<Splash>();
        var screen = new char[_width, _height];
        var screenColors = new ConsoleColor[_width, _height];

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

                // 新しい雨粒を生成
                if (rainDrops.Count < _maxDrops && _random.NextDouble() < 0.4 * _intensity)
                {
                    rainDrops.Add(new RainDrop(_width, _rainChars, _random));
                }

                // 雨粒を更新
                for (int i = rainDrops.Count - 1; i >= 0; i--)
                {
                    var drop = rainDrops[i];
                    drop.Update();

                    // 地面に到達した場合
                    if (drop.Y >= _height - 1)
                    {
                        // 水しぶきを生成
                        if (_random.NextDouble() < 0.7) // 70%の確率で水しぶき
                        {
                            splashes.Add(new Splash(drop.X, _height - 1, _splashChars, _random));
                        }
                        rainDrops.RemoveAt(i);
                        continue;
                    }

                    // 画面外に出た場合
                    if (!drop.IsActive || drop.X < 0 || drop.X >= _width)
                    {
                        rainDrops.RemoveAt(i);
                        continue;
                    }

                    // 雨粒を画面に描画（軌跡も含む）
                    for (int trail = 0; trail < drop.Length; trail++)
                    {
                        int trailY = (int)(drop.Y - trail);
                        int trailX = (int)drop.X;

                        if (trailX >= 0 && trailX < _width && trailY >= 0 && trailY < _height)
                        {
                            char trailChar = trail == 0 ? drop.Character : 
                                           trail == 1 ? '·' : 
                                           trail == 2 ? '∘' : ' ';
                            
                            if (trailChar != ' ')
                            {
                                screen[trailX, trailY] = trailChar;
                                screenColors[trailX, trailY] = _rainColor;
                            }
                        }
                    }
                }

                // 水しぶきを更新
                for (int i = splashes.Count - 1; i >= 0; i--)
                {
                    var splash = splashes[i];
                    splash.Update();

                    if (!splash.IsActive)
                    {
                        splashes.RemoveAt(i);
                        continue;
                    }

                    // 水しぶきを描画
                    int splashX = (int)splash.X;
                    int splashY = (int)splash.Y;

                    if (splashX >= 0 && splashX < _width && splashY >= 0 && splashY < _height)
                    {
                        screen[splashX, splashY] = splash.Character;
                        screenColors[splashX, splashY] = _splashColor;
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
    /// 指定された時間だけ雨エフェクトを実行します
    /// </summary>
    /// <param name="duration">実行時間（ミリ秒）</param>
    public void RunForDuration(int duration)
    {
        Console.CursorVisible = false;
        Console.BackgroundColor = _backgroundColor;
        Console.Clear();

        var rainDrops = new List<RainDrop>();
        var splashes = new List<Splash>();
        var screen = new char[_width, _height];
        var screenColors = new ConsoleColor[_width, _height];
        var startTime = DateTime.Now;

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

                // 新しい雨粒を生成
                if (rainDrops.Count < _maxDrops && _random.NextDouble() < 0.4 * _intensity)
                {
                    rainDrops.Add(new RainDrop(_width, _rainChars, _random));
                }

                // 雨粒を更新
                for (int i = rainDrops.Count - 1; i >= 0; i--)
                {
                    var drop = rainDrops[i];
                    drop.Update();

                    // 地面に到達した場合
                    if (drop.Y >= _height - 1)
                    {
                        // 水しぶきを生成
                        if (_random.NextDouble() < 0.7)
                        {
                            splashes.Add(new Splash(drop.X, _height - 1, _splashChars, _random));
                        }
                        rainDrops.RemoveAt(i);
                        continue;
                    }

                    // 画面外に出た場合
                    if (!drop.IsActive || drop.X < 0 || drop.X >= _width)
                    {
                        rainDrops.RemoveAt(i);
                        continue;
                    }

                    // 雨粒を画面に描画（軌跡も含む）
                    for (int trail = 0; trail < drop.Length; trail++)
                    {
                        int trailY = (int)(drop.Y - trail);
                        int trailX = (int)drop.X;

                        if (trailX >= 0 && trailX < _width && trailY >= 0 && trailY < _height)
                        {
                            char trailChar = trail == 0 ? drop.Character : 
                                           trail == 1 ? '·' : 
                                           trail == 2 ? '∘' : ' ';
                            
                            if (trailChar != ' ')
                            {
                                screen[trailX, trailY] = trailChar;
                                screenColors[trailX, trailY] = _rainColor;
                            }
                        }
                    }
                }

                // 水しぶきを更新
                for (int i = splashes.Count - 1; i >= 0; i--)
                {
                    var splash = splashes[i];
                    splash.Update();

                    if (!splash.IsActive)
                    {
                        splashes.RemoveAt(i);
                        continue;
                    }

                    // 水しぶきを描画
                    int splashX = (int)splash.X;
                    int splashY = (int)splash.Y;

                    if (splashX >= 0 && splashX < _width && splashY >= 0 && splashY < _height)
                    {
                        screen[splashX, splashY] = splash.Character;
                        screenColors[splashX, splashY] = _splashColor;
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
    /// 豪雨エフェクト
    /// </summary>
    public static void RunHeavyRain()
    {
        var rain = new RainEffect(
            delay: 40,
            intensity: 2.0,
            maxDrops: 120,
            rainColor: ConsoleColor.DarkBlue,
            splashColor: ConsoleColor.Blue
        );
        
        Console.WriteLine("豪雨エフェクトを開始します。任意のキーで停止してください...");
        rain.Run();
    }

    /// <summary>
    /// 小雨エフェクト
    /// </summary>
    public static void RunLightRain()
    {
        var rain = new RainEffect(
            delay: 120,
            intensity: 0.5,
            maxDrops: 30,
            rainColor: ConsoleColor.DarkCyan,
            splashColor: ConsoleColor.DarkBlue
        );
        
        Console.WriteLine("小雨エフェクトを開始します。任意のキーで停止してください...");
        rain.Run();
    }
}