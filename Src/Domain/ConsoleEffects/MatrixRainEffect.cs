using System;
using System.Threading;

namespace ConsoleEffects;

/// <summary>
/// Matrix映画風の緑の文字が縦に流れるエフェクトを提供するクラス
/// </summary>
public class MatrixRainEffect
{
    private readonly int _width;
    private readonly int _height;
    private readonly int _delay;
    private readonly Random _random;
    private readonly char[] _matrixChars;
    private readonly ConsoleColor _darkColor;
    private readonly ConsoleColor _brightColor;

    // Matrix風の文字セット（日本語のカタカナ、数字、記号）
    private static readonly char[] DefaultMatrixChars = 
    {
        'ア', 'イ', 'ウ', 'エ', 'オ', 'カ', 'キ', 'ク', 'ケ', 'コ',
        'サ', 'シ', 'ス', 'セ', 'ソ', 'タ', 'チ', 'ツ', 'テ', 'ト',
        'ナ', 'ニ', 'ヌ', 'ネ', 'ノ', 'ハ', 'ヒ', 'フ', 'ヘ', 'ホ',
        'マ', 'ミ', 'ム', 'メ', 'モ', 'ヤ', 'ユ', 'ヨ', 'ラ', 'リ',
        'ル', 'レ', 'ロ', 'ワ', 'ヲ', 'ン',
        '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
        '!', '@', '#', '$', '%', '^', '&', '*', '(', ')',
        '+', '-', '=', '[', ']', '{', '}', '|', '\\', ':',
        ';', '"', '\'', '<', '>', ',', '.', '?', '/', '~'
    };

    /// <summary>
    /// 列の状態を表すクラス
    /// </summary>
    private class Column
    {
        public int Position { get; set; }
        public int Length { get; set; }
        public int Speed { get; set; }
        public char[] Characters { get; set; }
        public bool IsActive { get; set; }

        public Column(int maxLength, char[] matrixChars, Random random)
        {
            Position = -random.Next(10, 30); // ランダムな開始遅延
            Length = random.Next(5, maxLength);
            Speed = random.Next(1, 4); // 1-3の速度
            Characters = new char[Length];
            IsActive = true;
            
            // ランダムな文字列を生成
            for (int i = 0; i < Length; i++)
            {
                Characters[i] = matrixChars[random.Next(matrixChars.Length)];
            }
        }

        public void Update(int maxHeight, Random random, char[] matrixChars)
        {
            Position += Speed;
            
            // 画面下に消えたら再初期化
            if (Position - Length > maxHeight)
            {
                Position = -random.Next(5, 20);
                Length = random.Next(5, Math.Min(15, maxHeight));
                Speed = random.Next(1, 4);
                Characters = new char[Length];
                
                for (int i = 0; i < Length; i++)
                {
                    Characters[i] = matrixChars[random.Next(matrixChars.Length)];
                }
            }
            
            // 時々文字を変更
            if (random.Next(100) < 5) // 5%の確率
            {
                int charIndex = random.Next(Characters.Length);
                Characters[charIndex] = matrixChars[random.Next(matrixChars.Length)];
            }
        }
    }

    /// <summary>
    /// MatrixRainEffectのインスタンスを初期化します
    /// </summary>
    /// <param name="width">エフェクトの幅（既定: コンソール幅）</param>
    /// <param name="height">エフェクトの高さ（既定: コンソール高さ）</param>
    /// <param name="delay">フレーム間隔（ミリ秒、既定: 50）</param>
    /// <param name="matrixChars">使用する文字セット（既定: Matrix風文字）</param>
    /// <param name="darkColor">暗い文字の色（既定: DarkGreen）</param>
    /// <param name="brightColor">明るい文字の色（既定: Green）</param>
    public MatrixRainEffect(
        int? width = null,
        int? height = null,
        int delay = 50,
        char[]? matrixChars = null,
        ConsoleColor darkColor = ConsoleColor.DarkGreen,
        ConsoleColor brightColor = ConsoleColor.Green)
    {
        _width = width ?? Math.Max(Console.WindowWidth - 1, 80);
        _height = height ?? Math.Max(Console.WindowHeight - 3, 20);
        _delay = delay;
        _matrixChars = matrixChars ?? DefaultMatrixChars;
        _darkColor = darkColor;
        _brightColor = brightColor;
        _random = new Random();
    }

    /// <summary>
    /// Matrix Rain エフェクトを実行します
    /// キー入力があるまで継続し、Ctrl+Cで停止できます
    /// </summary>
    public void Run()
    {
        Console.Clear();
        Console.CursorVisible = false;
        Console.WriteLine("Matrix Rain 起動中... 任意のキーで停止");
        
        // 各列の状態を初期化
        Column[] columns = new Column[_width];
        for (int i = 0; i < _width; i++)
        {
            columns[i] = new Column(_height, _matrixChars, _random);
        }

        try
        {
            while (!Console.KeyAvailable)
            {
                // 画面をクリア（カーソル位置をリセット）
                Console.SetCursorPosition(0, 1);
                
                // 各行を描画
                for (int y = 0; y < _height; y++)
                {
                    for (int x = 0; x < _width; x++)
                    {
                        Column column = columns[x];
                        int relativeY = y - column.Position;
                        
                        if (relativeY >= 0 && relativeY < column.Length)
                        {
                            // 文字の位置に応じて色を変更
                            if (relativeY == 0) // 先頭は明るい緑
                            {
                                Console.ForegroundColor = _brightColor;
                            }
                            else if (relativeY < column.Length / 3) // 上部1/3は少し明るい
                            {
                                Console.ForegroundColor = _brightColor;
                            }
                            else // 残りは暗い緑
                            {
                                Console.ForegroundColor = _darkColor;
                            }
                            
                            Console.Write(column.Characters[relativeY]);
                        }
                        else
                        {
                            Console.Write(' ');
                        }
                    }
                    Console.WriteLine(); // 改行
                }
                
                // 各列を更新
                for (int i = 0; i < columns.Length; i++)
                {
                    columns[i].Update(_height, _random, _matrixChars);
                }
                
                Thread.Sleep(_delay);
            }
        }
        catch (ThreadAbortException)
        {
            // 無視
        }
        finally
        {
            Console.ResetColor();
            Console.CursorVisible = true;
            Console.Clear();
            Console.WriteLine("Matrix Rain 停止");
            // キー入力をクリア
            while (Console.KeyAvailable) Console.ReadKey(true);
        }
    }

    /// <summary>
    /// カスタマイズされたMatrix Rain エフェクトを静的メソッドで実行します
    /// </summary>
    public static void RunCustom(
        int? width = null,
        int? height = null,
        int delay = 50,
        char[]? matrixChars = null,
        ConsoleColor darkColor = ConsoleColor.DarkGreen,
        ConsoleColor brightColor = ConsoleColor.Green)
    {
        var effect = new MatrixRainEffect(width, height, delay, matrixChars, darkColor, brightColor);
        effect.Run();
    }

    /// <summary>
    /// 青いMatrix風エフェクト
    /// </summary>
    public static void RunBlueMatrix()
    {
        RunCustom(darkColor: ConsoleColor.DarkBlue, brightColor: ConsoleColor.Cyan);
    }

    /// <summary>
    /// 赤いMatrix風エフェクト
    /// </summary>
    public static void RunRedMatrix()
    {
        RunCustom(darkColor: ConsoleColor.DarkRed, brightColor: ConsoleColor.Red);
    }
}