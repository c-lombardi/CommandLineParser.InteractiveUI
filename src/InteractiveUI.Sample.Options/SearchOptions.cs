using CommandLine;

namespace InteractiveUI.Sample.Options
{
    /// <summary>
    /// Command-line options for the 'search' command
    /// </summary>
    [Verb("search", HelpText = "Search for text within files.")]
    public sealed class SearchOptions
    {
        [Option('t', "text", Required = true, HelpText = "Text to search for.")]
        public string SearchText { get; set; } = "";

        [Option('d', "directory", Required = false, Default = ".", HelpText = "Directory to search in.")]
        public string Directory { get; set; } = ".";

        [Option('p', "pattern", Required = false, Default = "*.*", HelpText = "File pattern to search (e.g., *.cs).")]
        public string Pattern { get; set; } = "*.*";

        [Option('c', "case-sensitive", Required = false, Default = false, HelpText = "Perform case-sensitive search.")]
        public bool CaseSensitive { get; set; }
    }
}
