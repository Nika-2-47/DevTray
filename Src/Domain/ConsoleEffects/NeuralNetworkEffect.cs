using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ConsoleEffects
{
    public class NeuralNetworkEffect : IConsoleEffect
    {
        public string Name => "NeuralNetwork";
        public string Description => "Visualization of a neural network with firing synapses";

        private class Node
        {
            public int X;
            public int Y;
            public double Activation;
            public int LayerIndex;
        }

        private class Connection
        {
            public Node From;
            public Node To;
            public List<double> Signals = new List<double>(); // Position of signals (0.0 to 1.0)
        }

        public void Run()
        {
            Console.CursorVisible = false;
            Console.Clear();

            int width = Console.WindowWidth;
            int height = Console.WindowHeight;
            Random rand = new Random();

            // Setup Network
            List<List<Node>> layers = new List<List<Node>>();
            List<Connection> connections = new List<Connection>();

            int[] layerSizes = { 4, 6, 6, 4 }; // Number of nodes per layer
            int layerCount = layerSizes.Length;
            int layerSpacing = width / (layerCount + 1);

            for (int i = 0; i < layerCount; i++)
            {
                var currentLayer = new List<Node>();
                int nodeCount = layerSizes[i];
                int nodeSpacing = height / (nodeCount + 1);

                for (int j = 0; j < nodeCount; j++)
                {
                    currentLayer.Add(new Node
                    {
                        X = layerSpacing * (i + 1),
                        Y = nodeSpacing * (j + 1),
                        LayerIndex = i,
                        Activation = 0
                    });
                }
                layers.Add(currentLayer);
            }

            // Create connections (fully connected between adjacent layers)
            for (int i = 0; i < layerCount - 1; i++)
            {
                foreach (var fromNode in layers[i])
                {
                    foreach (var toNode in layers[i + 1])
                    {
                        connections.Add(new Connection { From = fromNode, To = toNode });
                    }
                }
            }

            while (!Console.KeyAvailable)
            {
                // Logic Update
                
                // Randomly fire input neurons
                if (rand.NextDouble() < 0.1)
                {
                    var inputNode = layers[0][rand.Next(layers[0].Count)];
                    inputNode.Activation = 1.0;
                    
                    // Start signals on connections from this node
                    foreach (var conn in connections.Where(c => c.From == inputNode))
                    {
                        conn.Signals.Add(0);
                    }
                }

                // Move signals
                double signalSpeed = 0.1;
                foreach (var conn in connections)
                {
                    for (int i = conn.Signals.Count - 1; i >= 0; i--)
                    {
                        conn.Signals[i] += signalSpeed;
                        if (conn.Signals[i] >= 1.0)
                        {
                            conn.Signals.RemoveAt(i);
                            conn.To.Activation = 1.0; // Activate target node
                            
                            // Propagate to next layer
                            if (conn.To.LayerIndex < layerCount - 1)
                            {
                                foreach (var nextConn in connections.Where(c => c.From == conn.To))
                                {
                                    nextConn.Signals.Add(0);
                                }
                            }
                        }
                    }
                }

                // Decay activation
                foreach (var layer in layers)
                {
                    foreach (var node in layer)
                    {
                        node.Activation = Math.Max(0, node.Activation - 0.05);
                    }
                }

                // Draw
                Console.Clear();

                // Draw Connections
                foreach (var conn in connections)
                {
                    DrawLine(conn.From.X, conn.From.Y, conn.To.X, conn.To.Y, ConsoleColor.DarkGray);
                    
                    // Draw Signals
                    foreach (var sig in conn.Signals)
                    {
                        int sx = (int)(conn.From.X + (conn.To.X - conn.From.X) * sig);
                        int sy = (int)(conn.From.Y + (conn.To.Y - conn.From.Y) * sig);
                        if (sx >= 0 && sx < width && sy >= 0 && sy < height)
                        {
                            Console.SetCursorPosition(sx, sy);
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write("·");
                        }
                    }
                }

                // Draw Nodes
                foreach (var layer in layers)
                {
                    foreach (var node in layer)
                    {
                        Console.SetCursorPosition(node.X, node.Y);
                        if (node.Activation > 0.8) Console.ForegroundColor = ConsoleColor.White;
                        else if (node.Activation > 0.5) Console.ForegroundColor = ConsoleColor.Cyan;
                        else if (node.Activation > 0.2) Console.ForegroundColor = ConsoleColor.Blue;
                        else Console.ForegroundColor = ConsoleColor.DarkBlue;
                        
                        Console.Write("O");
                    }
                }

                Thread.Sleep(50);
            }

            Console.ResetColor();
            Console.Clear();
            Console.CursorVisible = true;
            if (Console.KeyAvailable) Console.ReadKey(true);
        }

        private void DrawLine(int x0, int y0, int x1, int y1, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            int dx = Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
            int dy = -Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
            int err = dx + dy, e2;

            while (true)
            {
                if (x0 >= 0 && x0 < Console.WindowWidth && y0 >= 0 && y0 < Console.WindowHeight)
                {
                    Console.SetCursorPosition(x0, y0);
                    Console.Write(".");
                }
                if (x0 == x1 && y0 == y1) break;
                e2 = 2 * err;
                if (e2 >= dy) { err += dy; x0 += sx; }
                if (e2 <= dx) { err += dx; y0 += sy; }
            }
        }
    }
}
