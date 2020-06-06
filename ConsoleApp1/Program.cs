using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace ConsoleApp1
{
    class Program
    {
        const int PROCESS_WM_READ = 0x0010;
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(IntPtr hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        static void Main(string[] args)
        {
            int widthAddr = 0x010056AC; // chiều rộng map
            int heightAddr = 0x010056A8; //chiều cao map
            int firstAddr = 0x01005361; // ô mìn đầu tiên map[0][0]
            int opendAddr = 0x010057A4; // số ô đã mở
            byte[] buffer = new byte[4];
            int mywidth = 0, myheight = 0, myopend = 0;
            int mycurr = 0;
            Process process = Process.GetProcessesByName("Winmine__XP")[0];
            IntPtr processHandle = OpenProcess(PROCESS_WM_READ, false, process.Id);
            int reads = 0;













            while (true)
            {
                Console.Clear();
                ReadProcessMemory(processHandle, widthAddr, buffer, 1, ref reads);
                mywidth = BitConverter.ToInt32(buffer, 0);
                ReadProcessMemory(processHandle, heightAddr, buffer, 1, ref reads);
                myheight = BitConverter.ToInt32(buffer, 0);
                ReadProcessMemory(processHandle, opendAddr, buffer, 1, ref reads);
                myopend = BitConverter.ToInt32(buffer, 0);
                if (myopend == 0)
                {
                    for (int i = 0; i < myheight; i++)
                    {
                        for (int j = 0; j < mywidth; j++)
                        {
                            Console.Write("- ");
                        }
                        Console.WriteLine();
                    }
                }
                else
                {
                    int current = firstAddr;
                    for (int i = 0; i < myheight; i++)
                    {
                        for (int j = 0; j < mywidth; j++)
                        {
                            ReadProcessMemory(processHandle, current, buffer, 1, ref reads);
                            mycurr = BitConverter.ToInt32(buffer, 0);
                            //cout << mycurr;
                            if (mycurr == 0x8F || mycurr == 0x8E || mycurr == 0x8A) // 0x8F là bom, 0x8F là đặt cờ, 0x8A là bom mở ra sau khi kết thúc game
                            {
                                Console.Write("x ");
                            }
                            else if (mycurr == 0xCC) // 0xCC là quả bom lúc ấn vào nó nổ
                            {
                                Console.Write("0 ");
                            }
                            else if (mycurr == 0x0F)
                            { //0x0F là ô mặc định chưa mở hoặc chưa đặt cờ
                                Console.Write("- ");
                            }
                            else
                            {
                                Console.Write("  ");
                            }
                            current++; //  đi đến ô tiếp theo
                        }
                        current = (firstAddr + 0x20 * (i + 1)); //0x20 là giá trị mỗi hàng cách nhau, trừ đi là ra nó bằng 0x20
                        Console.WriteLine();
                    }
                }

                Thread.Sleep(1000); // mỗi giây update 1 lần
            }
        }

        //Console.WriteLine(mywidth);
    }
}
