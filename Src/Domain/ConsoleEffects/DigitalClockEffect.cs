using System;
using System.Threading;

namespace ConsoleEffects;

/// <summary>
/// デジタル時計エフェクトを提供するクラス
/// 大きなASCII文字で現在時刻を表示し、1秒ごとに更新します
/// </summary>
public class DigitalClockEffect
{
    private readonly int _delay;

    public DigitalClockEffect(int delay = 1000)
    {
        _delay = delay;
    }

    /// <summary>
    /// エフェクトを実行します。
    /// </summary>
    public void Run()
    {
        Console.Clear();
        Console.CursorVisible = false;

        try
        {
            while (!Console.KeyAvailable)
            {
                var now = DateTime.Now;
                string timeStr = now.ToString("HH:mm:ss");
                
                int startY = (Console.WindowHeight - 7) / 2;
                int startX = (Console.WindowWidth - 50) / 2;

                Console.SetCursorPosition(0, 0);
                DrawBigTime(timeStr, startX, startY);
                
                // 日付を下部に小さく表示
                string dateStr = now.ToString("yyyy年MM月dd日 (ddd)");
                int dateX = (Console.WindowWidth - dateStr.Length) / 2;
                Console.SetCursorPosition(dateX, startY + 8);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write(dateStr);
                Console.ResetColor();

                Thread.Sleep(_delay);
            }
        }
        finally
        {
            Console.ResetColor();
            Console.CursorVisible = true;
            Console.Clear();
        }
    }

    private void DrawBigTime(string time, int x, int y)
    {
        string[][] digits = GetDigitPatterns();
        
        for (int row = 0; row < 7; row++)
        {
            Console.SetCursorPosition(x, y + row);
            
            foreach (char c in time)
            {
                string pattern = "";
                if (c >= '0' && c <= '9')
                {
                    pattern = digits[c - '0'][row];
                }
                else if (c == ':')
                {
                    pattern = GetColonPattern()[row];
                }
                else
                {
                    pattern = "   ";
                }
                
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(pattern);
                Console.Write(" ");
            }
        }
        Console.ResetColor();
    }

    private string[][] GetDigitPatterns()
    {
        return new string[][]
        {
            // 0
            new string[] { " ███ ", "█   █", "█   █", "█   █", "█   █", "█   █", " ███ " },
            // 1
            new string[] { "  █  ", " ██  ", "  █  ", "  █  ", "  █  ", "  █  ", " ███ " },
            // 2
            new string[] { " ███ ", "█   █", "    █", "   █ ", "  █  ", " █   ", "█████" },
            // 3
            new string[] { " ███ ", "█   █", "    █", "  ██ ", "    █", "█   █", " ███ " },
            // 4
            new string[] { "   █ ", "  ██ ", " █ █ ", "█  █ ", "█████", "   █ ", "   █ " },
            // 5
            new string[] { "█████", "█    ", "████ ", "    █", "    █", "█   █", " ███ " },
            // 6
            new string[] { "  ██ ", " █   ", "█    ", "████ ", "█   █", "█   █", " ███ " },
            // 7
            new string[] { "█████", "    █", "   █ ", "  █  ", " █   ", " █   ", " █   " },
            // 8
            new string[] { " ███ ", "█   █", "█   █", " ███ ", "█   █", "█   █", " ███ " },
            // 9
            new string[] { " ███ ", "█   █", "█   █", " ████", "    █", "   █ ", " ██  " }
        };
    }

    private string[] GetColonPattern()
    {
        return new string[] { "   ", " █ ", "   ", "   ", "   ", " █ ", "   " };
    }
}
