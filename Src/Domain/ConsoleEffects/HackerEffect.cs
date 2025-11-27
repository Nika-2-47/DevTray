using System;
using System.Threading;

namespace ConsoleEffects
{
    public class HackerEffect : IConsoleEffect
    {
        public string Name => "Hacker";
        public string Description => "ハッカー風の端末画面エフェクト";

        private readonly Random _random = new Random();
        private readonly string[] _codeSnippets = new[]
        {
            "void InjectPayload(string target) {",
            "    if (Connect(target)) {",
            "        Upload(VIRUS_SIGNATURE);",
            "        ExecuteRemote();",
            "    }",
            "}",
            "struct DataPacket {",
            "    int id;",
            "    char buffer[1024];",
            "    bool encrypted;",
            "};",
            "SELECT * FROM users WHERE password IS NOT NULL;",
            "Decrypting private key... [OK]",
            "Bypassing firewall... [SUCCESS]",
            "Tracing IP address... 192.168.0.1",
            "Accessing mainframe...",
            "Downloading sensitive data...",
            "System.Security.Cryptography.Aes.Create()",
            "var hash = SHA256.ComputeHash(data);",
            "root@server:~# sudo rm -rf /var/log/*",
            "Initializing neural network layers...",
            "Training model: Epoch 1/100, Loss: 0.4521",
            "Buffer overflow detected at 0x8F4A21",
            "Establishing secure connection (SSL/TLS)...",
            "Scanning ports... 22, 80, 443, 8080 open",
            "Brute forcing password... ********",
            "Memory dump: 0x0045FA12 -> 0x9921",
            "Kernel panic - not syncing: Fatal exception",
            "Uploading trojan_horse.exe... 98%"
        };

        private readonly string[] _hexChars = new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F" };

        public void Run()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Initializing Hacker Mode... Press any key to stop.");
            Thread.Sleep(1000);

            try
            {
                while (!Console.KeyAvailable)
                {
                    int action = _random.Next(100);

                    if (action < 60) // 60% Code/Log
                    {
                        PrintCode();
                    }
                    else if (action < 90) // 30% Hex Dump
                    {
                        PrintHexDump();
                    }
                    else // 10% Progress Bar or Alert
                    {
                        if (_random.Next(2) == 0)
                            PrintProgressBar();
                        else
                            PrintAlert();
                    }

                    // 画面がいっぱいになったらクリアして上から再開（スクロールの代わり）
                    if (Console.CursorTop >= Console.WindowHeight - 2)
                    {
                        Console.Clear();
                    }

                    Thread.Sleep(_random.Next(50, 150));
                }
                Console.ReadKey(true);
            }
            finally
            {
                Console.ResetColor();
                Console.Clear();
            }
        }

        private void PrintCode()
        {
            string snippet = _codeSnippets[_random.Next(_codeSnippets.Length)];
            foreach (char c in snippet)
            {
                if (Console.KeyAvailable) return;
                Console.Write(c);
                // タイピング風エフェクト（少しランダムに遅延）
                if (_random.Next(10) < 2) Thread.Sleep(_random.Next(5, 20));
            }
            Console.WriteLine();
        }

        private void PrintHexDump()
        {
            Console.Write("0x" + _random.Next(0, 65535).ToString("X4") + ": ");
            for (int i = 0; i < 8; i++)
            {
                Console.Write(_hexChars[_random.Next(_hexChars.Length)] + _hexChars[_random.Next(_hexChars.Length)] + " ");
            }
            Console.WriteLine("| ........");
        }

        private void PrintProgressBar()
        {
            Console.Write("Loading: [");
            int width = _random.Next(10, 30);
            for (int i = 0; i < width; i++)
            {
                if (Console.KeyAvailable) return;
                Console.Write("#");
                Thread.Sleep(10);
            }
            Console.WriteLine("] 100%");
        }

        private void PrintAlert()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(">>> WARNING: UNAUTHORIZED ACCESS DETECTED <<<");
            Console.ForegroundColor = ConsoleColor.Green;
            Thread.Sleep(200);
        }
    }
}
