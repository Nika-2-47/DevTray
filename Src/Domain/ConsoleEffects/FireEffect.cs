using System;
using System.Collections.Generic;
using System.Threading;

namespace ConsoleEffects;

/// <summary>
/// 炎のようなアニメーションを表示するコンソールエフェクトを提供するクラス
/// </summary>
public class FireEffect
{
    private readonly int _width;
    private readonly int _height;
    private readonly int _delay;
    private readonly Random _random;
    private readonly char[] _fireChars;
    private readonly ConsoleColor[] _fireColors;
    private readonly ConsoleColor _backgroundColor;
    private readonly double _intensity;

    // 炎の文字セット
    private static readonly char[] DefaultFireChars = 
    {
        '▲', '▼', '◆', '♦', '█', '▓', '▒', '░', '*', '•', '∴', ':', '.', ' '
    };

    // 炎の色グラデーション（高温から低温へ）
    private static readonly ConsoleColor[] DefaultFireColors = 
    {
        ConsoleColor.White,      // 最高温（白）
        ConsoleColor.Yellow,     // 高温（黄）
        ConsoleColor.Red,        // 中温（赤）
        ConsoleColor.DarkRed,    // 低温（暗赤）
        ConsoleColor.Black       // 最低温（消滅）
    };

    /// <summary>
    /// 炎の粒子を表すクラス
    /// </summary>
    private class Flame
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double VelocityX { get; set; }
        public double VelocityY { get; set; }
        public int Intensity { get; set; }
        public int Life { get; set; }
        public int MaxLife { get; set; }
        public char Character { get; set; }
        public bool IsActive { get; set; }

        public Flame(double x, double y, char[] fireChars, Random random)
        {
            X = x;
            Y = y;
            VelocityX = (random.NextDouble() - 0.5) * 0.3; // -0.15 to 0.15
            VelocityY = -(0.2 + random.NextDouble() * 0.4); // -0.2 to -0.6 (上向き)
            Intensity = random.Next(0, 5);
            MaxLife = random.Next(10, 30);
            Life = MaxLife;
            Character = fireChars[random.Next(fireChars.Length)];
            IsActive = true;
        }

        public void Update(Random random)
        {
            if (!IsActive) return;

            X += VelocityX;
            Y += VelocityY;
            
            // 風の影響をランダムに追加
            VelocityX += (random.NextDouble() - 0.5) * 0.02;
            VelocityY += random.NextDouble() * 0.01; // 少し重力効果
            
            Life--;
            
            // 寿命による強度の減衰
            Intensity = Math.Max(0, (int)((double)Life / MaxLife * 4));
            
            if (Life <= 0 || Y < 0 || Intensity <= 0)
            {
                IsActive = false;
            }
        }
    }

    /// <summary>
    /// FireEffectのインスタンスを初期化します
    /// </summary>
    /// <param name="width">炎の幅（既定: コンソール幅）</param>
    /// <param name="height">炎の高さ（既定: コンソール高さ）</param>
    /// <param name="delay">フレーム間隔（ミリ秒、既定: 50）</param>
    /// <param name="fireChars">炎の文字セット（既定: DefaultFireChars）</param>
    /// <param name="fireColors">炎の色セット（既定: DefaultFireColors）</param>
    /// <param name="backgroundColor">背景色（既定: Black）</param>
    /// <param name="intensity">炎の強度（既定: 1.0）</param>
    public FireEffect(
        int? width = null,
        int? height = null,
        int delay = 50,
        char[]? fireChars = null,
        ConsoleColor[]? fireColors = null,
        ConsoleColor backgroundColor = ConsoleColor.Black,
        double intensity = 1.0)
    {
        _width = width ?? Console.WindowWidth;
        _height = height ?? Console.WindowHeight;
        _delay = delay;
        _random = new Random();
        _fireChars = fireChars ?? DefaultFireChars;
        _fireColors = fireColors ?? DefaultFireColors;
        _backgroundColor = backgroundColor;
        _intensity = intensity;
    }

    /// <summary>
    /// 炎エフェクトを実行します
    /// キー入力があるまで継続し、Ctrl+Cで停止できます
    /// </summary>
    public void Run()
    {
        Console.CursorVisible = false;
        Console.BackgroundColor = _backgroundColor;
        Console.Clear();

        var flames = new List<Flame>();
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

                // 新しい炎を底部から生成
                int fireSourceY = _height - 1;
                for (int x = 0; x < _width; x += 2)
                {
                    if (_random.NextDouble() < 0.3 * _intensity) // 30%の確率で新しい炎
                    {
                        flames.Add(new Flame(x + _random.NextDouble(), fireSourceY, _fireChars, _random));
                    }
                }

                // 既存の炎を更新
                for (int i = flames.Count - 1; i >= 0; i--)
                {
                    var flame = flames[i];
                    flame.Update(_random);

                    if (!flame.IsActive)
                    {
                        flames.RemoveAt(i);
                        continue;
                    }

                    // 画面に描画
                    int screenX = (int)Math.Round(flame.X);
                    int screenY = (int)Math.Round(flame.Y);

                    if (screenX >= 0 && screenX < _width && screenY >= 0 && screenY < _height)
                    {
                        screen[screenX, screenY] = flame.Character;
                        screenColors[screenX, screenY] = _fireColors[Math.Min(flame.Intensity, _fireColors.Length - 1)];
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
    /// 指定された時間だけ炎エフェクトを実行します
    /// </summary>
    /// <param name="duration">実行時間（ミリ秒）</param>
    public void RunForDuration(int duration)
    {
        Console.CursorVisible = false;
        Console.BackgroundColor = _backgroundColor;
        Console.Clear();

        var flames = new List<Flame>();
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

                // 新しい炎を底部から生成
                int fireSourceY = _height - 1;
                for (int x = 0; x < _width; x += 2)
                {
                    if (_random.NextDouble() < 0.3 * _intensity)
                    {
                        flames.Add(new Flame(x + _random.NextDouble(), fireSourceY, _fireChars, _random));
                    }
                }

                // 既存の炎を更新
                for (int i = flames.Count - 1; i >= 0; i--)
                {
                    var flame = flames[i];
                    flame.Update(_random);

                    if (!flame.IsActive)
                    {
                        flames.RemoveAt(i);
                        continue;
                    }

                    // 画面に描画
                    int screenX = (int)Math.Round(flame.X);
                    int screenY = (int)Math.Round(flame.Y);

                    if (screenX >= 0 && screenX < _width && screenY >= 0 && screenY < _height)
                    {
                        screen[screenX, screenY] = flame.Character;
                        screenColors[screenX, screenY] = _fireColors[Math.Min(flame.Intensity, _fireColors.Length - 1)];
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
    /// 青い炎エフェクト（高温炎）
    /// </summary>
    public static void RunBlueFlame()
    {
        var blueColors = new ConsoleColor[] 
        {
            ConsoleColor.White,
            ConsoleColor.Cyan,
            ConsoleColor.Blue,
            ConsoleColor.DarkBlue,
            ConsoleColor.Black
        };

        var fire = new FireEffect(
            delay: 40,
            fireColors: blueColors,
            intensity: 1.2
        );
        
        Console.WriteLine("青い炎エフェクトを開始します。任意のキーで停止してください...");
        fire.Run();
    }

    /// <summary>
    /// 緑の炎エフェクト（神秘的な炎）
    /// </summary>
    public static void RunGreenFlame()
    {
        var greenColors = new ConsoleColor[] 
        {
            ConsoleColor.White,
            ConsoleColor.Yellow,
            ConsoleColor.Green,
            ConsoleColor.DarkGreen,
            ConsoleColor.Black
        };

        var fire = new FireEffect(
            delay: 45,
            fireColors: greenColors,
            intensity: 0.8
        );
        
        Console.WriteLine("緑の炎エフェクトを開始します。任意のキーで停止してください...");
        fire.Run();
    }
}