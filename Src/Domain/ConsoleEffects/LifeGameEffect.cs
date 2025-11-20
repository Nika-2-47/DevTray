using System;
using System.Threading;

namespace ConsoleEffects;

/// <summary>
/// コンウェイのライフゲームを表示するコンソールエフェクト
/// </summary>
public class LifeGameEffect : IConsoleEffect
{
    public string Name => "LifeGame";
    public string Description => "コンウェイのライフゲームシミュレーション";

    private readonly int _width;
    private readonly int _height;
    private readonly int _delay;
    private bool[,] _grid;
    private bool[,] _nextGrid;
    private readonly Random _random;

    public LifeGameEffect(int? width = null, int? height = null, int delay = 100)
    {
        _width = width ?? Console.WindowWidth;
        _height = height ?? Console.WindowHeight;
        _delay = delay;
        _random = new Random();
        _grid = new bool[_width, _height];
        _nextGrid = new bool[_width, _height];
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        // ランダムに初期化 (約20%の生存率)
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                _grid[x, y] = _random.NextDouble() < 0.2;
            }
        }
    }

    public void Run()
    {
        Console.CursorVisible = false;
        Console.Clear();

        try
        {
            while (!Console.KeyAvailable)
            {
                Draw();
                Update();
                Thread.Sleep(_delay);
            }
            // キー入力を消費
            Console.ReadKey(true);
        }
        finally
        {
            Console.ResetColor();
            Console.CursorVisible = true;
            Console.Clear();
        }
    }

    private void Update()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                int neighbors = CountNeighbors(x, y);
                bool isAlive = _grid[x, y];

                if (isAlive)
                {
                    // 過疎または過密で死滅
                    _nextGrid[x, y] = (neighbors == 2 || neighbors == 3);
                }
                else
                {
                    // 誕生
                    _nextGrid[x, y] = (neighbors == 3);
                }
            }
        }

        // グリッドを更新
        Array.Copy(_nextGrid, _grid, _grid.Length);
    }

    private int CountNeighbors(int x, int y)
    {
        int count = 0;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue;

                int nx = (x + i + _width) % _width;
                int ny = (y + j + _height) % _height;

                if (_grid[nx, ny]) count++;
            }
        }
        return count;
    }

    private void Draw()
    {
        // 画面全体を再描画するとちらつく可能性があるため、変更があった部分だけ描画するか、
        // バッファリングを行うのが理想的だが、簡易実装としてStringBuilderを使用
        // コンソールのカーソル移動を最小限にする

        // 今回はシンプルに全描画 (パフォーマンスが気になる場合は最適化が必要)
        // 文字列連結で一括書き出し
        
        // コンソールバッファに直接書き込むのはC#標準だけでは難しいので、
        // カーソル位置をリセットして上書きする
        Console.SetCursorPosition(0, 0);
        
        var sb = new System.Text.StringBuilder();
        
        // 色の設定 (生存セルは緑、死滅セルは空白)
        // 色を変えるためにConsole.Writeを使う必要があるため、StringBuilderは使えない部分がある
        // ここではシンプルに文字だけで表現するか、色を変えるか。
        // 色を変えると遅くなるので、文字で表現する。
        
        for (int y = 0; y < _height - 1; y++) // 最後の行はスクロール防止のため避ける
        {
            for (int x = 0; x < _width; x++)
            {
                if (_grid[x, y])
                {
                    sb.Append('O'); // 生存
                }
                else
                {
                    sb.Append(' '); // 死滅
                }
            }
            if (y < _height - 2) sb.AppendLine();
        }
        
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write(sb.ToString());
    }
}
