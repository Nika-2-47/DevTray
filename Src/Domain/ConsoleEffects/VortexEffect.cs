using System;
using System.Collections.Generic;
using System.Threading;

namespace ConsoleEffects
{
    public class VortexEffect : IConsoleEffect
    {
        public string Name => "Vortex";
        public string Description => "Swirling vortex animation";

        private class Particle
        {
            public double Angle;
            public double Distance;
            public double Speed;
            public char Symbol;
            public ConsoleColor Color;
        }

        public void Run()
        {
            Console.CursorVisible = false;
            Console.Clear();

            int width = Console.WindowWidth;
            int height = Console.WindowHeight;
            int centerX = width / 2;
            int centerY = height / 2;
            
            // Adjust aspect ratio because terminal characters are usually taller than wide
            double aspectRatio = 2.0; 

            List<Particle> particles = new List<Particle>();
            Random random = new Random();
            int particleCount = 300;

            // Initialize particles
            for (int i = 0; i < particleCount; i++)
            {
                particles.Add(CreateParticle(width, height, random));
            }

            // Colors for the vortex (purple/blue/cyan theme)
            ConsoleColor[] colors = { ConsoleColor.DarkBlue, ConsoleColor.Blue, ConsoleColor.DarkCyan, ConsoleColor.Cyan, ConsoleColor.Magenta, ConsoleColor.DarkMagenta };

            while (!Console.KeyAvailable)
            {
                // Clear buffer (simulated by overwriting or clearing specific points, but clearing all might be flickery without double buffer)
                // Since we don't have a double buffer easily in System.Console without platform specific calls, 
                // we'll try to erase previous positions or just clear screen if fast enough. 
                // Let's try clearing only if we track positions, but for simplicity in this effect, 
                // let's try to just draw over. Actually, clearing is needed to remove trails.
                // A full Clear() is often too slow/flickery. 
                // Let's try to erase particles from previous frame? 
                // For this implementation, I'll use a simple approach: 
                // Since I don't want to manage a full screen buffer array here to keep it simple, 
                // I will just clear the screen. If it flickers, we can optimize.
                // Optimization: Only erase previous positions.
                
                // But first, let's try the "Erase previous, Update, Draw new" approach.
                // We need to store previous screen coordinates to erase them.
                // Actually, let's just use a full clear for now, it's the standard for these simple effects.
                // If it's too flickery, I'll switch to "draw space at old pos".
                
                // Better approach for smooth animation in console:
                // 1. Move cursor to 0,0
                // 2. Write the whole frame to a string buffer (char array)
                // 3. Write the buffer to console.
                // However, Console.Write(buffer) for the whole screen might be okay.
                
                // Let's stick to the "Erase old, Draw new" for individual particles to minimize I/O.
                
                // Erase old particles
                foreach (var p in particles)
                {
                    int x = (int)(centerX + Math.Cos(p.Angle) * p.Distance * aspectRatio);
                    int y = (int)(centerY + Math.Sin(p.Angle) * p.Distance);

                    if (x >= 0 && x < width && y >= 0 && y < height)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(' ');
                    }
                }

                // Update and Draw new particles
                foreach (var p in particles)
                {
                    // Update physics
                    // Inner particles move faster
                    p.Angle += p.Speed / (p.Distance * 0.1 + 1); 
                    p.Distance -= 0.2; // Move towards center

                    // Respawn if too close to center
                    if (p.Distance < 1)
                    {
                        var newP = CreateParticle(width, height, random);
                        p.Angle = newP.Angle;
                        p.Distance = newP.Distance;
                        p.Speed = newP.Speed;
                        p.Symbol = newP.Symbol;
                        p.Color = newP.Color;
                    }

                    // Calculate new position
                    int x = (int)(centerX + Math.Cos(p.Angle) * p.Distance * aspectRatio);
                    int y = (int)(centerY + Math.Sin(p.Angle) * p.Distance);

                    if (x >= 0 && x < width && y >= 0 && y < height)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.ForegroundColor = p.Color;
                        Console.Write(p.Symbol);
                    }
                }

                Thread.Sleep(30);
            }

            Console.ResetColor();
            Console.Clear();
            Console.CursorVisible = true;
            // Consume the key press
            if (Console.KeyAvailable) Console.ReadKey(true);
        }

        private Particle CreateParticle(int width, int height, Random random)
        {
            double maxDist = Math.Sqrt(width * width + height * height) / 2.0;
            // Spawn mostly at the outer edge, but some random distribution
            double dist = maxDist * (0.5 + 0.5 * random.NextDouble());
            
            return new Particle
            {
                Angle = random.NextDouble() * Math.PI * 2,
                Distance = dist,
                Speed = 0.1 + random.NextDouble() * 0.2,
                Symbol = random.Next(0, 2) == 0 ? '.' : '*',
                Color = GetRandomColor(random)
            };
        }

        private ConsoleColor GetRandomColor(Random random)
        {
            ConsoleColor[] colors = { ConsoleColor.DarkBlue, ConsoleColor.Blue, ConsoleColor.DarkCyan, ConsoleColor.Cyan, ConsoleColor.Magenta, ConsoleColor.DarkMagenta, ConsoleColor.White };
            return colors[random.Next(colors.Length)];
        }
    }
}
