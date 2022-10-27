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
            // This method enumerates recursively to search file sizes in all directories and sub-directories.
            // An output parameter is specified to output the total size of the files.
            // Upon attempting to access an unauthorized directory, a message box will appear with the error
            // message and output a file size of 0.

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
            // This method assigns each directory to a thread, calls the GetFileSizes method to recursively get
            // the size of every file and get their total sum, and return the display text to the main method along
            // with the total size of the files.

            long totalSizeInBytes = 0;
            string displayText = string.Empty;

            foreach (string directory in directories)
            {
                Thread thread = new Thread(() => GetFileSizes(directory, out totalSizeInBytes))
                {
                    Name = directory
                };

                thread.Start();
                thread.Join();

                displayText += $"The total file size for {thread.Name} is {totalSizeInBytes} Bytes. \n";
            }

            return displayText;
        }
        public static bool ValidateDirectoryLength(string[] inputDirectories)
        {
            // This method validates the directories to make sure only up to three are provided.

            bool isDirectoryLengthValid = true;

            if (inputDirectories.Length > 3)
            {
                isDirectoryLengthValid = false;
            }

            return isDirectoryLengthValid;
        }
        private void BtnReadFileSizes(object sender, RoutedEventArgs e)
        {
            // The code has been split into separate methods to avoid having this method do all the tasks.
            // This method gets the directories from the textbox, sends them to have their length evaluated, clears the
            // previous textblock value, starts the threads with the directories, and displays the results in the textblock.
            // If the length of the directories exceeds three, a message box is displayed and the method returns.

            string[] directories = InputDirectories.Text.Trim().Split(' ');

            if (!ValidateDirectoryLength(directories))
            {
                _ = MessageBox.Show("Please enter up to three directories only.", "Up To Three Directories Only");
                return;
            }

            DisplayFileSizes.Text = "";

            DisplayFileSizes.Text = StartThreads(directories);
        }
    }
}
