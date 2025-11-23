using CommandLine;
using CommandLineParser.InteractiveUI.Infrastructure;

namespace CommandLineParser.InteractiveUI.Infrastructure
{
    /// <summary>
    /// Generic CommandLineParser adapter that delegates execution to a callback
    /// </summary>
    public sealed class GenericCommandLineParserAdapter : ICommandLineParser
    {
        private readonly Func<string[], int> _parseHandler;

        public GenericCommandLineParserAdapter(Func<string[], int> parseHandler)
        {
            _parseHandler = parseHandler ?? throw new ArgumentNullException(nameof(parseHandler));
        }

        public int Execute(string[] args)
        {
            return _parseHandler(args);
        }
    }
}
