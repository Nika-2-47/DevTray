using System;
using System.Collections.Generic;
using System.Threading;

namespace ConsoleEffects
{
    public class SolarSystemEffect : IConsoleEffect
    {
        public string Name => "SolarSystem";
        public string Description => "Solar system simulation with orbiting planets";

        private class Planet
        {
            public double Distance;
            public double Angle;
            public double Speed;
            public char Symbol;
            public ConsoleColor Color;
            public bool HasMoon;
            public double MoonAngle;
            public double MoonSpeed;
            public double MoonDistance;
        }

        public void Run()
        {
            Console.CursorVisible = false;
            Console.Clear();

            int width = Console.WindowWidth;
            int height = Console.WindowHeight;
            int centerX = width / 2;
            int centerY = height / 2;
            double aspectRatio = 2.0; // Terminal characters are tall

            List<Planet> planets = new List<Planet>
            {
                new Planet { Distance = 6, Speed = 0.08, Symbol = 'o', Color = ConsoleColor.Gray }, // Mercury-ish
                new Planet { Distance = 10, Speed = 0.06, Symbol = 'O', Color = ConsoleColor.DarkYellow }, // Venus-ish
                new Planet { Distance = 14, Speed = 0.05, Symbol = '@', Color = ConsoleColor.Blue, HasMoon = true, MoonDistance = 2, MoonSpeed = 0.2 }, // Earth-ish
                new Planet { Distance = 18, Speed = 0.04, Symbol = 'o', Color = ConsoleColor.Red }, // Mars-ish
                new Planet { Distance = 26, Speed = 0.02, Symbol = '0', Color = ConsoleColor.DarkRed }, // Jupiter-ish
                new Planet { Distance = 34, Speed = 0.015, Symbol = '%', Color = ConsoleColor.Yellow }, // Saturn-ish
                new Planet { Distance = 40, Speed = 0.01, Symbol = 'o', Color = ConsoleColor.Cyan }, // Uranus-ish
                new Planet { Distance = 46, Speed = 0.008, Symbol = '.', Color = ConsoleColor.Blue }  // Neptune-ish
            };

            // Background stars
            var random = new Random();
            var stars = new List<(int x, int y)>();
            for(int i=0; i<50; i++)
            {
                stars.Add((random.Next(width), random.Next(height)));
            }

            while (!Console.KeyAvailable)
            {
                Console.Clear();

                // Draw Stars
                Console.ForegroundColor = ConsoleColor.DarkGray;
                foreach(var star in stars)
                {
                    if(star.x >= 0 && star.x < width && star.y >= 0 && star.y < height)
                    {
                        Console.SetCursorPosition(star.x, star.y);
                        Console.Write('.');
                    }
                }

                // Draw Sun
                if (centerX >= 0 && centerX < width && centerY >= 0 && centerY < height)
                {
                    Console.SetCursorPosition(centerX, centerY);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("(@)");
                }

                // Draw Planets
                foreach (var planet in planets)
                {
                    // Update Planet Position
                    planet.Angle += planet.Speed;
                    int px = (int)(centerX + Math.Cos(planet.Angle) * planet.Distance * aspectRatio);
                    int py = (int)(centerY + Math.Sin(planet.Angle) * planet.Distance);

                    if (px >= 0 && px < width && py >= 0 && py < height)
                    {
                        Console.SetCursorPosition(px, py);
                        Console.ForegroundColor = planet.Color;
                        Console.Write(planet.Symbol);
                    }

                    // Draw Moon
                    if (planet.HasMoon)
                    {
                        planet.MoonAngle += planet.MoonSpeed;
                        int mx = (int)(px + Math.Cos(planet.MoonAngle) * planet.MoonDistance * aspectRatio);
                        int my = (int)(py + Math.Sin(planet.MoonAngle) * planet.MoonDistance);
                        
                        if (mx >= 0 && mx < width && my >= 0 && my < height)
                        {
                            Console.SetCursorPosition(mx, my);
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write('.');
                        }
                    }
                }

                Thread.Sleep(50);
            }

            Console.ResetColor();
            Console.Clear();
            Console.CursorVisible = true;
            if (Console.KeyAvailable) Console.ReadKey(true);
        }
    }
}
