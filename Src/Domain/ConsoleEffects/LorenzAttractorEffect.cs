using System;
using System.Collections.Generic;
using System.Threading;

namespace ConsoleEffects
{
    public class LorenzAttractorEffect : IConsoleEffect
    {
        public string Name => "LorenzAttractor";
        public string Description => "Visualization of the Lorenz chaotic attractor";

        private const double Sigma = 10.0;
        private const double Rho = 28.0;
        private const double Beta = 8.0 / 3.0;
        private const double Dt = 0.01;

        public void Run()
        {
            Console.CursorVisible = false;
            Console.Clear();

            int width = Console.WindowWidth;
            int height = Console.WindowHeight;
            int centerX = width / 2;
            int centerY = height / 2;

            double x = 0.1;
            double y = 0;
            double z = 0;

            // Buffer for trail
            var points = new List<(int x, int y, ConsoleColor color)>();
            int maxPoints = 200;
            double hue = 0;

            while (!Console.KeyAvailable)
            {
                // Calculate Lorenz equations
                double dx = (Sigma * (y - x)) * Dt;
                double dy = (x * (Rho - z) - y) * Dt;
                double dz = (x * y - Beta * z) * Dt;

                x += dx;
                y += dy;
                z += dz;

                // Project 3D to 2D (Simple orthographic with rotation or just X/Z mapping)
                // Let's rotate slightly to see the structure better
                double scale = 1.5; // Adjust scale to fit console
                
                // Simple rotation around Y axis
                double angle = 0.0; // Fixed angle or rotating? Let's keep it fixed for the attractor shape
                // Actually, rotating the view makes it look 3D
                angle = DateTime.Now.Ticks / 10000000.0; 

                double rotX = x * Math.Cos(angle) - z * Math.Sin(angle);
                double rotZ = x * Math.Sin(angle) + z * Math.Cos(angle);
                
                // 2D Projection
                // X maps to screen X (adjusted for aspect ratio 2:1)
                // Z (or Y in original coords) maps to screen Y. 
                // In Lorenz, Z is "up". So let's map Lorenz Z to Screen Y (inverted) and Lorenz X/Y to Screen X.
                
                // Let's use the rotated X and original Y (which is actually horizontal in some visualizations, but let's try)
                // Standard Lorenz: Z is vertical axis.
                
                int screenX = centerX + (int)(rotX * scale * 2.0); // *2 for char aspect
                int screenY = centerY + (int)((z - 25) * scale * -1.0); // Center Z around ~25, invert Y

                // Color cycling
                hue += 0.05;
                if (hue > 6) hue = 0;
                ConsoleColor color = GetColorFromHue(hue);

                points.Add((screenX, screenY, color));
                if (points.Count > maxPoints)
                {
                    // Erase old point
                    var old = points[0];
                    if (IsValid(old.x, old.y, width, height))
                    {
                        Console.SetCursorPosition(old.x, old.y);
                        Console.Write(" ");
                    }
                    points.RemoveAt(0);
                }

                // Draw new point
                if (IsValid(screenX, screenY, width, height))
                {
                    Console.SetCursorPosition(screenX, screenY);
                    Console.ForegroundColor = color;
                    Console.Write("*");
                }

                // Draw HUD
                Console.SetCursorPosition(0, 0);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"X: {x:F2} Y: {y:F2} Z: {z:F2}");

                Thread.Sleep(10);
            }

            Console.ResetColor();
            Console.Clear();
            Console.CursorVisible = true;
            if (Console.KeyAvailable) Console.ReadKey(true);
        }

        private bool IsValid(int x, int y, int width, int height)
        {
            return x >= 0 && x < width && y >= 0 && y < height;
        }

        private ConsoleColor GetColorFromHue(double h)
        {
            int i = (int)h;
            switch (i)
            {
                case 0: return ConsoleColor.Red;
                case 1: return ConsoleColor.Yellow;
                case 2: return ConsoleColor.Green;
                case 3: return ConsoleColor.Cyan;
                case 4: return ConsoleColor.Blue;
                case 5: return ConsoleColor.Magenta;
                default: return ConsoleColor.White;
            }
        }
    }
}
