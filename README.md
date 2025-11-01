# Penpot MCP Server - F# Edition 🎨🤖

A functional F# implementation of the Penpot Model Context Protocol (MCP) server, enabling AI models like Claude to interact with Penpot designs.

## Overview

This is an idiomatic F# functional programming conversion of the Python [penpot-mcp](https://github.com/IvanTheGeek/penpot-mcp) project. It provides the same functionality using F#'s strong type system, immutable data structures, and functional programming paradigms.

## Features

- ✅ **List Projects & Files**: Browse Penpot projects and design files
- ✅ **File Retrieval**: Fetch complete file data with caching
- ✅ **Object Tree Navigation**: Traverse design hierarchies with field selection
- ✅ **Search**: Find objects by name using regex patterns
- ✅ **Image Export**: Export designs as PNG/SVG
- ✅ **HTTP Server**: Optional HTTP server for serving exported images
- ✅ **Caching**: In-memory TTL-based caching for performance
- ✅ **Error Handling**: Comprehensive error handling including CloudFlare detection

## Architecture

The project is organized into functional modules following F# best practices:

- **Config.fs**: Environment-based configuration using pure functions
- **Cache.fs**: Generic in-memory cache with TTL support
- **HttpServer.fs**: Suave-based HTTP server for image delivery
- **PenpotApi.fs**: HTTP client for Penpot API with async operations
- **PenpotTree.fs**: Functional tree traversal and manipulation
- **McpServer.fs**: MCP tool implementations
- **Program.fs**: Entry point and command-line interface

## Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) or later
- A [Penpot](https://penpot.app/) account
- Penpot API credentials (email and password)

## Installation

### 1. Clone the repository

```bash
git clone <your-repo-url>
cd FnPenpotMCP
```

### 2. Configure environment variables

Copy the example environment file:

```bash
cp .env.example .env
```

Edit `.env` with your Penpot credentials:

```bash
PENPOT_USERNAME=your-email@example.com
PENPOT_PASSWORD=your-password
```

### 3. Restore dependencies

```bash
dotnet restore
```

### 4. Build the project

```bash
dotnet build
```

## Usage

### Running the server

```bash
dotnet run
```

### Command-line options

```bash
dotnet run -- --mode stdio
dotnet run -- --mode sse --port 5000
dotnet run -- --debug
```

### Configuration Options

| Variable | Default | Description |
|----------|---------|-------------|
| `PENPOT_API_URL` | `https://design.penpot.app/api` | Penpot API endpoint |
| `PENPOT_USERNAME` | (required) | Your Penpot email |
| `PENPOT_PASSWORD` | (required) | Your Penpot password |
| `DEBUG` | `true` | Enable debug logging |
| `ENABLE_HTTP_SERVER` | `true` | Enable HTTP server for images |
| `HTTP_SERVER_HOST` | `localhost` | HTTP server host |
| `HTTP_SERVER_PORT` | `0` | HTTP server port (0 = random) |
| `RESOURCES_AS_TOOLS` | `true` | Expose resources as tools |
| `MODE` | `stdio` | MCP mode (stdio or sse) |

## Available Tools

### Project Management
- **list_projects**: Retrieve all available Penpot projects
- **get_project_files**: Get all files in a specific project

### File Operations
- **get_file**: Retrieve a file by ID (with caching)
- **search_object**: Search for objects by name using regex

### Design Export
- **export_object**: Export design objects as images (PNG/SVG)
- **get_object_tree**: Get object tree structure with screenshot

### Resources
- **get_server_info**: Server status and configuration
- **get_cached_files**: List cached files
- **get_rendered_component**: Retrieve rendered components
- **get_penpot_schema**: Penpot API schema
- **get_penpot_tree_schema**: Object tree schema

## Functional Programming Features

This F# implementation showcases:

- **Immutable Data**: All data structures are immutable by default
- **Option Types**: Null-safe operations using `Option<'T>`
- **Result Types**: Explicit error handling with `Result<'T, 'E>`
- **Async Workflows**: Non-blocking I/O with F# async
- **Pattern Matching**: Exhaustive pattern matching for control flow
- **Type Safety**: Strong typing throughout with type inference
- **Function Composition**: Small, composable functions
- **Expression-Based**: Everything is an expression returning a value

## Project Structure

```
FnPenpotMCP/
├── Config.fs              # Configuration management
├── Cache.fs               # TTL-based caching
├── HttpServer.fs          # Image HTTP server (Suave)
├── PenpotApi.fs           # Penpot API client
├── PenpotTree.fs          # Tree traversal logic
├── McpServer.fs           # MCP tool implementations
├── Program.fs             # Entry point
├── resources/
│   ├── penpot-schema.json
│   └── penpot-tree-schema.json
├── .env.example           # Environment template
├── FnPenpotMCP.fsproj     # Project file
└── README.md              # This file
```

## Dependencies

- **FSharp.Data**: HTTP requests and data access
- **FSharp.SystemTextJson**: JSON serialization
- **DotNetEnv**: Environment variable management
- **Suave**: Lightweight web server
- **ModelContextProtocol**: MCP SDK (preview)

## CloudFlare Protection

If you encounter CloudFlare protection errors:

1. Open https://design.penpot.app in your browser
2. Log in to your Penpot account
3. Complete any CloudFlare verification challenges
4. Try your request again

The verification typically lasts for a period of time.

## Development

### Building

```bash
dotnet build
```

### Running tests (when available)

```bash
dotnet test
```

### Publishing

```bash
dotnet publish ./FnPenpotMCP.fsproj -c Release -o ./publish
```

## Differences from Python Version

While maintaining feature parity, this F# version includes:

- **Type Safety**: Compile-time type checking prevents many runtime errors
- **Immutability**: Data structures are immutable by default
- **Pattern Matching**: Cleaner control flow with exhaustive matching
- **Async Workflows**: Native F# async instead of Python asyncio
- **Option Types**: Explicit handling of nullable values
- **Result Types**: Explicit error handling without exceptions where appropriate

## Limitations

- Full MCP protocol integration requires additional implementation with the ModelContextProtocol .NET SDK
- YAML output support is simplified compared to Python version (uses JSON with note)
- Some advanced Python-specific features may differ in implementation

## Contributing

Contributions are welcome! Please feel free to submit issues or pull requests.

## License

This project is licensed under the GNU Affero General Public License v3.0 (AGPL-3.0).

This is a derivative work of the Python project [penpot-mcp](https://github.com/montevive/penpot-mcp), which was originally licensed under the MIT License. Under the permissive terms of the MIT License, this derivative work has been relicensed under AGPL-3.0 to ensure that any further modifications and enhancements remain freely available to all users.

See the LICENSE file for full details.

## Integration Guides

Learn how to integrate this MCP server with different tools:

- **[LM Studio Integration](LMSTUDIO-INTEGRATION.md)** - General guide for integrating with LM Studio on any platform
- **[LM Studio on Linux Mint](LMSTUDIO-LINUX-MINT.md)** - Beginner-friendly guide specifically for Linux Mint users (includes TLDR and detailed explanations)
- **[JetBrains Rider/Junie Integration](RIDER-JUNIE-INTEGRATION.md)** - Guide for using with JetBrains Rider and Junie AI assistant

Also see:
- **[Project Overview](PROJECT-OVERVIEW.md)** - User-friendly introduction to what this project does
- **[Session Log](SESSION-LOG.md)** - Development history and implementation details

## Acknowledgments

- Original Python implementation: [penpot-mcp](https://github.com/montevive/penpot-mcp)
- [Penpot](https://penpot.app/) - The open-source design platform
- [Model Context Protocol](https://modelcontextprotocol.io) - Standardized protocol for AI context

## See Also

- [Penpot Documentation](https://help.penpot.app/)
- [Penpot API Documentation](https://penpot-plugins-api-doc.pages.dev/)
- [F# Documentation](https://docs.microsoft.com/en-us/dotnet/fsharp/)
- [MCP Specification](https://spec.modelcontextprotocol.io/)
