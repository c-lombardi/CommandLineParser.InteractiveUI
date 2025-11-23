using CommandLine;

namespace InteractiveUI.Sample.Options
{
    /// <summary>
    /// Command-line options for the 'count' command
    /// </summary>
    [Verb("count", HelpText = "Count files by extension.")]
    public sealed class CountOptions
    {
        [Option('d', "directory", Required = false, Default = ".", HelpText = "Directory to analyze.")]
        public string Directory { get; set; } = ".";

        [Option('n', "top", Required = false, Default = 10, HelpText = "Number of top extensions to show.")]
        public int TopCount { get; set; }
    }
}
