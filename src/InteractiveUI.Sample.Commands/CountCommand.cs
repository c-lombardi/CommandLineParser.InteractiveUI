using InteractiveUI.Sample.Commands;

namespace InteractiveUI.Sample.Commands
{
    /// <summary>
    /// Command to count files by extension
    /// </summary>
    public sealed class CountCommand
    {
        public string Directory { get; set; } = ".";
        public int TopCount { get; set; } = 10;

        public int Execute()
        {
            try
            {
                Console.WriteLine($"Counting files in: {Directory}");
                Console.WriteLine("--------------------------------------------------");

                // Get all files
                var files = System.IO.Directory.GetFiles(Directory, "*.*", SearchOption.AllDirectories);

                // Group by extension and count
                var extensionCounts = files
                    .Select(f => Path.GetExtension(f).ToLower())
                    .Where(ext => !string.IsNullOrEmpty(ext))
                    .GroupBy(ext => ext)
                    .Select(g => new { Extension = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .Take(TopCount);

                Console.WriteLine($"Top {TopCount} file extensions:");
                foreach (var item in extensionCounts)
                {
                    Console.WriteLine($"  {item.Extension}: {item.Count} files");
                }

                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine($"Total: {files.Length} files");

                return 0;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: {ex.Message}");
                Console.ResetColor();
                return 1;
            }
        }
    }
}
