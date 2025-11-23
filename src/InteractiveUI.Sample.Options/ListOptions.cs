using CommandLine;

namespace InteractiveUI.Sample.Options
{
    /// <summary>
    /// Command-line options for the 'list' command
    /// </summary>
    [Verb("list", HelpText = "List files in a directory.")]
    public sealed class ListOptions
    {
        [Option('d', "directory", Required = false, Default = ".", HelpText = "Directory to list files from.")]
        public string Directory { get; set; } = ".";

        [Option('r', "recursive", Required = false, Default = false, HelpText = "Search recursively in subdirectories.")]
        public bool Recursive { get; set; }

        [Option('p', "pattern", Required = false, Default = "*.*", HelpText = "File pattern to match (e.g., *.txt).")]
        public string Pattern { get; set; } = "*.*";

        [Option('v', "verbose", Required = false, Default = false, HelpText = "Show detailed information.")]
        public bool Verbose { get; set; }
    }
}
