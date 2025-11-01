# Penpot MCP Server - Project Overview 🎨🤖

## What is This Project?

**Penpot MCP Server (F# Edition)** is a bridge that connects AI assistants (like Claude) with [Penpot](https://penpot.app/), the open-source design and prototyping platform. It allows AI models to interact with your Penpot designs, browse projects, search for design elements, and even export visual assets—all through natural conversation.

Think of it as giving your AI assistant "eyes" to see and understand your design files, and "hands" to retrieve and manipulate them.

---

## Why Does This Exist?

### The Problem
Designers and developers often need to:
- Browse through multiple design files to find specific components
- Extract design specifications and dimensions
- Export assets in various formats
- Document design systems and component hierarchies
- Search for design elements by name across large projects

Doing this manually through the Penpot UI can be time-consuming, especially when working with large design systems.

### The Solution
This MCP (Model Context Protocol) server enables AI assistants to perform these tasks automatically through conversation. You can ask your AI assistant to:
- "Show me all button components in the design system"
- "Export the login screen as PNG"
- "Find all frames named 'Hero Section'"
- "What are the dimensions of this component?"

The AI handles the technical details, and you get the results instantly.

---

## What Can It Do?

### Core Capabilities

#### 🗂️ **Project Management**
- List all your Penpot projects
- Browse files within projects
- Access file metadata and structure

#### 🔍 **Search & Discovery**
- Search for design objects by name (supports regex patterns)
- Find components across multiple pages
- Navigate complex design hierarchies

#### 📸 **Asset Export**
- Export designs as PNG or SVG images
- Retrieve specific components or entire pages
- Generate screenshots of design elements

#### 🌲 **Tree Navigation**
- Explore design object hierarchies
- Extract subtrees at any depth
- Select specific fields from design objects
- Understand parent-child relationships in designs

#### 💾 **Performance Features**
- Smart caching to reduce API calls
- Optional HTTP server for serving exported images
- Fast repeated access to frequently used files

---

## How It Works

### Architecture Overview

```
┌─────────────┐
│   AI Agent  │  (e.g., Claude)
│  (via MCP)  │
└──────┬──────┘
       │
       ▼
┌─────────────────────────────┐
│  Penpot MCP Server (F#)     │
│  ┌─────────────────────┐    │
│  │  API Integration    │    │
│  ├─────────────────────┤    │
│  │  Caching Layer      │    │
│  ├─────────────────────┤    │
│  │  Tree Navigator     │    │
│  ├─────────────────────┤    │
│  │  Image Exporter     │    │
│  └─────────────────────┘    │
└──────────┬──────────────────┘
           │
           ▼
    ┌──────────────┐
    │   Penpot     │
    │  design.penpot.app │
    └──────────────┘
```

### Component Breakdown

1. **Configuration Module**: Manages credentials and settings
2. **API Client**: Communicates with Penpot's REST API
3. **Cache System**: Stores frequently accessed data temporarily
4. **Tree Navigator**: Parses and traverses design object hierarchies
5. **HTTP Server**: (Optional) Serves exported images via URLs
6. **MCP Tools**: Exposes 11 different tools for AI interaction

---

## Available Tools

The server exposes 11 tools that AI assistants can use:

### Essential Tools

| Tool | Purpose | Example Use |
|------|---------|-------------|
| `list_projects` | Get all your Penpot projects | "What projects do I have?" |
| `get_project_files` | List files in a project | "Show files in my Design System project" |
| `get_file` | Retrieve complete file data | "Load the mobile-app.penpot file" |
| `search_object` | Find objects by name | "Find all components with 'button' in the name" |
| `export_object` | Export as PNG/SVG | "Export the header component as PNG" |
| `get_object_tree` | Get structure + screenshot | "Show me the structure of this frame" |

### Utility Tools

| Tool | Purpose |
|------|---------|
| `get_rendered_component` | Retrieve previously rendered images |
| `get_cached_files` | See what's in cache |
| `get_server_info` | Check server status |
| `get_penpot_schema` | Get API schema documentation |
| `get_penpot_tree_schema` | Get tree structure schema |

---

## Quick Start

### Prerequisites

Before you begin, make sure you have:

- ✅ **[.NET 9.0 SDK](https://dotnet.microsoft.com/download)** or later installed
- ✅ A **[Penpot account](https://penpot.app/)** (free or self-hosted)
- ✅ Your **Penpot credentials** (email and password)
- ✅ An **AI assistant** that supports MCP (like Claude Desktop)

### Installation Steps

#### 1️⃣ Get the Project
```bash
cd /home/linux/RiderProjects/FnPenpotMCP
```

#### 2️⃣ Configure Your Credentials
```bash
# Copy the example configuration
cp .env.example .env

# Edit .env with your credentials
nano .env
```

Set these values in `.env`:
```bash
PENPOT_USERNAME=your-email@example.com
PENPOT_PASSWORD=your-password
PENPOT_API_URL=https://design.penpot.app/api
```

#### 3️⃣ Build the Project
```bash
dotnet build
```

You should see:
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

#### 4️⃣ Run the Server
```bash
dotnet run
```

You'll see:
```
===================================
Penpot MCP Server (F# Edition)
===================================
Mode: stdio
Debug: true
API URL: https://design.penpot.app/api
===================================

Server initialized successfully!

Available Tools:
  - list_projects
  - get_project_files
  - get_file
  ...
```

The server is now ready to accept commands from your AI assistant!

---

## Usage Examples

### Example 1: Browse Your Projects
**You ask Claude:** "What Penpot projects do I have?"

**Behind the scenes:** Claude calls `list_projects` tool

**You get:** A list of all your projects with names, IDs, and metadata

---

### Example 2: Find a Component
**You ask Claude:** "Find all button components in my design system"

**Behind the scenes:** 
1. Claude calls `get_project_files` to find the design system file
2. Calls `get_file` to load the file data
3. Calls `search_object` with regex pattern "button"

**You get:** A list of all matching components with their IDs, names, and locations

---

### Example 3: Export a Design
**You ask Claude:** "Export the login screen as PNG"

**Behind the scenes:**
1. Claude calls `search_object` to find "login screen"
2. Calls `export_object` with the found object ID
3. Returns the PNG image

**You get:** The exported PNG, either as a data URL or via the HTTP server

---

## Configuration Options

All configuration is done through environment variables in the `.env` file:

### Required Settings
```bash
PENPOT_USERNAME=your-email@example.com    # Your Penpot account email
PENPOT_PASSWORD=your-secure-password      # Your Penpot password
```

### Optional Settings
```bash
# API Configuration
PENPOT_API_URL=https://design.penpot.app/api  # Default: penpot.app
DEBUG=true                                     # Default: true

# HTTP Server (for serving images)
ENABLE_HTTP_SERVER=true                        # Default: true
HTTP_SERVER_HOST=localhost                     # Default: localhost
HTTP_SERVER_PORT=0                             # Default: 0 (random port)

# Server Mode
MODE=stdio                                     # Default: stdio
PORT=5000                                      # For SSE mode only

# Advanced
RESOURCES_AS_TOOLS=true                        # Expose schemas as tools
```

---

## Advanced Features

### Caching System
The server automatically caches file data to improve performance:
- **Cache Duration**: 10 minutes (600 seconds) by default
- **Cache Type**: In-memory (cleared on restart)
- **What's Cached**: File data from `get_file` calls
- **Why It Helps**: Reduces API calls, speeds up repeated operations

### HTTP Image Server
When enabled, the server runs a local HTTP server to serve exported images:
- **Purpose**: Makes images accessible via URLs instead of data URIs
- **Port**: Automatically assigned (or configurable)
- **Use Case**: Useful for AI assistants that prefer URL-based images
- **Default**: Enabled

### Debug Mode
Enable debug logging to see detailed API interactions:
```bash
DEBUG=true
```

Shows:
- API request URLs
- Response status codes
- Cache hits/misses
- Server operations

---

## Why F#?

This project is written in F#, a functional-first programming language. Here's why:

### Benefits for Users
- **Reliability**: Strong type system catches errors at compile time
- **Performance**: Native .NET performance
- **Stability**: Immutable data structures prevent common bugs
- **Safety**: Null-safe by design using Option types

### Benefits for Developers
- **Maintainability**: Clear module structure and function signatures
- **Testability**: Pure functions are easy to test
- **Expressiveness**: Pattern matching and function composition
- **Async Support**: First-class async workflows for I/O operations

### Comparison to Python Version
This is a conversion from the [original Python implementation](https://github.com/IvanTheGeek/penpot-mcp). Both versions provide the same functionality, but the F# version offers:
- ✅ Compile-time type safety
- ✅ Better performance for large operations
- ✅ Explicit error handling
- ✅ Native .NET integration

---

## Troubleshooting

### CloudFlare Protection Error

**Problem:** You see an error about CloudFlare protection blocking requests.

**Solution:**
1. Open your web browser
2. Go to https://design.penpot.app
3. Log in to your Penpot account
4. Complete any human verification challenges
5. Try your request again

The verification typically lasts for several hours.

---

### Build Errors

**Problem:** `dotnet build` fails

**Solution:**
- Ensure you have .NET 9.0 SDK installed: `dotnet --version`
- Try cleaning the project: `dotnet clean && dotnet build`
- Check for missing dependencies: `dotnet restore`

---

### Authentication Errors

**Problem:** Server can't authenticate with Penpot

**Solution:**
- Verify your credentials in `.env`
- Make sure there are no extra spaces in the values
- Test login manually at https://design.penpot.app
- Check if your password has special characters that need escaping

---

### Connection Errors

**Problem:** Can't connect to Penpot API

**Solution:**
- Check your internet connection
- Verify the API URL is correct
- If using self-hosted Penpot, ensure it's accessible
- Check firewall settings

---

## Use Cases

### For Designers
- **Component Documentation**: Automatically generate component lists
- **Asset Export**: Bulk export design elements
- **Design Search**: Find components across multiple files
- **Spec Generation**: Extract dimensions and properties

### For Developers
- **Design Handoff**: Get precise measurements and specs via chat
- **Asset Pipeline**: Automate asset export for builds
- **Design Tokens**: Extract color and spacing values
- **Documentation**: Auto-generate design system docs

### For Teams
- **Design Reviews**: AI-assisted design critiques
- **Consistency Checks**: Find inconsistent naming or structures
- **Onboarding**: Help new team members explore design systems
- **Migration**: Assist in moving designs between files

---

## Project Structure

```
FnPenpotMCP/
├── Config.fs              # Configuration management
├── Cache.fs               # Caching system
├── HttpServer.fs          # Image HTTP server
├── PenpotApi.fs           # Penpot API client
├── PenpotTree.fs          # Tree navigation
├── McpServer.fs           # MCP tool handlers
├── Program.fs             # Application entry point
├── resources/
│   ├── penpot-schema.json      # API documentation
│   └── penpot-tree-schema.json # Tree structure docs
├── README.md              # Technical documentation
├── PROJECT-OVERVIEW.md    # This file
├── SESSION-LOG.md         # Development history
├── .env.example           # Configuration template
└── FnPenpotMCP.fsproj     # Project file
```

---

## Frequently Asked Questions

### Q: Do I need to know F# to use this?
**A:** No! You only need to configure the `.env` file and run the server. The AI assistant handles all interactions.

### Q: Can I use this with my own Penpot instance?
**A:** Yes! Just change the `PENPOT_API_URL` in your `.env` file to point to your self-hosted instance.

### Q: Is my password secure?
**A:** The password is stored locally in `.env` and never leaves your machine. Make sure to keep `.env` private and never commit it to version control.

### Q: What AI assistants support this?
**A:** Any AI assistant that supports the Model Context Protocol (MCP), such as Claude Desktop. Check your AI assistant's documentation for MCP support.

### Q: Does this work with Figma/Sketch/Adobe XD?
**A:** No, this server is specifically designed for Penpot. Each design tool has different APIs and would require a separate implementation.

### Q: Can I extend this with custom tools?
**A:** Yes! The codebase is modular and well-documented. You can add new tools in `McpServer.fs` and expose them through the MCP interface.

### Q: Is this production-ready?
**A:** This is a functional implementation suitable for personal use and small teams. For production use, consider adding:
- Proper logging and monitoring
- Error recovery and retry logic
- Rate limiting
- Security hardening
- Unit and integration tests

---

## Getting Help

### Documentation
- **README.md**: Technical documentation and API details
- **SESSION-LOG.md**: Development history and implementation notes
- **This file (PROJECT-OVERVIEW.md)**: User guide and concepts

### Resources
- [Penpot Documentation](https://help.penpot.app/)
- [Model Context Protocol Spec](https://spec.modelcontextprotocol.io/)
- [F# Documentation](https://docs.microsoft.com/en-us/dotnet/fsharp/)
- [Original Python Implementation](https://github.com/IvanTheGeek/penpot-mcp)

### Issues
If you encounter problems:
1. Check the Troubleshooting section above
2. Review debug logs (set `DEBUG=true`)
3. Verify your configuration in `.env`
4. Check the SESSION-LOG.md for implementation details

---

## Integration with AI Tools

Ready to connect this server to your AI assistant? Choose your platform:

### 🖥️ LM Studio
- **[General LM Studio Guide](LMSTUDIO-INTEGRATION.md)** - Works on Windows, macOS, and Linux
- **[Linux Mint Specific Guide](LMSTUDIO-LINUX-MINT.md)** - Beginner-friendly guide for Linux Mint users with step-by-step explanations

### 🛠️ JetBrains Rider
- **[Rider/Junie Integration Guide](RIDER-JUNIE-INTEGRATION.md)** - Connect to JetBrains AI Assistant (Junie)

### 📚 Documentation
- **[README.md](README.md)** - Technical documentation
- **[SESSION-LOG.md](SESSION-LOG.md)** - Development history

---

## Next Steps

Now that you understand what this project is, here's what to do next:

1. ✅ **Choose your platform** - Pick LM Studio or JetBrains Rider
2. ✅ **Follow the integration guide** - See links above
3. ✅ **Set up** your environment (see Quick Start or integration guides)
4. ✅ **Configure** your `.env` file with Penpot credentials
5. ✅ **Build** the project with `dotnet build`
6. ✅ **Run** the server with `dotnet run`
7. ✅ **Connect** your AI assistant using MCP configuration
8. ✅ **Try** asking your AI to list your Penpot projects

---

## Contributing

This project is open for contributions! Whether you want to:
- Add new tools or features
- Improve documentation
- Fix bugs
- Optimize performance
- Add tests

Your contributions are welcome!

---

## License

This project is licensed under the MIT License. See the LICENSE file for details.

---

## Acknowledgments

- **Original Creator**: [IvanTheGeek](https://github.com/IvanTheGeek) - Python implementation
- **Penpot Team**: For creating an amazing open-source design platform
- **Model Context Protocol**: For standardizing AI-application communication
- **F# Community**: For excellent tools and libraries

---

*Last Updated: November 1, 2025*  
*Version: 1.0.0*  
*Project Status: ✅ Stable and Functional*
