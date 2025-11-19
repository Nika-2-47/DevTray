using System;
using System.Threading;

namespace ConsoleEffects
{
    public class TunnelEffect : IConsoleEffect
    {
        public string Name => "tunnel";
        public string Description => "3D風のトンネルを進んでいくアニメーション";

        public void Run()
        {
            int width = 60;
            int height = 24;
            int frames = 200;
            int frameDelay = 50;
            double speed = 0.15;
            Console.Clear();
            for (int frame = 0; frame < frames; frame++)
            {
                Console.SetCursorPosition(0, 0);
                for (int y = 0; y < height; y++)
                {
                    double depth = (double)y / height;
                    double radius = (1.0 - depth) * (width / 2.0) * 0.9;
                    double phase = frame * speed + depth * 8.0;
                    int cx = width / 2;
                    char tunnelChar = (depth > 0.95) ? '@' : (depth > 0.85) ? '#' : (depth > 0.7) ? '*' : (depth > 0.5) ? '+' : '.';
                    char[] line = new char[width];
                    for (int x = 0; x < width; x++)
                    {
                        double dx = x - cx;
                        double angle = Math.Atan2(dx, radius);
                        double wave = Math.Sin(phase + angle * 3.0);
                        double r = Math.Abs(dx);
                        if (r < radius + wave * 2.0 && r > radius - 2.0 + wave * 2.0)
                            line[x] = tunnelChar;
                        else
                            line[x] = ' ';
                    }
                    Console.WriteLine(new string(line));
                }
                Thread.Sleep(frameDelay);
            }
        }
    }
}
