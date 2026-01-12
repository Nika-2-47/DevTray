using System;
using System.Collections.Generic;
using System.Threading;

namespace ConsoleEffects
{
    public class ParticleFountainEffect : IConsoleEffect
    {
        public string Name => "Fountain";
        public string Description => "Particle fountain simulation";

        private class Particle
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double VX { get; set; }
            public double VY { get; set; }
            public ConsoleColor Color { get; set; }
            public char Symbol { get; set; }
        }

        public void Run()
        {
            Console.CursorVisible = false;
            Console.Clear();
            
            int width = Console.WindowWidth;
            int height = Console.WindowHeight;
            
            // Double buffering buffers
            char[,] currentBuffer = new char[width, height];
            ConsoleColor[,] colorBuffer = new ConsoleColor[width, height];
            char[,] prevBuffer = new char[width, height];
            ConsoleColor[,] prevColorBuffer = new ConsoleColor[width, height];

            // Initialize previous buffer
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    prevBuffer[x, y] = ' '; 
                    prevColorBuffer[x, y] = ConsoleColor.Black;
                }
            }

            var particles = new List<Particle>();
            var rnd = new Random();
            var colors = new[] { ConsoleColor.DarkBlue, ConsoleColor.Blue, ConsoleColor.Cyan, ConsoleColor.White, ConsoleColor.Magenta };
            var symbols = new[] { '.', 'o', '*', '+', 'x', '°' };

            try
            {
                while (!Console.KeyAvailable)
                {
                    // Check for window resize
                    if (Console.WindowWidth != width || Console.WindowHeight != height)
                    {
                        width = Console.WindowWidth;
                        height = Console.WindowHeight;
                        currentBuffer = new char[width, height];
                        colorBuffer = new ConsoleColor[width, height];
                        prevBuffer = new char[width, height];
                        prevColorBuffer = new ConsoleColor[width, height];
                        
                        // Fill new previous buffer with empty
                         for (int x = 0; x < width; x++)
                        {
                            for (int y = 0; y < height; y++)
                            {
                                prevBuffer[x, y] = ' '; 
                                prevColorBuffer[x, y] = ConsoleColor.Black;
                            }
                        }
                        
                        Console.Clear();
                        particles.Clear(); // Clear particles on resize to avoid out of bounds
                    }

                    // Clear current buffer logic
                    // Instead of iterating to clear, we just overwrite when we draw particles?
                    // No, we need to know what is effectively empty space to overwrite old particles with space.
                    // So we must clear currentBuffer.
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            currentBuffer[x, y] = ' ';
                            colorBuffer[x, y] = ConsoleColor.Black;
                        }
                    }

                    // Spawn new particles
                    for (int i = 0; i < 5; i++)
                    {
                        particles.Add(new Particle
                        {
                            X = width / 2.0,
                            Y = height - 1,
                            VX = (rnd.NextDouble() - 0.5) * 4.0, 
                            VY = -2.0 - (rnd.NextDouble() * 1.5), 
                            Color = colors[rnd.Next(colors.Length)],
                            Symbol = symbols[rnd.Next(symbols.Length)]
                        });
                    }

                    // Update particles
                    for (int i = particles.Count - 1; i >= 0; i--)
                    {
                        var p = particles[i];
                        
                        p.X += p.VX;
                        p.Y += p.VY;
                        p.VY += 0.2; // Gravity
                        
                        // Boundary check
                        if (p.Y >= height || p.X < 0 || p.X >= width)
                        {
                            particles.RemoveAt(i);
                            continue;
                        }

                        int ix = (int)p.X;
                        int iy = (int)p.Y;

                        if (ix >= 0 && ix < width && iy >= 0 && iy < height)
                        {
                            currentBuffer[ix, iy] = p.Symbol;
                            colorBuffer[ix, iy] = p.Color;
                        }
                    }

                    // Render
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            char c = currentBuffer[x, y];
                            ConsoleColor col = colorBuffer[x, y];

                            // Optimization: Only write if changed
                            // Also need to write if it was something else previously and now it's space
                            if (c != prevBuffer[x, y] || (c != ' ' && col != prevColorBuffer[x, y]))
                            {
                                Console.SetCursorPosition(x, y);
                                Console.ForegroundColor = col;
                                Console.Write(c);
                                prevBuffer[x, y] = c;
                                prevColorBuffer[x, y] = col;
                            }
                        }
                    }

                    Thread.Sleep(50);
                }
            }
            finally
            {
                Console.ResetColor();
                Console.Clear();
                Console.CursorVisible = true;
                // Eat the key press
                if (Console.KeyAvailable) Console.ReadKey(true);
            }
        }
    }
}
