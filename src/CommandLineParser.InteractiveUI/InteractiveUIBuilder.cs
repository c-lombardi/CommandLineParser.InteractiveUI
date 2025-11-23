using System.Reflection;
using CommandLineParser.InteractiveUI.Infrastructure;

namespace CommandLineParser.InteractiveUI
{
    /// <summary>
    /// Fluent builder for creating InteractiveUI instances
    /// </summary>
    public sealed class InteractiveUIBuilder
    {
        private readonly Assembly[] _assemblies;
        private ICommandLineParser? _parser;

        internal InteractiveUIBuilder(Assembly[] assemblies)
        {
            _assemblies = assemblies;
        }

        /// <summary>
        /// Configure a custom parser implementation
        /// </summary>
        public InteractiveUIBuilder WithParser(ICommandLineParser parser)
        {
            _parser = parser;
            return this;
        }

        /// <summary>
        /// Build the InteractiveUI instance
        /// </summary>
        public InteractiveUI Build()
        {
            // Use provided parser or create a simple pass-through adapter
            // The interactive UI will build command lines but won't execute them by default
            var parser = _parser ?? new GenericCommandLineParserAdapter(args =>
            {
                Console.WriteLine($"Command would be executed: {string.Join(" ", args)}");
                Console.WriteLine("Note: Provide a custom ICommandLineParser to actually execute commands.");
                return 0;
            });
            return new InteractiveUI(parser, _assemblies);
        }
    }
}
