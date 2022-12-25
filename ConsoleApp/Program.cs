using System;
using System.IO;
using System.Threading;

namespace ConsoleApp
{
    internal class Program
    {
        public static void GetFileSizes(string directory)
        {
            long totalSizeInBytes = 0;

            if (Directory.Exists(directory))
            {
                try
                {
                    foreach (string file in Directory.EnumerateFiles(directory, "*", SearchOption.AllDirectories))
                    {
                        totalSizeInBytes += new FileInfo(file).Length;
                    }
                }
                catch (UnauthorizedAccessException e)
                {
                    Console.WriteLine(e.Message);
                    return;
                }
            }

            Console.WriteLine($"The total size of the files in directory {Thread.CurrentThread.Name} is {totalSizeInBytes} bytes.");
        }
        public static void StartThreads(string[] directories)
        {
            foreach (string directory in directories)
            {
                Thread thread = new Thread(() => GetFileSizes(directory))
                {
                    Name = directory
                };
                thread.Start();
                // Using thread.Join() to wait for the threads to complete. It will lock the UI for about one second. Without using Join, the CPU time used is less than one second.
                thread.Join();
            }
        }
        private static void Main()
        {
            bool quitProcess = false;

            while (true)
            {
                string[] directories;

                while (true)
                {
                    Console.WriteLine();
                    Console.WriteLine(@"Enter up to three input directories separated by spaces. Enter Q to quit.");
                    Console.Write(@"(e.g. c:\input c:\numbers c:\files): ");

                    directories = Console.ReadLine().Trim().Split(' ');

                    if (directories.Length == 1 && directories[0].ToLower() == "q")
                    {
                        quitProcess = true;
                        break;
                    }

                    Console.WriteLine();

                    if (directories.Length > 3)
                    {
                        Console.WriteLine("Please enter up to three directories only.");
                        continue;
                    }

                    break;
                }

                if (quitProcess)
                {
                    break;
                }

                StartThreads(directories);
            }
        }
    }
}

