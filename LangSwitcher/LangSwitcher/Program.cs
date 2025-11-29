using System;
using System.Threading;
using System.Runtime.InteropServices; // P/Invoke用
using RJCP.IO.Ports; // SerialPortStreamを使用

namespace LangSwitcher
{
    class Program
    {
        // キーボード操作用のWin32 API
        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        const byte VK_IME_ON = 0x16;
        const byte VK_IME_OFF = 0x1A;
        const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        const uint KEYEVENTF_KEYUP = 0x0002;

        static void Main(string[] args)
        {
            // ポート設定
            string portName = "COM9";
            int baudRate = 9600;

            Console.WriteLine($"Arduino ({portName}) に接続中 (SerialPortStream)...");
            Console.WriteLine("機能: Lang1 -> IME OFF, Lang2 -> IME ON");

            // 接続リトライロジック
            int maxRetries = 3;
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    ConnectAndRun(portName, baudRate);
                    return; // 正常終了
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n接続エラー (試行 {i + 1}/{maxRetries}): {ex.Message}");
                    if (i < maxRetries - 1)
                    {
                        Console.WriteLine("ポートをリセットするために少し待機して再試行します...");
                        Thread.Sleep(2000); // 待機時間を少し延長
                    }
                    else
                    {
                        Console.WriteLine("接続に失敗しました。");
                        Console.WriteLine("詳細: " + ex.ToString());
                    }
                }
            }
        }

        static void ConnectAndRun(string portName, int baudRate)
        {
            // コンストラクタですべてのパラメータを指定する
            using (SerialPortStream serialPort = new SerialPortStream(portName, baudRate, 8, Parity.None, StopBits.One))
            {
                serialPort.Handshake = Handshake.None;
                
                                try {
                    serialPort.DtrEnable = true;
                    serialPort.RtsEnable = true;
                    Console.WriteLine("DTR/RTS設定完了 (Arduinoリセット)");
                } catch (Exception ex) {
                    Console.WriteLine($"DTR/RTS設定警告: {ex.Message}");
                }

                Thread.Sleep(200);

                Console.WriteLine("ポートを開いています...");
                serialPort.Open();
                Console.WriteLine("Open成功！");

                // 接続後にバッファをクリア
                serialPort.DiscardInBuffer();
                serialPort.DiscardOutBuffer();

                // 接続後にDTR/RTSを有効化（Arduinoリセット用）


                Console.WriteLine("Arduinoの起動を待機しています(2秒)...");
                Thread.Sleep(2000); 
                Console.WriteLine("接続成功！終了するには 'exit' と入力してください。");

                // 受信バッファ
                string buffer = "";
                bool isRunning = true;

                // 受信スレッド
                new Thread(() => {
                    while (isRunning && serialPort.IsOpen)
                    {
                        try
                        {
                            if (serialPort.BytesToRead > 0)
                            {
                                string data = serialPort.ReadExisting();
                                Console.Write(data);
                                
                                buffer += data;
                                if (buffer.Contains("Lang1"))
                                {
                                    Console.WriteLine("\n>> コマンド検出: Lang1 -> IME OFF");
                                    // IME OFF (VK_IME_OFF)
                                    keybd_event(VK_IME_OFF, 0, KEYEVENTF_EXTENDEDKEY, UIntPtr.Zero);
                                    keybd_event(VK_IME_OFF, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, UIntPtr.Zero);
                                    buffer = "";
                                }
                                else if (buffer.Contains("Lang2"))
                                {
                                    Console.WriteLine("\n>> コマンド検出: Lang2 -> IME ON");
                                    // IME ON (VK_IME_ON)
                                    keybd_event(VK_IME_ON, 0, KEYEVENTF_EXTENDEDKEY, UIntPtr.Zero);
                                    keybd_event(VK_IME_ON, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, UIntPtr.Zero);
                                    buffer = "";
                                }
                                if (buffer.Length > 1000) buffer = "";
                            }
                            Thread.Sleep(50);
                        }
                        catch { break; }
                    }
                }).Start();

                while (true)
                {
                    string? message = Console.ReadLine();
                    if (message == "exit") break;

                    // リセットコマンドの実装
                    if (message == "reset")
                    {
                        Console.WriteLine("Arduinoにリセット信号を送信中...");
                        try
                        {
                            // DTR信号をトグルさせることでリセットがかかります
                            // (Arduino Uno/Nanoなどの仕様)
                            serialPort.DtrEnable = false;
                            serialPort.RtsEnable = false;
                            Thread.Sleep(200); // 確実にLowにする時間
                            serialPort.DtrEnable = true;
                            serialPort.RtsEnable = true;
                            Console.WriteLine("リセット完了。再起動を待機しています...");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"リセット操作エラー: {ex.Message}");
                        }
                        continue;
                    }
                    
                    if (serialPort.IsOpen && !string.IsNullOrEmpty(message))
                    {
                        serialPort.WriteLine(message);
                    }
                }

                // ループを抜けたらフラグを下ろす
                isRunning = false;

                if (serialPort.IsOpen)
                {
                    Console.WriteLine("切断処理中...");
                    serialPort.Close();
                    Console.WriteLine("切断完了");
                }
            }
        }
    }
}
