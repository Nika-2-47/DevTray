using System;
using System.Collections.Generic;
using System.Threading;

namespace ConsoleEffects
{
    public class MazeGenEffect : IConsoleEffect
    {
        public string Name => "MazeGen";
        public string Description => "Visualizes maze generation using Recursive Backtracker";

        private int width;
        private int height;
        private int[,] maze = new int[0,0]; // 0: Wall, 1: Path
        private Random rand = new Random();

        public void Run()
        {
            Console.CursorVisible = false;
            Console.Clear();

            // Maze dimensions (must be odd)
            width = Console.WindowWidth - 1;
            height = Console.WindowHeight - 1;
            
            // Ensure odd dimensions for the algorithm
            if (width % 2 == 0) width--;
            if (height % 2 == 0) height--;

            // Safety check
            if (width < 5 || height < 5) return;

            maze = new int[width, height];

            // Initialize with walls (0) implicitly

            Stack<(int x, int y)> stack = new Stack<(int x, int y)>();
            int startX = 1;
            int startY = 1;
            maze[startX, startY] = 1;
            stack.Push((startX, startY));
            
            DrawCell(startX, startY, true); // Draw start head

            while (stack.Count > 0 && !Console.KeyAvailable)
            {
                var current = stack.Peek();
                var neighbors = GetUnvisitedNeighbors(current.x, current.y);

                if (neighbors.Count > 0)
                {
                    var next = neighbors[rand.Next(neighbors.Count)];
                    
                    // Remove wall between
                    int wallX = (current.x + next.x) / 2;
                    int wallY = (current.y + next.y) / 2;
                    maze[wallX, wallY] = 1;
                    maze[next.x, next.y] = 1;

                    stack.Push(next);

                    // Draw animation
                    // Previous head becomes path
                    DrawCell(current.x, current.y, false); 
                    // Wall becomes path
                    DrawCell(wallX, wallY, false);
                    // New head
                    DrawCell(next.x, next.y, true);
                    
                    Thread.Sleep(10); // Speed of generation
                }
                else
                {
                    var back = stack.Pop();
                    // Backtracking - draw as path (or different color if desired)
                    DrawCell(back.x, back.y, false);
                    
                    // If stack has items, highlight the new top as head (optional, looks nice)
                    if (stack.Count > 0)
                    {
                        var prev = stack.Peek();
                        DrawCell(prev.x, prev.y, true);
                        Thread.Sleep(5); // Faster backtracking
                    }
                }
            }

            // Final cleanup
            Console.ResetColor();
            Console.CursorVisible = true;
            
            // Wait for key if finished naturally
            if (!Console.KeyAvailable)
            {
                // Blink the "Done" status or just wait
                Console.SetCursorPosition(0, 0);
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Maze Generated!");
                Console.ReadKey(true);
            }
            else
            {
                Console.ReadKey(true);
            }
            
            Console.Clear();
        }

        private void DrawCell(int x, int y, bool isHead)
        {
            if (x >= 0 && x < Console.WindowWidth && y >= 0 && y < Console.WindowHeight)
            {
                Console.SetCursorPosition(x, y);
                if (isHead)
                {
                    Console.BackgroundColor = ConsoleColor.Red; // Head color
                    Console.Write("  "); // Double space for square-ish look if possible, but grid is 1x1
                    // Actually, grid logic assumes 1 char step. 
                    // If we want square cells, we need to adjust coordinate mapping.
                    // For simplicity, let's stick to 1 char = 1 cell.
                    Console.Write(" ");
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.White; // Path color
                    Console.Write(" ");
                }
                Console.ResetColor();
            }
        }

        private List<(int x, int y)> GetUnvisitedNeighbors(int x, int y)
        {
            var list = new List<(int x, int y)>();
            int[] dx = { 0, 0, 2, -2 };
            int[] dy = { -2, 2, 0, 0 };

            for (int i = 0; i < 4; i++)
            {
                int nx = x + dx[i];
                int ny = y + dy[i];

                if (nx > 0 && nx < width && ny > 0 && ny < height && maze[nx, ny] == 0)
                {
                    list.Add((nx, ny));
                }
            }
            return list;
        }
    }
}
