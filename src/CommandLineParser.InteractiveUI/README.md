# CommandLineParser.InteractiveUI

A powerful extension library for [CommandLineParser](https://github.com/commandlineparser/commandline) that automatically generates interactive text-based UIs from your command-line verb definitions. Transform your CLI applications into user-friendly, menu-driven experiences without writing any additional UI code.

## Features

**Automatic UI Generation** - Dynamically generates interactive menus from CommandLineParser verb and option attributes  
**Rich Console UI** - Beautiful colored menus with headers, prompts, and validation  
**Flexible Integration** - Works alongside normal command-line parsing, extending rather than replacing it  
**Type-Aware Input** - Smart prompts based on option types (boolean, string, etc.)  
**Validation** - Built-in validation for required fields and type checking  
**Fluent API** - Clean builder pattern for easy configuration

## Installation

Add the `CommandLineParser.InteractiveUI` project reference to your application:

```xml
<ItemGroup>
  <ProjectReference Include="..\CommandLineParser.InteractiveUI\CommandLineParser.InteractiveUI.csproj" />
</ItemGroup>
```

## Quick Start

### 1. Define Your Command Options

Use standard CommandLineParser attributes to define your commands:

```csharp
using CommandLine;

[Verb("list", HelpText = "List files in a directory.")]
public class ListOptions
{
    [Option('d', "directory", Required = false, Default = ".", 
            HelpText = "Directory to list files from.")]
    public string Directory { get; set; } = ".";

    [Option('r', "recursive", Required = false, Default = false, 
            HelpText = "Search recursively in subdirectories.")]
    public bool Recursive { get; set; }

    [Option('p', "pattern", Required = false, Default = "*.*", 
            HelpText = "File pattern to match (e.g., *.txt).")]
    public string Pattern { get; set; } = "*.*";
}

[Verb("search", HelpText = "Search for text in files.")]
public class SearchOptions
{
    [Option('t', "text", Required = true, 
            HelpText = "Text to search for.")]
    public string SearchText { get; set; } = "";

    [Option('d', "directory", Required = false, Default = ".", 
            HelpText = "Directory to search in.")]
    public string Directory { get; set; } = ".";

    [Option('c', "case-sensitive", Required = false, Default = false, 
            HelpText = "Perform case-sensitive search.")]
    public bool CaseSensitive { get; set; }
}
```

### 2. Create a Command Executor

Implement `ICommandLineParser` to execute your commands:

```csharp
using CommandLine;
using CommandLineParser.InteractiveUI.Infrastructure;

public class MyCommandExecutor : ICommandLineParser
{
    public int Execute(string[] args)
    {
        return Parser.Default.ParseArguments<ListOptions, SearchOptions>(args)
            .MapResult(
                (ListOptions opts) => ExecuteListCommand(opts),
                (SearchOptions opts) => ExecuteSearchCommand(opts),
                errs => 1);
    }

    private int ExecuteListCommand(ListOptions opts)
    {
        Console.WriteLine($"Listing files in: {opts.Directory}");
        Console.WriteLine($"Pattern: {opts.Pattern}");
        Console.WriteLine($"Recursive: {opts.Recursive}");
        // Your command implementation here
        return 0;
    }

    private int ExecuteSearchCommand(SearchOptions opts)
    {
        Console.WriteLine($"Searching for: {opts.SearchText}");
        Console.WriteLine($"In directory: {opts.Directory}");
        Console.WriteLine($"Case-sensitive: {opts.CaseSensitive}");
        // Your command implementation here
        return 0;
    }
}
```

### 3. Launch the Interactive UI

In your `Main` method, create and run the interactive UI:

```csharp
using CommandLineParser.InteractiveUI;

class Program
{
    static int Main(string[] args)
    {
        var executor = new MyCommandExecutor();

        // If args provided, parse and execute normally
        if (args.Length > 0)
        {
            return executor.Execute(args);
        }

        // No args - launch interactive UI
        var interactiveUi = InteractiveUI
            .CreateFrom<ListOptions, SearchOptions, SearchOptions>()
            .WithParser(executor)
            .Build();
        
        interactiveUi.Run();
        return 0;
    }
}
```

## API Reference

### InteractiveUI Class

The main class that provides the interactive menu system.

#### Static Factory Method

```csharp
public static InteractiveUIBuilder CreateFrom<T1, T2, T3>()
    where T1 : class
    where T2 : class
    where T3 : class
```

Creates a builder for InteractiveUI from up to 3 verb types. The builder will scan the assemblies containing these types for all available verbs.

**Example:**
```csharp
var builder = InteractiveUI.CreateFrom<ListOptions, SearchOptions, CountOptions>();
```

#### Methods

```csharp
public void Run()
```

Starts the interactive UI loop. Displays the main menu and handles user interaction until the user chooses to exit.

### InteractiveUIBuilder Class

Fluent builder for configuring and creating `InteractiveUI` instances.

#### Methods

```csharp
public InteractiveUIBuilder WithParser(ICommandLineParser parser)
```

Configures a custom parser implementation to execute commands. This is **required** for actual command execution.

**Parameters:**
- `parser` - Implementation of `ICommandLineParser` that handles command execution

**Returns:** The builder instance for method chaining

**Example:**
```csharp
var ui = InteractiveUI.CreateFrom<Verb1, Verb2, Verb3>()
    .WithParser(new MyCommandExecutor())
    .Build();
```

```csharp
public InteractiveUI Build()
```

Builds and returns the configured `InteractiveUI` instance.

**Returns:** A new `InteractiveUI` instance ready to run

### ParserExtensions Class

Extension methods for CommandLineParser's `Parser` class.

```csharp
public static InteractiveUI WithInteractiveUI<T1, T2, T3>(this Parser parser)
    where T1 : class
    where T2 : class
    where T3 : class
```

Creates an `InteractiveUI` directly from a `Parser` instance. Note: This creates a UI with a default no-op executor that only displays command construction. Use `InteractiveUIBuilder.WithParser()` for actual command execution.

**Example:**
```csharp
using CommandLine;
using CommandLineParser.InteractiveUI;

var parser = new Parser();
var ui = parser.WithInteractiveUI<ListOptions, SearchOptions, CountOptions>();
ui.Run();
```

### ICommandLineParser Interface

Interface for implementing command executors.

```csharp
public interface ICommandLineParser
{
    int Execute(string[] args);
}
```

**Methods:**
- `Execute(string[] args)` - Executes command line arguments and returns an exit code (0 for success, non-zero for failure)

### GenericCommandLineParserAdapter Class

A convenience implementation of `ICommandLineParser` that delegates to a callback function.

```csharp
public class GenericCommandLineParserAdapter : ICommandLineParser
{
    public GenericCommandLineParserAdapter(Func<string[], int> parseHandler)
}
```

**Example:**
```csharp
var adapter = new GenericCommandLineParserAdapter(args =>
{
    Console.WriteLine($"Executing: {string.Join(" ", args)}");
    return 0;
});
```

## Usage Examples

### Example 1: Basic Interactive UI with Custom Executor

```csharp
using CommandLine;
using CommandLineParser.InteractiveUI;
using CommandLineParser.InteractiveUI.Infrastructure;

// Define your verbs
[Verb("add", HelpText = "Add two numbers.")]
public class AddOptions
{
    [Option('a', "first", Required = true, HelpText = "First number.")]
    public int First { get; set; }

    [Option('b', "second", Required = true, HelpText = "Second number.")]
    public int Second { get; set; }
}

[Verb("multiply", HelpText = "Multiply two numbers.")]
public class MultiplyOptions
{
    [Option('a', "first", Required = true, HelpText = "First number.")]
    public int First { get; set; }

    [Option('b', "second", Required = true, HelpText = "Second number.")]
    public int Second { get; set; }
}

// Create executor
public class CalculatorExecutor : ICommandLineParser
{
    public int Execute(string[] args)
    {
        return Parser.Default.ParseArguments<AddOptions, MultiplyOptions>(args)
            .MapResult(
                (AddOptions opts) => {
                    Console.WriteLine($"Result: {opts.First + opts.Second}");
                    return 0;
                },
                (MultiplyOptions opts) => {
                    Console.WriteLine($"Result: {opts.First * opts.Second}");
                    return 0;
                },
                errs => 1);
    }
}

// Launch interactive UI
class Program
{
    static void Main(string[] args)
    {
        if (args.Length > 0)
        {
            new CalculatorExecutor().Execute(args);
            return;
        }

        var ui = InteractiveUI.CreateFrom<AddOptions, MultiplyOptions, MultiplyOptions>()
            .WithParser(new CalculatorExecutor())
            .Build();
        
        ui.Run();
    }
}
```

### Example 2: Using GenericCommandLineParserAdapter

For simple scenarios, use the adapter to avoid creating a full class:

```csharp
using CommandLineParser.InteractiveUI.Infrastructure;

var executor = new GenericCommandLineParserAdapter(args =>
{
    return Parser.Default.ParseArguments<AddOptions, MultiplyOptions>(args)
        .MapResult(
            (AddOptions opts) => {
                Console.WriteLine($"Sum: {opts.First + opts.Second}");
                return 0;
            },
            (MultiplyOptions opts) => {
                Console.WriteLine($"Product: {opts.First * opts.Second}");
                return 0;
            },
            errs => 1);
});

var ui = InteractiveUI.CreateFrom<AddOptions, MultiplyOptions, MultiplyOptions>()
    .WithParser(executor)
    .Build();

ui.Run();
```

### Example 3: Dual-Mode Application (CLI + Interactive)

Build applications that work both ways - traditional command-line and interactive:

```csharp
class Program
{
    static int Main(string[] args)
    {
        var executor = new MyCommandExecutor();

        // Traditional CLI mode: execute if arguments provided
        if (args.Length > 0)
        {
            Console.WriteLine("Running in CLI mode...");
            return executor.Execute(args);
        }

        // Interactive mode: no arguments, show menus
        Console.WriteLine("No arguments provided. Launching interactive mode...");
        Console.WriteLine();
        
        var ui = InteractiveUI.CreateFrom<ListOptions, SearchOptions, CountOptions>()
            .WithParser(executor)
            .Build();
        
        ui.Run();
        return 0;
    }
}
```

### Example 4: File Operations with Rich Options

```csharp
[Verb("backup", HelpText = "Backup files to a destination.")]
public class BackupOptions
{
    [Option('s', "source", Required = true, 
            HelpText = "Source directory to backup.")]
    public string Source { get; set; } = "";

    [Option('d', "destination", Required = true, 
            HelpText = "Destination directory for backup.")]
    public string Destination { get; set; } = "";

    [Option('c', "compress", Required = false, Default = false, 
            HelpText = "Compress backup files.")]
    public bool Compress { get; set; }

    [Option('e', "exclude", Required = false, Default = "*.tmp;*.log", 
            HelpText = "File patterns to exclude (semicolon-separated).")]
    public string ExcludePatterns { get; set; } = "*.tmp;*.log";

    [Option('v', "verbose", Required = false, Default = false, 
            HelpText = "Show detailed progress.")]
    public bool Verbose { get; set; }
}

public class FileCommandExecutor : ICommandLineParser
{
    public int Execute(string[] args)
    {
        return Parser.Default.ParseArguments<BackupOptions>(args)
            .MapResult(
                (BackupOptions opts) => ExecuteBackup(opts),
                errs => 1);
    }

    private int ExecuteBackup(BackupOptions opts)
    {
        Console.WriteLine($"Backing up from: {opts.Source}");
        Console.WriteLine($"Backing up to: {opts.Destination}");
        Console.WriteLine($"Compression: {(opts.Compress ? "Enabled" : "Disabled")}");
        Console.WriteLine($"Excluded: {opts.ExcludePatterns}");
        
        // Implementation here...
        
        return 0;
    }
}
```

## How It Works

1. **Metadata Extraction**: The library uses reflection to scan assemblies for classes decorated with `[Verb]` attributes and extracts metadata about options using `[Option]` attributes.

2. **Dynamic Menu Generation**: Based on extracted metadata, it builds interactive menus showing all available commands with their help text.

3. **Interactive Input Collection**: For each option in a selected command, the UI prompts the user with type-appropriate inputs:
   - Boolean options: Yes/No choice menu
   - String/numeric options: Text input with validation
   - Required options: Enforced input validation
   - Optional options: Ability to use default values

4. **Command Construction**: User inputs are assembled into command-line argument arrays (e.g., `["list", "-d", "C:\\temp", "-r", "true"]`)

5. **Execution**: The constructed arguments are passed to your `ICommandLineParser` implementation for execution

## UI Features

The interactive UI provides:

- **Colored Headers** - Cyan headers with command names and descriptions
- **Numbered Menus** - Easy selection with numeric choices
- **Smart Prompts** - Context-aware input prompts based on option types
- **Validation Feedback** - Clear error messages with required field enforcement
- **Default Value Hints** - Shows default values for optional parameters
- **Execution Summary** - Reviews the complete command before execution
- **Confirmation Prompt** - Optional confirmation before running commands
- **Success/Error Reporting** - Color-coded execution results
- **Safe Console Handling** - Graceful degradation when console features aren't available

## Best Practices

### 1. Always Provide Help Text

Make your UI self-documenting with clear help text:

```csharp
[Verb("process", HelpText = "Process files with advanced options.")]
[Option('i', "input", Required = true, HelpText = "Input file path to process.")]
```

### 2. Use Sensible Defaults

Provide defaults for optional parameters to improve user experience:

```csharp
[Option('t', "timeout", Required = false, Default = 30, 
        HelpText = "Timeout in seconds (default: 30).")]
public int Timeout { get; set; } = 30;
```

### 3. Combine Both Modes

Support both CLI and interactive modes for maximum flexibility:

```csharp
static int Main(string[] args)
{
    var executor = new MyExecutor();
    
    // CLI mode for automation/scripting
    if (args.Length > 0)
        return executor.Execute(args);
    
    // Interactive mode for manual use
    var ui = InteractiveUI.CreateFrom<...>()
        .WithParser(executor)
        .Build();
    ui.Run();
    return 0;
}
```

### 4. Implement Proper Error Handling

Your executor should handle errors gracefully:

```csharp
private int ExecuteCommand(MyOptions opts)
{
    try
    {
        // Command logic
        return 0;
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Error: {ex.Message}");
        Console.ResetColor();
        return 1;
    }
}
```

## Requirements

- .NET 9.0 or higher
- CommandLineParser library

## License

This project is part of the CommandLineParser.InteractiveUI solution.

## Contributing

Contributions are welcome! Please ensure your code follows the existing patterns and includes appropriate documentation.
