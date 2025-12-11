using System;
using System.Threading;

namespace ConsoleEffects
{
    public class DoublePendulumEffect : IConsoleEffect
    {
        public string Name => "DoublePendulum";
        public string Description => "Simulation of a double pendulum chaotic system";

        private const double G = 1.0; // Gravity
        private const double L1 = 10.0; // Length of rod 1
        private const double L2 = 10.0; // Length of rod 2
        private const double M1 = 10.0; // Mass of bob 1
        private const double M2 = 10.0; // Mass of bob 2

        public void Run()
        {
            Console.CursorVisible = false;
            Console.Clear();

            int width = Console.WindowWidth;
            int height = Console.WindowHeight;
            int centerX = width / 2;
            int centerY = height / 3;

            // Initial state
            double theta1 = Math.PI / 2;
            double theta2 = Math.PI / 2;
            double omega1 = 0;
            double omega2 = 0;
            double dt = 0.2;

            // Trail buffer
            int trailLength = 50;
            var trail = new (int x, int y)[trailLength];
            int trailIndex = 0;

            while (!Console.KeyAvailable)
            {
                // Physics calculations (Runge-Kutta or simple Euler for visual effect)
                // Using simplified equations of motion for double pendulum
                
                double num1 = -G * (2 * M1 + M2) * Math.Sin(theta1);
                double num2 = -M2 * G * Math.Sin(theta1 - 2 * theta2);
                double num3 = -2 * Math.Sin(theta1 - theta2) * M2;
                double num4 = omega2 * omega2 * L2 + omega1 * omega1 * L1 * Math.Cos(theta1 - theta2);
                double den = L1 * (2 * M1 + M2 - M2 * Math.Cos(2 * theta1 - 2 * theta2));
                
                double alpha1 = (num1 + num2 + num3 * num4) / den;

                num1 = 2 * Math.Sin(theta1 - theta2);
                num2 = (omega1 * omega1 * L1 * (M1 + M2));
                num3 = G * (M1 + M2) * Math.Cos(theta1);
                num4 = omega2 * omega2 * L2 * M2 * Math.Cos(theta1 - theta2);
                den = L2 * (2 * M1 + M2 - M2 * Math.Cos(2 * theta1 - 2 * theta2));
                
                double alpha2 = (num1 * (num2 + num3 + num4)) / den;

                omega1 += alpha1 * dt;
                omega2 += alpha2 * dt;
                theta1 += omega1 * dt;
                theta2 += omega2 * dt;

                // Damping to prevent explosion due to numerical errors
                omega1 *= 0.999;
                omega2 *= 0.999;

                // Calculate positions
                // Scale for console aspect ratio (approx 2:1 char size)
                double scaleX = 2.0; 
                
                int x1 = centerX + (int)(L1 * Math.Sin(theta1) * scaleX);
                int y1 = centerY + (int)(L1 * Math.Cos(theta1));

                int x2 = x1 + (int)(L2 * Math.Sin(theta2) * scaleX);
                int y2 = y1 + (int)(L2 * Math.Cos(theta2));

                Console.Clear();

                // Draw pivot
                Console.SetCursorPosition(centerX, centerY);
                Console.Write("+");

                // Draw rods (simplified as lines)
                DrawLine(centerX, centerY, x1, y1, '.');
                DrawLine(x1, y1, x2, y2, '.');

                // Draw bobs
                if (IsValid(x1, y1, width, height))
                {
                    Console.SetCursorPosition(x1, y1);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("O");
                }

                if (IsValid(x2, y2, width, height))
                {
                    Console.SetCursorPosition(x2, y2);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("@");
                }

                // Update and draw trail
                trail[trailIndex] = (x2, y2);
                trailIndex = (trailIndex + 1) % trailLength;

                Console.ForegroundColor = ConsoleColor.DarkGray;
                for (int i = 0; i < trailLength; i++)
                {
                    var pos = trail[i];
                    if (pos.x != 0 && pos.y != 0 && IsValid(pos.x, pos.y, width, height))
                    {
                        Console.SetCursorPosition(pos.x, pos.y);
                        Console.Write("·");
                    }
                }

                Console.ResetColor();
                Thread.Sleep(30);
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

        private void DrawLine(int x0, int y0, int x1, int y1, char ch)
        {
            int dx = Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
            int dy = -Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
            int err = dx + dy, e2;

            while (true)
            {
                if (x0 >= 0 && x0 < Console.WindowWidth && y0 >= 0 && y0 < Console.WindowHeight)
                {
                    Console.SetCursorPosition(x0, y0);
                    Console.Write(ch);
                }
                
                if (x0 == x1 && y0 == y1) break;
                e2 = 2 * err;
                if (e2 >= dy) { err += dy; x0 += sx; }
                if (e2 <= dx) { err += dx; y0 += sy; }
            }
        }
    }
}
