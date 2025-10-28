using System;
using System.Threading;

namespace ConsoleEffects;

/// <summary>
/// 波のようなアニメーションを表示するコンソールエフェクトを提供するクラス
/// </summary>
public class WaveEffect
{
    private readonly int _width;
    private readonly int _height;
    private readonly int _delay;
    private readonly char _waveChar;
    private readonly ConsoleColor _waveColor;
    private readonly ConsoleColor _backgroundColor;
    private readonly double _frequency;
    private readonly double _amplitude;
    private readonly double _speed;

    /// <summary>
    /// WaveEffectのインスタンスを初期化します
    /// </summary>
    /// <param name="width">波の幅（既定: コンソール幅）</param>
    /// <param name="height">波の高さ（既定: コンソール高さ）</param>
    /// <param name="delay">フレーム間隔（ミリ秒、既定: 100）</param>
    /// <param name="waveChar">波の文字（既定: ～）</param>
    /// <param name="waveColor">波の色（既定: Cyan）</param>
    /// <param name="backgroundColor">背景色（既定: Black）</param>
    /// <param name="frequency">波の周波数（既定: 0.1）</param>
    /// <param name="amplitude">波の振幅（既定: 3.0）</param>
    /// <param name="speed">波の速度（既定: 0.2）</param>
    public WaveEffect(
        int? width = null,
        int? height = null,
        int delay = 100,
        char waveChar = '～',
        ConsoleColor waveColor = ConsoleColor.Cyan,
        ConsoleColor backgroundColor = ConsoleColor.Black,
        double frequency = 0.1,
        double amplitude = 3.0,
        double speed = 0.2)
    {
        _width = width ?? Console.WindowWidth;
        _height = height ?? Console.WindowHeight;
        _delay = delay;
        _waveChar = waveChar;
        _waveColor = waveColor;
        _backgroundColor = backgroundColor;
        _frequency = frequency;
        _amplitude = amplitude;
        _speed = speed;
    }

    /// <summary>
    /// 波エフェクトを実行します
    /// キー入力があるまで継続し、Ctrl+Cで停止できます
    /// </summary>
    public void Run()
    {
        Console.CursorVisible = false;
        Console.BackgroundColor = _backgroundColor;
        Console.Clear();

        double phase = 0.0;
        var previousWave = new int[_width];

        try
        {
            while (!Console.KeyAvailable)
            {
                // 前フレームの波を消去
                for (int x = 0; x < _width && x < previousWave.Length; x++)
                {
                    if (previousWave[x] >= 0 && previousWave[x] < _height)
                    {
                        Console.SetCursorPosition(x, previousWave[x]);
                        Console.Write(' ');
                    }
                }

                // 新しい波を計算して描画
                Console.ForegroundColor = _waveColor;
                for (int x = 0; x < _width; x++)
                {
                    double waveValue = Math.Sin((x * _frequency) + phase) * _amplitude;
                    int y = (int)(_height / 2 + waveValue);
                    
                    // 画面範囲内チェック
                    if (y >= 0 && y < _height)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(_waveChar);
                        previousWave[x] = y;
                    }
                    else
                    {
                        previousWave[x] = -1; // 無効な位置
                    }
                }

                phase += _speed;
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
    /// 指定された時間だけ波エフェクトを実行します
    /// </summary>
    /// <param name="duration">実行時間（ミリ秒）</param>
    public void RunForDuration(int duration)
    {
        Console.CursorVisible = false;
        Console.BackgroundColor = _backgroundColor;
        Console.Clear();

        double phase = 0.0;
        var previousWave = new int[_width];
        var startTime = DateTime.Now;

        try
        {
            while ((DateTime.Now - startTime).TotalMilliseconds < duration)
            {
                // 前フレームの波を消去
                for (int x = 0; x < _width && x < previousWave.Length; x++)
                {
                    if (previousWave[x] >= 0 && previousWave[x] < _height)
                    {
                        Console.SetCursorPosition(x, previousWave[x]);
                        Console.Write(' ');
                    }
                }

                // 新しい波を計算して描画
                Console.ForegroundColor = _waveColor;
                for (int x = 0; x < _width; x++)
                {
                    double waveValue = Math.Sin((x * _frequency) + phase) * _amplitude;
                    int y = (int)(_height / 2 + waveValue);
                    
                    // 画面範囲内チェック
                    if (y >= 0 && y < _height)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(_waveChar);
                        previousWave[x] = y;
                    }
                    else
                    {
                        previousWave[x] = -1; // 無効な位置
                    }
                }

                phase += _speed;
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
    /// 複数の波を重ねて表示する波エフェクト
    /// </summary>
    /// <param name="waveCount">波の数（既定: 3）</param>
    public void RunMultiWave(int waveCount = 3)
    {
        Console.CursorVisible = false;
        Console.BackgroundColor = _backgroundColor;
        Console.Clear();

        double[] phases = new double[waveCount];
        var previousWaves = new int[waveCount][];
        var colors = new ConsoleColor[] { ConsoleColor.Cyan, ConsoleColor.Blue, ConsoleColor.DarkCyan };
        var frequencies = new double[] { 0.1, 0.15, 0.08 };
        var amplitudes = new double[] { 3.0, 2.0, 4.0 };
        var speeds = new double[] { 0.2, 0.15, 0.25 };

        for (int i = 0; i < waveCount; i++)
        {
            previousWaves[i] = new int[_width];
        }

        try
        {
            while (!Console.KeyAvailable)
            {
                // 前フレームの波を消去
                for (int wave = 0; wave < waveCount; wave++)
                {
                    for (int x = 0; x < _width && x < previousWaves[wave].Length; x++)
                    {
                        if (previousWaves[wave][x] >= 0 && previousWaves[wave][x] < _height)
                        {
                            Console.SetCursorPosition(x, previousWaves[wave][x]);
                            Console.Write(' ');
                        }
                    }
                }

                // 新しい波を計算して描画
                for (int wave = 0; wave < waveCount; wave++)
                {
                    Console.ForegroundColor = colors[wave % colors.Length];
                    
                    for (int x = 0; x < _width; x++)
                    {
                        double waveValue = Math.Sin((x * frequencies[wave % frequencies.Length]) + phases[wave]) 
                                         * amplitudes[wave % amplitudes.Length];
                        int y = (int)(_height / 2 + waveValue);
                        
                        // 画面範囲内チェック
                        if (y >= 0 && y < _height)
                        {
                            Console.SetCursorPosition(x, y);
                            Console.Write(_waveChar);
                            previousWaves[wave][x] = y;
                        }
                        else
                        {
                            previousWaves[wave][x] = -1; // 無効な位置
                        }
                    }
                    
                    phases[wave] += speeds[wave % speeds.Length];
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
}