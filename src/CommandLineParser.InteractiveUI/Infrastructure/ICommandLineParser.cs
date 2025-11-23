namespace CommandLineParser.InteractiveUI.Infrastructure
{
    /// <summary>
    /// Abstraction for command line parsing functionality
    /// </summary>
    public interface ICommandLineParser
    {
        /// <summary>
        /// Execute command line arguments and execute the appropriate command
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <returns>Exit code (0 for success, non-zero for failure)</returns>
        int Execute(string[] args);
    }
}
