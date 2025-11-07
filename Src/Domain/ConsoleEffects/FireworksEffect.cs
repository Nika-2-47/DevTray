using System;
using System.Threading;

namespace ConsoleEffects
{
    public class FireworksEffect
    {
        private static readonly Random Random = new Random();

        public void Run()
        {
            Console.Clear();
            Console.CursorVisible = false;

            try
            {
                while (!Console.KeyAvailable)
                {
                    LaunchFirework();
                    Thread.Sleep(500);
                }
            }
            finally
            {
                Console.CursorVisible = true;
            }
        }

        private void LaunchFirework()
        {
            int width = Console.WindowWidth;
            int height = Console.WindowHeight;

            int launchX = Random.Next(0, width);
            int launchY = Random.Next(height / 2, height);

            for (int i = launchY; i >= 0; i--)
            {
                Console.SetCursorPosition(launchX, i);
                Console.ForegroundColor = GetRandomColor();
                Console.Write("*");
                Thread.Sleep(50);
                Console.SetCursorPosition(launchX, i);
                Console.Write(" ");
            }

            Explode(launchX, 0);
        }

        private void Explode(int x, int y)
        {
            int explosionRadius = Random.Next(3, 6);

            for (int i = 0; i < 10; i++)
            {
                foreach (var (dx, dy) in GetExplosionPattern(explosionRadius))
                {
                    int drawX = x + dx;
                    int drawY = y + dy;

                    if (drawX >= 0 && drawX < Console.WindowWidth && drawY >= 0 && drawY < Console.WindowHeight)
                    {
                        Console.SetCursorPosition(drawX, drawY);
                        Console.ForegroundColor = GetRandomColor();
                        Console.Write("*");
                    }
                }

                Thread.Sleep(100);

                foreach (var (dx, dy) in GetExplosionPattern(explosionRadius))
                {
                    int drawX = x + dx;
                    int drawY = y + dy;

                    if (drawX >= 0 && drawX < Console.WindowWidth && drawY >= 0 && drawY < Console.WindowHeight)
                    {
                        Console.SetCursorPosition(drawX, drawY);
                        Console.Write(" ");
                    }
                }
            }
        }

        private static (int, int)[] GetExplosionPattern(int radius)
        {
            var pattern = new (int, int)[radius * 8];
            int index = 0;

            for (int i = 0; i < radius; i++)
            {
                pattern[index++] = (i, 0);
                pattern[index++] = (-i, 0);
                pattern[index++] = (0, i);
                pattern[index++] = (0, -i);
                pattern[index++] = (i, i);
                pattern[index++] = (-i, -i);
                pattern[index++] = (i, -i);
                pattern[index++] = (-i, i);
            }

            return pattern;
        }

        private static ConsoleColor GetRandomColor()
        {
            var colors = new[]
            {
                ConsoleColor.Red,
                ConsoleColor.Green,
                ConsoleColor.Blue,
                ConsoleColor.Yellow,
                ConsoleColor.Cyan,
                ConsoleColor.Magenta,
                ConsoleColor.White
            };

            return colors[Random.Next(colors.Length)];
        }
    }
}