# Solution Structure Diagram

This repository contains two separate solutions:

1. **InteractiveUI.sln** - The core library for extending CommandLineParser with interactive UI capabilities
2. **InteractiveUI.Samples.sln** - Sample applications demonstrating the library in action

## Solution 1: InteractiveUI.sln (Library)

```
InteractiveUI/
│
├── 📄 InteractiveUI.sln                        # Library solution file
├── 📄 README.md                                 # Main documentation
├── 📄 STRUCTURE.md                              # This file
├── 📄 License.md                                # License information
│
└── 📁 src/
    │
    └── 📁 CommandLineParser.InteractiveUI/      # ⭐ Core Library
```

## Solution 2: InteractiveUI.Samples.sln (Samples)

```
InteractiveUI.Samples/
│
├── 📄 InteractiveUI.Samples.sln                # Samples solution file
│
└── 📁 src/
    │
    ├── 📁 InteractiveUI.Sample.Options/        # 📋 Command Option Definitions
    │   ├── 📄 ListOptions.cs
    │   ├── 📄 SearchOptions.cs
    │   ├── 📄 CountOptions.cs
    │   └── 📄 InteractiveUI.Sample.Options.csproj
    │       └── 📦 → CommandLineParser (NuGet v2.9.1)
    │
    ├── 📁 InteractiveUI.Sample.Commands/       # 🎯 Command Implementations
    │   ├── 📄 ListCommand.cs
    │   ├── 📄 SearchCommand.cs
    │   ├── � CountCommand.cs
    │   └── � InteractiveUI.Sample.Commands.csproj
    │       └── 📦 → InteractiveUI.Sample.Options
    │
    └── 📁 InteractiveUI.Sample.Demo/           # 🚀 Demo Console Application
        ├── 📄 Program.cs
        ├── 📄 DemoCommandExecutor.cs
        └── 📄 InteractiveUI.Sample.Demo.csproj
            ├── 📦 → CommandLineParser.InteractiveUI
            ├── 📦 → InteractiveUI.Sample.Options
            ├── 📦 → InteractiveUI.Sample.Commands
            └── 📦 → CommandLineParser (NuGet v2.9.1)
```

## Dependency Flow

### Library (InteractiveUI.sln)
```
┌───────────────────────────────────┐
│  CommandLineParser.InteractiveUI  │
│       (Core Library)               │
│                                    │
│  • InteractiveUI                   │
│  • InteractiveUIBuilder            │
│  • ParserExtensions                │
│  • Metadata Extraction             │
│  • Command Execution Infrastructure│
└───────────────────────────────────┘
           │
           ▼
┌───────────────────────────────────┐
│   CommandLineParser (NuGet)       │
└───────────────────────────────────┘
```

### Samples (InteractiveUI.Samples.sln)
```
           ┌─────────────────────┐
           │  Sample.Demo        │
           │  (Console App)      │
           └──────────┬──────────┘
                      │
         ┌────────────┴────────────┐
         │                         │
         ▼                         ▼
┌─────────────────┐    ┌──────────────────────┐
│  Sample.Commands│    │  InteractiveUI       │
│ (Business Logic)│    │  (Library)           │
└────────┬────────┘    └──────────────────────┘
         │
         ▼
┌─────────────────┐
│  Sample.Options │
│  (Data Models)  │
└─────────────────┘
```

## Build Order

### Library Solution
1. ⭐ **CommandLineParser.InteractiveUI** (depends on CommandLineParser NuGet)

### Samples Solution
1. 📋 **InteractiveUI.Sample.Options** (depends on CommandLineParser)
2. 🎯 **InteractiveUI.Sample.Commands** (depends on Sample.Options)
3. � **InteractiveUI.Sample.Demo** (depends on InteractiveUI library + Sample.Commands + Sample.Options)

## Architecture Patterns

### Clean Separation
```
┌────────────────────────────────────────┐
│         Library Layer                  │
│  (CommandLineParser.InteractiveUI)     │
│  • Extension methods                   │
│  • Interactive UI builder              │
│  • Metadata reflection                 │
│  • Command execution infrastructure    │
└────────────────────────────────────────┘
                 ▲
                 │
┌────────────────┴───────────────────────┐
│         Sample Application             │
│  ┌──────────────────────────────────┐  │
│  │  Demo (Entry Point)             │  │
│  └────────────┬─────────────────────┘  │
│               │                         │
│  ┌────────────▼─────────────────────┐  │
│  │  Commands (Business Logic)       │  │
│  └────────────┬─────────────────────┘  │
│               │                         │
│  ┌────────────▼─────────────────────┐  │
│  │  Options (Data Models)           │  │
│  └──────────────────────────────────┘  │
└────────────────────────────────────────┘
```

## Key Points

✅ **2 Solutions** - Library and samples completely separated  
✅ **Reusable Library** - CommandLineParser.InteractiveUI can be used in any project  
✅ **Clean Dependencies** - Enforced at compile time  
✅ **No Circular References** - Clean dependency graph  
✅ **Testable** - Each component can be tested independently  
✅ **Extensible** - Easy to add new commands or create new applications using the library  
✅ **Fluent API** - Builder pattern for easy configuration  
✅ **Dual Mode** - Supports both traditional CLI and interactive UI modes

## Usage Patterns

### As a Library Consumer
```csharp
// Reference CommandLineParser.InteractiveUI in your project
using CommandLineParser.InteractiveUI;

// Use the fluent API to create an interactive UI
var ui = InteractiveUI
    .CreateFrom<YourVerb1, YourVerb2, YourVerb3>()
    .WithParser(yourExecutor)
    .Build();

ui.Run();
```

### For Development
- Work on **InteractiveUI.sln** to modify the library
- Work on **InteractiveUI.Samples.sln** to test or create sample applications
- Both solutions are independent but share the same source directory structure
