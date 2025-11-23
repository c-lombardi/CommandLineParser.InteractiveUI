using System.Reflection;
using System.Text;
using CommandLineParser.InteractiveUI.Infrastructure;
using CommandLineParser.InteractiveUI.Metadata;
using CommandLineParser.InteractiveUI.Metadata.Models;

namespace CommandLineParser.InteractiveUI
{
    /// <summary>
    /// Interactive text-based UI that dynamically generates menus from metadata
    /// </summary>
    public sealed class InteractiveUI
    {
        private readonly ICommandLineParser _adapter;
        private readonly Assembly[] _assemblies;

        public InteractiveUI(ICommandLineParser adapter, Assembly[] assemblies)
        {
            _adapter = adapter;
            _assemblies = assemblies;
        }

        /// <summary>
        /// Create an InteractiveUI builder from verb types
        /// </summary>
        public static InteractiveUIBuilder CreateFrom<T1, T2, T3>()
            where T1 : class
            where T2 : class
            where T3 : class
        {
            var assemblies = new[] { typeof(T1).Assembly, typeof(T2).Assembly, typeof(T3).Assembly }
                .Distinct()
                .ToArray();
            return new InteractiveUIBuilder(assemblies);
        }

        public void Run()
        {
            while (true)
            {
                try
                {
                    if (!ShowMainMenu())
                        break;
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\nERROR: {ex.Message}");
                    Console.ResetColor();
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                }
            }

            Console.WriteLine("\nGoodbye!");
        }

        private bool ShowMainMenu()
        {
            SafeClear();
            ShowHeader("MAIN MENU", "Select a command to execute");

            var verbs = CommandLineMetadataExtractor.ExtractAllVerbs(_assemblies);

            if (!verbs.Any())
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("⚠ No commands found!");
                Console.WriteLine("Make sure the Contracts assembly is loaded.");
                Console.ResetColor();
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
                return false;
            }

            // Display numbered list of commands
            for (int i = 0; i < verbs.Count; i++)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"  [{i + 1}] ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"{verbs[i].Name,-15}");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine($" - {verbs[i].HelpText}");
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"  [0] Exit");
            Console.ResetColor();

            Console.WriteLine();
            Console.Write("Enter your choice (0-" + verbs.Count + "): ");

            if (!int.TryParse(Console.ReadLine(), out int choice))
            {
                ShowError("Invalid input. Please enter a number.");
                return true;
            }

            if (choice == 0)
                return false;

            if (choice < 1 || choice > verbs.Count)
            {
                ShowError($"Invalid choice. Please select 1-{verbs.Count} or 0 to exit.");
                return true;
            }

            var selectedVerb = verbs[choice - 1];
            ExecuteCommand(selectedVerb);

            return true;
        }

        private void ExecuteCommand(VerbMetadata verb)
        {
            SafeClear();
            ShowHeader($"CONFIGURING: {verb.Name.ToUpper()}", verb.HelpText);

            var args = new List<string> { verb.Name };
            var optionValues = new Dictionary<string, string>();

            // Collect values for each option
            foreach (var option in verb.Options)
            {
                var value = PromptForOption(option);
                if (value != null)
                {
                    optionValues[option.PropertyName] = value;

                    // Add to args
                    if (option.ShortName.HasValue)
                    {
                        args.Add($"-{option.ShortName.Value}");
                    }
                    else if (!string.IsNullOrEmpty(option.LongName))
                    {
                        args.Add($"--{option.LongName}");
                    }

                    args.Add(value);
                }
            }

            // Show summary
            Console.WriteLine();
            ShowHeader("EXECUTION SUMMARY", "Review your command");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Command: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(string.Join(" ", args));
            Console.ResetColor();

            Console.WriteLine();
            Console.Write("Execute this command? (Y/n): ");
            var confirm = Console.ReadLine();

            if (confirm?.ToLower() == "n")
            {
                Console.WriteLine("Command cancelled.");
                Thread.Sleep(1000);
                return;
            }

            // Execute
            Console.WriteLine();
            DrawLine('=');
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("EXECUTING COMMAND...");
            Console.ResetColor();
            DrawLine('=');
            Console.WriteLine();

            try
            {
                int exitCode = _adapter.Execute(args.ToArray());

                Console.WriteLine();
                DrawLine('=');
                if (exitCode == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("✓ Command completed successfully!");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"✗ Command failed with exit code: {exitCode}");
                }
                Console.ResetColor();
                DrawLine('=');
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nERROR: {ex.Message}");
                Console.ResetColor();
            }

            Console.WriteLine("\nPress any key to return to main menu...");
            Console.ReadKey();
        }

        private string? PromptForOption(OptionMetadata option)
        {
            Console.WriteLine();
            DrawLine('-');

            // Show option name and description
            Console.ForegroundColor = ConsoleColor.Cyan;
            var optionDisplay = new StringBuilder();
            if (option.ShortName.HasValue)
                optionDisplay.Append($"-{option.ShortName.Value}");
            if (!string.IsNullOrEmpty(option.LongName))
            {
                if (optionDisplay.Length > 0)
                    optionDisplay.Append(" / ");
                optionDisplay.Append($"--{option.LongName}");
            }
            Console.WriteLine($"Option: {optionDisplay}");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"Description: {option.HelpText}");
            Console.WriteLine($"Type: {option.PropertyTypeName}");

            if (option.Required)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("⚠ REQUIRED");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Optional");
                if (option.DefaultValue != null)
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write($" (Default: {option.DefaultValue})");
                }
                Console.WriteLine();
            }
            Console.ResetColor();

            // Prompt based on type
            if (option.PropertyTypeName == "Boolean")
            {
                return PromptBoolean(option);
            }
            else
            {
                return PromptString(option);
            }
        }

        private string? PromptBoolean(OptionMetadata option)
        {
            Console.WriteLine("\n  [1] Yes (true)");
            Console.WriteLine("  [2] No (false)");

            if (!option.Required)
            {
                Console.WriteLine($"  [0] Use default ({option.DefaultValue ?? "false"})");
            }

            Console.Write("\nYour choice: ");
            var input = Console.ReadLine();

            if (!option.Required && input == "0")
                return null; // Use default

            if (input == "1")
                return "true";
            if (input == "2")
                return "false";

            // Default to false if invalid
            return "false";
        }

        private string? PromptString(OptionMetadata option)
        {
            Console.Write($"\nEnter value");

            if (!option.Required && option.DefaultValue != null)
            {
                Console.Write($" (or press Enter for default '{option.DefaultValue}')");
            }

            Console.Write(": ");

            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                if (option.Required)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("⚠ This option is required!");
                    Console.ResetColor();
                    return PromptString(option); // Retry
                }
                return null; // Use default
            }

            return input;
        }

        private void ShowHeader(string title, string subtitle)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            DrawLine('═');
            Console.WriteLine($"  {title}");
            if (!string.IsNullOrEmpty(subtitle))
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine($"  {subtitle}");
            }
            Console.ForegroundColor = ConsoleColor.Cyan;
            DrawLine('═');
            Console.ResetColor();
            Console.WriteLine();
        }

        private void DrawLine(char character = '-')
        {
            Console.WriteLine(new string(character, 65));
        }

        private void ShowError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n{message}");
            Console.ResetColor();
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
        private void SafeClear()
        {
            try
            {
                Console.Clear();
            }
            catch (IOException)
            {
                // Console.Clear() can fail if output is redirected or handle is invalid.
                // In this case, we just print a few newlines to simulate clearing.
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine();
            }
        }
    }
}
