using System.Reflection;
using CommandLine;
using CommandLineParser.InteractiveUI.Infrastructure;

namespace CommandLineParser.InteractiveUI
{
    /// <summary>
    /// Extension methods for CommandLineParser's Parser class
    /// </summary>
    public static class ParserExtensions
    {
        /// <summary>
        /// Creates an InteractiveUI from the parser with specified verb types
        /// </summary>
        public static InteractiveUI WithInteractiveUI<T1, T2, T3>(this Parser parser)
            where T1 : class
            where T2 : class
            where T3 : class
        {
            var assemblies = new[] { typeof(T1).Assembly, typeof(T2).Assembly, typeof(T3).Assembly }
                .Distinct()
                .ToArray();

            var adapter = new GenericCommandLineParserAdapter(args =>
            {
                Console.WriteLine($"Command would be executed: {string.Join(" ", args)}");
                Console.WriteLine("Note: Use InteractiveUIBuilder.WithParser() to provide custom command execution.");
                return 0;
            });
            return new InteractiveUI(adapter, assemblies);
        }
    }
}
