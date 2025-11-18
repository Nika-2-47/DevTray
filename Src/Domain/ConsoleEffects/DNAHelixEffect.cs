using System;
using System.Threading;

namespace ConsoleEffects
{
    public class DNAHelixEffect : IConsoleEffect
    {
        public string Name => "dna";
        public string Description => "DNAの二重らせん構造が回転するアニメーション";

        public void Run()
        {
            int width = 40;
            int height = 20;
            int helixLength = height;
            int period = 8;
            int frameDelay = 60;
            char[] bases = { 'A', 'T', 'C', 'G' };
            Console.Clear();
            for (int frame = 0; frame < 200; frame++)
            {
                Console.SetCursorPosition(0, 0);
                for (int y = 0; y < helixLength; y++)
                {
                    double angle = (frame * 0.2) + (y * 2 * Math.PI / period);
                    int x1 = (int)(width / 2 + Math.Sin(angle) * (width / 4));
                    int x2 = (int)(width / 2 + Math.Sin(angle + Math.PI) * (width / 4));
                    char base1 = bases[(y + frame) % bases.Length];
                    char base2 = bases[(y + frame + 2) % bases.Length];
                    string line = new string(' ', Math.Min(x1, x2));
                    line += base1;
                    int spaceCount = Math.Abs(x2 - x1) - 1;
                    line += (spaceCount > 0) ? new string(' ', spaceCount) : "";
                    line += base2;
                    Console.WriteLine(line.PadRight(width));
                }
                Thread.Sleep(frameDelay);
            }
        }
    }
}
