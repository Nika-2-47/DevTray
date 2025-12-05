using System;
using System.Threading;

namespace ConsoleEffects
{
    public class TypewriterEffect : IConsoleEffect
    {
        public string Name => "Typewriter";
        public string Description => "Classic typewriter text animation with random glitches";

        private Random rand = new Random();
        private string[] sampleTexts = 
        {
            "The quick brown fox jumps over the lazy dog.",
            "In a world of endless possibilities, we code our dreams into reality.",
            "All your base are belong to us.",
            "Hello, World! This is a typewriter effect demonstration.",
            "To be or not to be, that is the question.",
            "I think, therefore I am.",
            "The only way to do great work is to love what you do.",
            "Stay hungry, stay foolish."
        };

        public void Run()
        {
            Console.CursorVisible = true;
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;

            int lineCount = 0;
            int maxLines = Console.WindowHeight - 2;

            while (!Console.KeyAvailable)
            {
                string text = sampleTexts[rand.Next(sampleTexts.Length)];
                
                // Type out the text character by character
                foreach (char c in text)
                {
                    if (Console.KeyAvailable) break;

                    // Random glitch effect (5% chance)
                    if (rand.Next(100) < 5)
                    {
                        char glitchChar = (char)rand.Next(33, 126);
                        Console.Write(glitchChar);
                        Thread.Sleep(50);
                        Console.Write("\b \b"); // Erase glitch
                    }

                    Console.Write(c);
                    
                    // Variable typing speed
                    int delay = c == ' ' ? 30 : rand.Next(40, 120);
                    Thread.Sleep(delay);
                }

                Console.WriteLine();
                lineCount++;

                // Pause between lines
                Thread.Sleep(rand.Next(500, 1500));

                // Scroll or clear if too many lines
                if (lineCount >= maxLines)
                {
                    Thread.Sleep(2000);
                    Console.Clear();
                    lineCount = 0;
                }
            }

            Console.ResetColor();
            Console.Clear();
            Console.CursorVisible = true;
            if (Console.KeyAvailable) Console.ReadKey(true);
        }
    }
}
