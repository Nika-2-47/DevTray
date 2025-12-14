using System;
using System.Threading;

namespace ConsoleEffects
{
    public class FractalTreeEffect : IConsoleEffect
    {
        public string Name => "FractalTree";
        public string Description => "Recursive fractal tree generation with wind animation";

        public void Run()
        {
            Console.CursorVisible = false;
            Console.Clear();
            
            int width = Console.WindowWidth;
            int height = Console.WindowHeight;
            double t = 0;

            while (!Console.KeyAvailable)
            {
                Console.Clear();
                
                // Wind simulation using sine wave
                double wind = Math.Sin(t) * 0.15;
                
                double startX = width / 2;
                double startY = height - 2;
                double initialLength = height / 4.0;
                
                // Draw the tree
                DrawBranch(startX, startY, initialLength, -Math.PI / 2, 9, wind);
                
                t += 0.15;
                Thread.Sleep(50);
            }

            Console.ResetColor();
            Console.Clear();
            Console.CursorVisible = true;
            if (Console.KeyAvailable) Console.ReadKey(true);
        }

        private void DrawBranch(double x, double y, double length, double angle, int depth, double wind)
        {
            if (depth == 0) return;

            // Calculate end point
            // Multiply X by 2.0 to correct for console character aspect ratio (usually ~1:2)
            double x2 = x + length * Math.Cos(angle) * 2.0;
            double y2 = y + length * Math.Sin(angle);

            DrawLine((int)x, (int)y, (int)x2, (int)y2, depth);

            double newLength = length * 0.75;
            
            // Wind affects smaller branches (lower depth) more
            // Base angle spread is 0.4 radians
            double angleSpread = 0.4;
            double currentWind = wind * (1.0 + (9 - depth) * 0.1);

            DrawBranch(x2, y2, newLength, angle - angleSpread + currentWind, depth - 1, wind);
            DrawBranch(x2, y2, newLength, angle + angleSpread + currentWind, depth - 1, wind);
        }

        private void DrawLine(int x0, int y0, int x1, int y1, int depth)
        {
            // Color based on depth (Trunk = Brown/Red, Leaves = Green)
            if (depth > 6) Console.ForegroundColor = ConsoleColor.DarkRed;
            else if (depth > 3) Console.ForegroundColor = ConsoleColor.DarkGreen;
            else Console.ForegroundColor = ConsoleColor.Green;

            int dx = Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
            int dy = -Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
            int err = dx + dy, e2;

            while (true)
            {
                if (x0 >= 0 && x0 < Console.WindowWidth && y0 >= 0 && y0 < Console.WindowHeight)
                {
                    Console.SetCursorPosition(x0, y0);
                    // Use different characters for trunk vs leaves
                    Console.Write(depth <= 2 ? "*" : "o");
                }
                
                if (x0 == x1 && y0 == y1) break;
                e2 = 2 * err;
                if (e2 >= dy) { err += dy; x0 += sx; }
                if (e2 <= dx) { err += dx; y0 += sy; }
            }
        }
    }
}
