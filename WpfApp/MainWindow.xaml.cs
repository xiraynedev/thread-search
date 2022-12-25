using System;
using System.Windows;
using System.Threading;
using System.IO;

namespace WpfApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            _ = InputDirectories.Focus();
        }
        public static void GetFileSizes(string directory, out long totalSizeInBytes)
        {
            totalSizeInBytes = 0;

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
                    _ = MessageBox.Show(e.Message, "Access Denied");
                    totalSizeInBytes = 0;
                }
            }
        }
        public static string StartThreads(string[] directories)
        {
            long totalSizeInBytes = 0;
            string displayText = string.Empty;

            foreach (string directory in directories)
            {
                Thread thread = new Thread(() => GetFileSizes(directory, out totalSizeInBytes))
                {
                    Name = directory
                };

                thread.Start();
                // Using thread.Join() to wait for the threads to complete. It will lock the UI for about one second. Without using Join, the CPU time used is less than one second.
                thread.Join();

                displayText += $"The total file size for {thread.Name} is {totalSizeInBytes} Bytes. \n";
            }

            return displayText;
        }
        private void InputDirectories_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (InputDirectories.Text.Length > 0)
            {
                readFileSizesButton.IsEnabled = true;
            }
            else
            {
                readFileSizesButton.IsEnabled = false;
            }
        }

        private void ReadFileSizes(object sender, RoutedEventArgs e)
        {
            string[] directories = InputDirectories.Text.Trim().Split(' ');

            if (!(directories.Length <= 3))
            {
                _ = MessageBox.Show("Please enter up to three directories only.", "Invalid Amount");
                return;
            }

            DisplayFileSizes.Text = "";

            DisplayFileSizes.Text = StartThreads(directories);
        }
    }
}
