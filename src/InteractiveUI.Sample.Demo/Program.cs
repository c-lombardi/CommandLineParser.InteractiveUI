using CommandLine;
using CommandLineParser.InteractiveUI;
using InteractiveUI.Sample.Options;

namespace InteractiveUI.Sample.Demo
{
    /// <summary>
    /// Entry point demonstrating how to use the CommandLineParser.InteractiveUI library
    /// Shows how interactive UI extends normal command-line parsing with actual command execution
    /// </summary>
    class Program
    {
        static int Main(string[] args)
        {
            Console.WriteLine("CommandLineParser.InteractiveUI - Demo Application");
            Console.WriteLine("===================================================");
            Console.WriteLine();

            // Create the demo command executor
            var executor = new DemoCommandExecutor();

            // If args provided, parse and execute normally
            if (args.Length > 0)
            {
                Console.WriteLine("Parsing and executing command-line arguments...");
                Console.WriteLine();
                return executor.Execute(args);
            }

            // No args provided - launch interactive UI
            Console.WriteLine("No arguments provided. Launching interactive UI...");
            Console.WriteLine("(Use arrow keys and Enter to select commands)");
            Console.WriteLine();

            var interactiveUi = CommandLineParser.InteractiveUI.InteractiveUI.CreateFrom<ListOptions, SearchOptions, CountOptions>()
                .WithParser(executor)
                .Build();
            interactiveUi.Run();

            return 0;
        }
    }
}
