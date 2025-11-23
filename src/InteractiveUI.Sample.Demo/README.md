# Reflective CLI

A .NET C# command-line application demonstrating clean architecture principles with CommandLineParser NuGet package.

## Architecture Overview

This project showcases **separation of concerns** and **dependency inversion** by abstracting the CommandLineParser library away from the core application logic.

### Project Structure

```
ReflectiveCLI/
├── Program.cs                              # Entry point - depends only on abstractions
├── ICommandLineParser.cs                   # Abstraction for CLI parsing
├── ICommand.cs                             # Command execution abstraction
├── Commands/
│   ├── ListCommand.cs                      # List files command implementation
│   ├── SearchCommand.cs                    # Search text in files command
│   └── CountCommand.cs                     # Count files by extension command
└── Infrastructure/
    └── CommandLineParserAdapter.cs         # CommandLineParser library adapter
```

### Design Benefits

1. **Loose Coupling**: The core application logic (`Program.cs` and commands) has no direct dependency on CommandLineParser
2. **Testability**: Commands can be easily unit tested without the parsing framework
3. **Flexibility**: The parsing library can be swapped for another implementation (e.g., System.CommandLine, custom parser) by simply creating a new adapter
4. **Single Responsibility**: Each command class handles only its specific functionality

## Available Commands

### List Command
Lists files in a directory with optional filtering and detailed information.

```bash
dotnet run -- list [OPTIONS]

Options:
  -d, --directory    Directory to list files from (default: current directory)
  -r, --recursive    Search recursively in subdirectories
  -p, --pattern      File pattern to match (e.g., *.txt)
  -v, --verbose      Show detailed information (size, date)
```

**Examples:**
```bash
# List all files in current directory
dotnet run -- list

# List C# files recursively with details
dotnet run -- list -r -p *.cs -v

# List text files in specific directory
dotnet run -- list -d "C:\MyFolder" -p *.txt
```

### Search Command
Searches for text within files.

```bash
dotnet run -- search [OPTIONS]

Options:
  -t, --text          Text to search for (required)
  -d, --directory     Directory to search in (default: current directory)
  -p, --pattern       File pattern to search (e.g., *.cs)
  -c, --case-sensitive Perform case-sensitive search
```

**Examples:**
```bash
# Search for "TODO" in all files
dotnet run -- search -t "TODO"

# Case-sensitive search in C# files
dotnet run -- search -t "className" -p *.cs -c

# Search in specific directory
dotnet run -- search -t "error" -d "C:\Logs" -p *.log
```

### Count Command
Counts and displays files grouped by extension.

```bash
dotnet run -- count [OPTIONS]

Options:
  -d, --directory    Directory to analyze (default: current directory)
  -n, --top          Number of top extensions to show (default: 10)
```

**Examples:**
```bash
# Count files in current directory
dotnet run -- count

# Show top 5 extensions in specific directory
dotnet run -- count -d "C:\Projects" -n 5
```

## Building and Running

### Prerequisites
- .NET 9.0 SDK or later

### Build
```bash
dotnet build
```

### Run
```bash
dotnet run -- [command] [options]
```

### Publish (Create executable)
```bash
dotnet publish -c Release -o publish
```

Then run the executable directly:
```bash
.\publish\ReflectiveCLI.exe list -v
```

## Code Architecture Details

### Abstraction Layer

**ICommandLineParser** - Defines the contract for parsing command-line arguments:
```csharp
public interface ICommandLineParser
{
    int Parse(string[] args);
}
```

**ICommand** - Defines the contract for command execution:
```csharp
public interface ICommand
{
    int Execute();
}
```

### Infrastructure Layer

**CommandLineParserAdapter** - Adapts the CommandLineParser library to our abstractions:
- Contains library-specific attributes (`[Verb]`, `[Option]`)
- Translates parsed options into domain command objects
- Isolates the third-party dependency

### Benefits of This Architecture

1. **Testing**: Commands can be instantiated and tested directly without parsing
   ```csharp
   var command = new ListCommand { Directory = ".", Verbose = true };
   var result = command.Execute();
   ```

2. **Library Independence**: Switching to a different CLI parsing library only requires creating a new adapter

3. **Domain Clarity**: Command classes represent pure business logic without parsing concerns

## License

This is a demonstration project for educational purposes.
