# CommandLineParser.InteractiveUI

[![NuGet Version](https://img.shields.io/nuget/v/CommandLineParser.InteractiveUI.svg)](https://www.nuget.org/packages/CommandLineParser.InteractiveUI/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/CommandLineParser.InteractiveUI.svg)](https://www.nuget.org/packages/CommandLineParser.InteractiveUI/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/c-lombardi/CommandLineParser.InteractiveUI/blob/main/License.md)

A powerful extension library for [CommandLineParser](https://github.com/commandlineparser/commandline) that automatically generates interactive text-based UIs from your command-line verb definitions. Transform your CLI applications into user-friendly, menu-driven experiences without writing any additional UI code.

## Repository Overview

This repository contains two separate solutions:

1. **InteractiveUI.sln** - The core reusable library that extends CommandLineParser
2. **InteractiveUI.Samples.sln** - Sample applications demonstrating the library

## Features

✨ **Automatic UI Generation** - Dynamically generates interactive menus from CommandLineParser verb and option attributes  
🎨 **Rich Console UI** - Beautiful colored menus with headers, prompts, and validation  
🔧 **Flexible Integration** - Works alongside normal command-line parsing, extending rather than replacing it  
📝 **Type-Aware Input** - Smart prompts based on option types (boolean, string, numeric, etc.)  
✅ **Validation** - Built-in validation for required fields and type checking  
🏗️ **Fluent API** - Clean builder pattern for easy configuration  
⚡ **Dual-Mode Support** - Applications work as both traditional CLI and interactive menu systems

## Quick Start

### Using the Library in Your Project

1. **Reference the library** in your `.csproj`:
   ```xml
   <ItemGroup>
     <ProjectReference Include="..\CommandLineParser.InteractiveUI\CommandLineParser.InteractiveUI.csproj" />
   </ItemGroup>
   ```

2. **Define your command options** with standard CommandLineParser attributes:
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
   }
   ```

3. **Create a command executor**:
   ```csharp
   using CommandLineParser.InteractiveUI.Infrastructure;

   public class MyCommandExecutor : ICommandLineParser
   {
       public int Execute(string[] args)
       {
           return Parser.Default.ParseArguments<ListOptions>(args)
               .MapResult(
                   (ListOptions opts) => ExecuteList(opts),
                   errs => 1);
       }

       private int ExecuteList(ListOptions opts)
       {
           Console.WriteLine($"Listing: {opts.Directory}");
           // Your implementation...
           return 0;
       }
   }
   ```

4. **Launch the interactive UI**:
   ```csharp
   using CommandLineParser.InteractiveUI;

   class Program
   {
       static int Main(string[] args)
       {
           var executor = new MyCommandExecutor();

           // Traditional CLI mode if args provided
           if (args.Length > 0)
               return executor.Execute(args);

           // Interactive mode when launched without arguments
           var ui = InteractiveUI
               .CreateFrom<ListOptions, ListOptions, ListOptions>()
               .WithParser(executor)
               .Build();
           
           ui.Run();
           return 0;
       }
   }
   ```

## Repository Structure

```
CommandLineParser.InteractiveUI/
├── InteractiveUI.sln                          # Core library solution
├── InteractiveUI.Samples.sln                  # Sample applications solution
├── README.md                                   # This file
├── STRUCTURE.md                                # Detailed structure documentation
├── License.md                                  # License information
│
└── src/
    ├── CommandLineParser.InteractiveUI/       # Core library
    │   ├── InteractiveUI.cs                   # Main UI implementation
    │   ├── InteractiveUIBuilder.cs            # Fluent builder
    │   ├── ParserExtensions.cs                # Extension methods
    │   ├── README.md                           # Library-specific docs
    │   ├── Infrastructure/                     # Interfaces and adapters
    │   └── Metadata/                           # Reflection metadata extraction
    │
    ├── InteractiveUI.Sample.Options/          # Sample: Command options
    ├── InteractiveUI.Sample.Commands/         # Sample: Command implementations
    └── InteractiveUI.Sample.Demo/             # Sample: Demo application
```

## Solutions

### InteractiveUI.sln (Library)

The core library that provides the interactive UI functionality. Open this solution to:
- Modify or extend the library
- Build the library for distribution
- View the library source code

**Contains:**
- `CommandLineParser.InteractiveUI` - The main library project

### InteractiveUI.Samples.sln (Samples)

Sample applications demonstrating library usage. Open this solution to:
- See working examples of the library in action
- Learn implementation patterns
- Test the library with sample commands

**Contains:**
- `InteractiveUI.Sample.Options` - Sample command option definitions
- `InteractiveUI.Sample.Commands` - Sample command implementations  
- `InteractiveUI.Sample.Demo` - Console application demonstrating dual-mode (CLI + Interactive)

## Building and Running

### Build the Library

```bash
# Build the library
dotnet build InteractiveUI.sln
```

### Build and Run the Sample

```bash
# Build the sample solution
dotnet build InteractiveUI.Samples.sln

# Run in interactive mode (no arguments)
dotnet run --project src/InteractiveUI.Sample.Demo

# Run in CLI mode (with arguments)
dotnet run --project src/InteractiveUI.Sample.Demo -- list -d . -r
dotnet run --project src/InteractiveUI.Sample.Demo -- search -t "ICommand" -d src
dotnet run --project src/InteractiveUI.Sample.Demo -- count -d src -n 10
```

### Publish the Sample Application

```bash
dotnet publish src/InteractiveUI.Sample.Demo -c Release -o publish
./publish/InteractiveUI.Sample.Demo.exe
```

## Library API

### InteractiveUI Class

Main class providing interactive menu functionality.

```csharp
// Create from verb types
public static InteractiveUIBuilder CreateFrom<T1, T2, T3>()
    where T1 : class
    where T2 : class
    where T3 : class

// Run the interactive UI
public void Run()
```

### InteractiveUIBuilder Class

Fluent builder for configuration.

```csharp
// Configure command executor
public InteractiveUIBuilder WithParser(ICommandLineParser parser)

// Build the UI instance
public InteractiveUI Build()
```

### ParserExtensions Class

Extension methods for CommandLineParser's `Parser` class.

```csharp
public static InteractiveUI WithInteractiveUI<T1, T2, T3>(this Parser parser)
    where T1 : class
    where T2 : class
    where T3 : class
```

### ICommandLineParser Interface

Interface for command execution.

```csharp
public interface ICommandLineParser
{
    int Execute(string[] args);
}
```

For detailed API documentation, see [CommandLineParser.InteractiveUI/README.md](src/CommandLineParser.InteractiveUI/README.md)

## How It Works

1. **Metadata Extraction**: Uses reflection to scan assemblies for `[Verb]` and `[Option]` attributes
2. **Dynamic Menu Generation**: Builds interactive menus from extracted metadata
3. **Type-Aware Input**: Prompts users with appropriate inputs based on option types:
   - Boolean options: Yes/No menu
   - String/numeric options: Text input with validation
   - Required options: Enforced validation
4. **Command Construction**: Assembles user inputs into argument arrays
5. **Execution**: Passes arguments to your `ICommandLineParser` for execution

## Interactive UI Features

- 🎨 **Colored Headers** - Cyan headers with command names and descriptions
- 🔢 **Numbered Menus** - Easy numeric selection
- 🤖 **Smart Prompts** - Context-aware input based on types
- ✅ **Validation** - Required field enforcement with clear error messages
- 💡 **Default Value Hints** - Shows defaults for optional parameters
- 📋 **Execution Summary** - Reviews complete command before running
- ✔️ **Confirmation Prompts** - Optional confirmation before execution
- 🎯 **Color-Coded Results** - Success (green) and error (red) reporting
- 🛡️ **Safe Console Handling** - Graceful degradation when console features unavailable

## Dual-Mode Application Pattern

Build applications that support both traditional CLI and interactive modes:

```csharp
class Program
{
    static int Main(string[] args)
    {
        var executor = new MyCommandExecutor();

        // CLI mode: execute immediately with provided arguments
        if (args.Length > 0)
        {
            Console.WriteLine("Running in CLI mode...");
            return executor.Execute(args);
        }

        // Interactive mode: show menus for user-friendly input
        Console.WriteLine("Launching interactive mode...\n");
        
        var ui = InteractiveUI.CreateFrom<Verb1, Verb2, Verb3>()
            .WithParser(executor)
            .Build();
        
        ui.Run();
        return 0;
    }
}
```

This pattern provides:
- **Automation Support** - Scripts can pass arguments for non-interactive execution
- **User-Friendly Manual Use** - Interactive menus for occasional users
- **Single Codebase** - Same command implementations serve both modes
- **Consistent Behavior** - Both modes use the same CommandLineParser validation

## Best Practices

### 1. Provide Clear Help Text

Make your UI self-documenting:

```csharp
[Verb("process", HelpText = "Process files with advanced options.")]
public class ProcessOptions
{
    [Option('i', "input", Required = true, 
            HelpText = "Input file path to process.")]
    public string Input { get; set; } = "";
}
```

### 2. Use Sensible Defaults

Improve user experience with defaults:

```csharp
[Option('t', "timeout", Required = false, Default = 30, 
        HelpText = "Timeout in seconds (default: 30).")]
public int Timeout { get; set; } = 30;
```

### 3. Implement Error Handling

Handle errors gracefully in your executor:

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

## Architecture Benefits

### Library Benefits
- ✅ **Reusable** - Use in any project that uses CommandLineParser
- ✅ **Zero Code Generation** - Pure runtime reflection
- ✅ **Non-Invasive** - Extends existing CommandLineParser usage
- ✅ **Type-Safe** - Leverages CommandLineParser's type system
- ✅ **Testable** - Clean separation of concerns

### Sample Application Benefits
- ✅ **Clean Architecture** - Proper separation of concerns
- ✅ **Compile-Time Safety** - Dependencies enforced by project references
- ✅ **Modular** - Commands and options in separate projects
- ✅ **Extensible** - Easy to add new commands

## Requirements

- .NET 9.0 or higher
- CommandLineParser library (NuGet package)

## Example Use Cases

- **DevOps Tools** - Build tools that work in CI/CD (CLI) and manual use (Interactive)
- **File Management** - Create file operation tools with guided workflows
- **Database Tools** - Database migration/query tools with safe interactive prompts
- **Build Utilities** - Build systems with both automated and manual modes
- **Admin Tools** - System administration tools with guided wizards

## Contributing

Contributions are welcome! When contributing:

1. Work on the appropriate solution:
   - Library changes → `InteractiveUI.sln`
   - Sample/demo changes → `InteractiveUI.Samples.sln`

2. Ensure code follows existing patterns
3. Update documentation as needed
4. Test both CLI and interactive modes

## License

See [License.md](License.md) for license information.

## Documentation

- [STRUCTURE.md](STRUCTURE.md) - Detailed repository structure
- [CommandLineParser.InteractiveUI/README.md](src/CommandLineParser.InteractiveUI/README.md) - Library API documentation

## Related Projects

- [CommandLineParser](https://github.com/commandlineparser/commandline) - The excellent CLI parsing library this extends
