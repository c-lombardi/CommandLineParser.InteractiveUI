using CommandLine;
using CommandLineParser.InteractiveUI.Infrastructure;
using InteractiveUI.Sample.Options;
using InteractiveUI.Sample.Commands;

namespace InteractiveUI.Sample.Demo
{
    /// <summary>
    /// Custom command executor for the demo application
    /// </summary>
    public sealed class DemoCommandExecutor : ICommandLineParser
    {
        public int Execute(string[] args)
        {
            return Parser.Default.ParseArguments<ListOptions, SearchOptions, CountOptions>(args)
                .MapResult(
                    (ListOptions opts) => ExecuteListCommand(opts),
                    (SearchOptions opts) => ExecuteSearchCommand(opts),
                    (CountOptions opts) => ExecuteCountCommand(opts),
                    errs => 1);
        }

        private int ExecuteListCommand(ListOptions opts)
        {
            var command = new ListCommand
            {
                Directory = opts.Directory,
                Recursive = opts.Recursive,
                Pattern = opts.Pattern,
                Verbose = opts.Verbose
            };
            return command.Execute();
        }

        private int ExecuteSearchCommand(SearchOptions opts)
        {
            var command = new SearchCommand
            {
                SearchText = opts.SearchText,
                Directory = opts.Directory,
                Pattern = opts.Pattern,
                CaseSensitive = opts.CaseSensitive
            };
            return command.Execute();
        }

        private int ExecuteCountCommand(CountOptions opts)
        {
            var command = new CountCommand
            {
                Directory = opts.Directory,
                TopCount = opts.TopCount
            };
            return command.Execute();
        }
    }
}
