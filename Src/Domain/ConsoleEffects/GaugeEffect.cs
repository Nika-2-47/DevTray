using System;
using System.Threading;

namespace ConsoleEffects;

/// <summary>
/// ゲージをシミュレートするコンソールエフェクト
/// </summary>
public class GaugeEffect
{
    private readonly int _delay;
    private readonly int _maxValue;
    private readonly ConsoleColor _gaugeColor;

    public GaugeEffect(int delay = 100, int maxValue = 100, ConsoleColor gaugeColor = ConsoleColor.Green)
    {
        _delay = delay;
        _maxValue = maxValue;
        _gaugeColor = gaugeColor;
    }

    /// <summary>
    /// ゲージエフェクトを実行します。
    /// </summary>
    public void Run()
    {
        Console.Clear();
        Console.CursorVisible = false;

        try
        {
            for (int value = 0; value <= _maxValue; value++)
            {
                DrawGauge(value);
                Thread.Sleep(_delay);
            }
        }
        finally
        {
            Console.ResetColor();
            Console.CursorVisible = true;
        }
    }

    private void DrawGauge(int value)
    {
        int width = Console.WindowWidth - 10;
        int filledWidth = (int)((value / (double)_maxValue) * width);

        Console.SetCursorPosition(0, Console.WindowHeight / 2);
        Console.ForegroundColor = _gaugeColor;
        Console.Write("[" + new string('#', filledWidth) + new string('-', width - filledWidth) + $"] {value}%");
    }
}