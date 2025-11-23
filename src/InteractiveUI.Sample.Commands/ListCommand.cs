using InteractiveUI.Sample.Commands;
using System;
using System.IO;
using System.Linq;

namespace InteractiveUI.Sample.Commands
{
    public sealed class ListCommand
    {
        public string Directory { get; set; } = ".";
        public bool Recursive { get; set; }
        public string Pattern { get; set; } = "*.*";
        public bool Verbose { get; set; }

        public int Execute()
        {
            try
            {
                Console.WriteLine($"Listing files in: {Directory}");
                Console.WriteLine($"Recursive: {Recursive}");
                Console.WriteLine($"Pattern: {Pattern}");
                Console.WriteLine(new string('-', 50));

                var searchOption = Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                var files = System.IO.Directory.GetFiles(Directory, Pattern, searchOption);

                if (files.Length == 0)
                {
                    Console.WriteLine("No files found.");
                    return 0;
                }

                foreach (var file in files.OrderBy(f => f))
                {
                    var fileInfo = new FileInfo(file);
                    if (Verbose)
                    {
                        Console.WriteLine($"{fileInfo.Name,-40} {fileInfo.Length,15:N0} bytes  {fileInfo.LastWriteTime:yyyy-MM-dd HH:mm:ss}");
                    }
                    else
                    {
                        Console.WriteLine(fileInfo.Name);
                    }
                }

                Console.WriteLine(new string('-', 50));
                Console.WriteLine($"Total: {files.Length} file(s)");
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return 1;
            }
        }
    }
}
