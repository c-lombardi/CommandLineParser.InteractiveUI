using InteractiveUI.Sample.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace InteractiveUI.Sample.Commands
{
    public sealed class SearchCommand
    {
        public string SearchText { get; set; } = "";
        public string Directory { get; set; } = ".";
        public string Pattern { get; set; } = "*.*";
        public bool CaseSensitive { get; set; }

        public int Execute()
        {
            try
            {
                Console.WriteLine($"Searching for '{SearchText}' in: {Directory}");
                Console.WriteLine($"Pattern: {Pattern}");
                Console.WriteLine($"Case sensitive: {CaseSensitive}");
                Console.WriteLine(new string('-', 50));

                var searchOption = SearchOption.AllDirectories;
                var files = System.IO.Directory.GetFiles(Directory, Pattern, searchOption);
                var comparison = CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
                int matchCount = 0;

                foreach (var file in files)
                {
                    try
                    {
                        var lines = File.ReadAllLines(file);
                        var matches = new List<(int lineNum, string line)>();

                        for (int i = 0; i < lines.Length; i++)
                        {
                            if (lines[i].Contains(SearchText, comparison))
                            {
                                matches.Add((i + 1, lines[i]));
                            }
                        }

                        if (matches.Any())
                        {
                            matchCount++;
                            Console.WriteLine($"\n{file}:");
                            foreach (var (lineNum, line) in matches)
                            {
                                Console.WriteLine($"  Line {lineNum}: {line.Trim()}");
                            }
                        }
                    }
                    catch
                    {
                        // Skip files that can't be read
                    }
                }

                Console.WriteLine(new string('-', 50));
                Console.WriteLine($"Found matches in {matchCount} file(s)");
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
