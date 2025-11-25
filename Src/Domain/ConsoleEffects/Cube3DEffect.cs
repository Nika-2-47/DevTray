using System;
using System.Text;
using System.Threading;

namespace ConsoleEffects;

/// <summary>
/// 回転する3Dキューブを表示するエフェクト
/// </summary>
public class Cube3DEffect : IConsoleEffect
{
    public string Name => "Cube3D";
    public string Description => "回転する3Dキューブのエフェクト";

    private double _angleX = 0;
    private double _angleY = 0;
    private double _angleZ = 0;

    private struct Point3D
    {
        public double X, Y, Z;
        public Point3D(double x, double y, double z) { X = x; Y = y; Z = z; }
    }

    // キューブの頂点定義
    private readonly Point3D[] _vertices = new Point3D[]
    {
        new Point3D(-1, -1, -1), new Point3D(1, -1, -1),
        new Point3D(1, 1, -1), new Point3D(-1, 1, -1),
        new Point3D(-1, -1, 1), new Point3D(1, -1, 1),
        new Point3D(1, 1, 1), new Point3D(-1, 1, 1)
    };

    // エッジ（頂点のインデックスのペア）
    private readonly int[][] _edges = new int[][]
    {
        new[] {0,1}, new[] {1,2}, new[] {2,3}, new[] {3,0}, // 前面
        new[] {4,5}, new[] {5,6}, new[] {6,7}, new[] {7,4}, // 背面
        new[] {0,4}, new[] {1,5}, new[] {2,6}, new[] {3,7}  // 接続線
    };

    public void Run()
    {
        Console.CursorVisible = false;
        Console.Clear();
        int width = Console.WindowWidth;
        int height = Console.WindowHeight;
        
        // スケール調整（画面サイズに合わせて）
        double scale = Math.Min(width, height) / 5.0;

        try
        {
            while (!Console.KeyAvailable)
            {
                // ウィンドウサイズ変更の検知
                if (Console.WindowWidth != width || Console.WindowHeight != height)
                {
                    width = Console.WindowWidth;
                    height = Console.WindowHeight;
                    scale = Math.Min(width, height) / 5.0;
                    Console.Clear();
                }

                // バッファの初期化
                char[,] buffer = new char[height, width];
                for (int y = 0; y < height; y++)
                    for (int x = 0; x < width; x++)
                        buffer[y, x] = ' ';

                // 頂点の回転と投影
                Point3D[] projected = new Point3D[8];
                for (int i = 0; i < 8; i++)
                {
                    Point3D v = _vertices[i];
                    
                    // X軸回転
                    double y1 = v.Y * Math.Cos(_angleX) - v.Z * Math.Sin(_angleX);
                    double z1 = v.Y * Math.Sin(_angleX) + v.Z * Math.Cos(_angleX);
                    double x1 = v.X;

                    // Y軸回転
                    double x2 = x1 * Math.Cos(_angleY) - z1 * Math.Sin(_angleY);
                    double z2 = x1 * Math.Sin(_angleY) + z1 * Math.Cos(_angleY);
                    double y2 = y1;

                    // Z軸回転
                    double x3 = x2 * Math.Cos(_angleZ) - y2 * Math.Sin(_angleZ);
                    double y3 = x2 * Math.Sin(_angleZ) + y2 * Math.Cos(_angleZ);
                    
                    // 投影 (簡易的な透視投影風)
                    // コンソールの文字アスペクト比（縦長）を考慮してXを2倍に広げる
                    double aspect = 2.0; 
                    
                    int px = (int)(width / 2 + (x3 * scale * aspect));
                    int py = (int)(height / 2 + (y3 * scale));

                    projected[i] = new Point3D(px, py, 0);
                }

                // エッジの描画
                foreach (var edge in _edges)
                {
                    DrawLine(buffer, (int)projected[edge[0]].X, (int)projected[edge[0]].Y, 
                                     (int)projected[edge[1]].X, (int)projected[edge[1]].Y);
                }

                // バッファの描画
                Console.SetCursorPosition(0, 0);
                var sb = new StringBuilder();
                for (int y = 0; y < height - 1; y++) // 最後の行はスクロール防止のため避ける
                {
                    for (int x = 0; x < width; x++)
                    {
                        sb.Append(buffer[y, x]);
                    }
                    if (y < height - 2) sb.AppendLine();
                }
                Console.Write(sb.ToString());

                // 角度の更新
                _angleX += 0.04;
                _angleY += 0.07;
                _angleZ += 0.02;

                Thread.Sleep(33); // 約30fps
            }
            
            Console.ReadKey(true);
        }
        catch (Exception)
        {
            // エラー時は安全に終了
        }
        finally
        {
            Console.CursorVisible = true;
            Console.Clear();
        }
    }

    // ブレゼンハムの直線描画アルゴリズム
    private void DrawLine(char[,] buffer, int x0, int y0, int x1, int y1)
    {
        int height = buffer.GetLength(0);
        int width = buffer.GetLength(1);

        int dx = Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
        int dy = -Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
        int err = dx + dy, e2;

        while (true)
        {
            if (x0 >= 0 && x0 < width && y0 >= 0 && y0 < height)
            {
                // 頂点に近いほど明るい文字にするなどの工夫も可能だが、今回はシンプルに
                buffer[y0, x0] = '#'; 
            }

            if (x0 == x1 && y0 == y1) break;
            e2 = 2 * err;
            if (e2 >= dy) { err += dy; x0 += sx; }
            if (e2 <= dx) { err += dx; y0 += sy; }
        }
    }
}
