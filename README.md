# WPN - Notes

A lightweight, fast note-taking desktop application for Windows built with C# .NET 8 and Avalonia UI.

## Features

- ✨ **Clean, Modern UI** - Minimal and distraction-free interface
- 📝 **Markdown Support** - Notes are stored as Markdown (.md) files
- 💾 **Auto-Save** - Automatic saving with 1-second debounce
- 🚀 **Fast Startup** - Optimized for quick launch and low memory usage
- 📌 **Always on Top** - Optional toggle to keep the app above other windows
- 📂 **Local Storage** - All notes stored in Documents/WPN Notes folder

## Technology Stack

- **.NET 8** - Latest .NET framework
- **Avalonia UI** - Cross-platform XAML-based UI framework
- **MVVM Pattern** - Clean separation of concerns
- **CommunityToolkit.Mvvm** - Modern MVVM implementation

## Building from Source

### Prerequisites
- .NET 8 SDK

### Build Commands
```bash
# Restore dependencies
dotnet restore WPN/WPN.csproj

# Build Release
dotnet build WPN/WPN.csproj --configuration Release

# Publish for Windows x64
dotnet publish WPN/WPN.csproj --configuration Release --runtime win-x64 --self-contained true --output ./publish/win-x64
```

## CI/CD

The project includes a GitHub Actions workflow that automatically:
- Restores dependencies
- Builds the application in Release mode
- Publishes a win-x64 standalone executable
- Uploads build artifacts

Workflow runs on every push to the `main` branch.

## Usage

1. Launch the application
2. Click "New Note" to create a note
3. Type your content in the editor (supports Markdown)
4. Notes auto-save after 1 second of inactivity
5. All notes are listed in the left panel
6. Click any note to open it
7. Use "Always on Top" to keep the window above others

## License

See LICENSE file for details.

