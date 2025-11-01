# Integrating Penpot MCP Server with LM Studio

This guide walks you through setting up the Penpot MCP Server to work with LM Studio, enabling AI models to interact with your Penpot designs.

> 🐧 **Linux Mint Users:** For a beginner-friendly guide with detailed command explanations specifically for Linux Mint, see **[LMSTUDIO-LINUX-MINT.md](LMSTUDIO-LINUX-MINT.md)** which includes a TLDR quick-start section.

## Table of Contents

- [Overview](#overview)
- [Prerequisites](#prerequisites)
- [Installation Steps](#installation-steps)
- [Configuration](#configuration)
- [Testing the Integration](#testing-the-integration)
- [Usage Examples](#usage-examples)
- [Troubleshooting](#troubleshooting)
- [Advanced Configuration](#advanced-configuration)

---

## Overview

LM Studio supports the Model Context Protocol (MCP), allowing AI models to access external tools and data sources. By integrating the Penpot MCP Server with LM Studio, you can:

- Query Penpot projects and design files
- Search for design components
- Export design elements as images
- Navigate design hierarchies
- Get design metadata and properties

**Integration Method:** The Penpot MCP Server runs as a separate process and communicates with LM Studio via standard input/output (stdio).

---

## Prerequisites

### Required Software

1. **LM Studio** - Version 0.2.9 or later (with MCP support)
   - Download from: https://lmstudio.ai/
   - Ensure MCP features are enabled

2. **.NET 9.0 SDK** or later
   - Download from: https://dotnet.microsoft.com/download/dotnet/9.0
   - Verify installation: `dotnet --version`

3. **Penpot Account**
   - Create an account at: https://penpot.app/
   - Note your email and password

### System Requirements

- **Operating System:** Windows 10/11, macOS 11+, or Linux
- **RAM:** At least 4GB available
- **Storage:** 500MB for .NET SDK and dependencies

---

## Installation Steps

### Step 1: Set Up the F# Penpot MCP Server

1. **Navigate to the project directory:**

```bash
cd /home/linux/RiderProjects/FnPenpotMCP
```

Or wherever you cloned/saved the project.

2. **Create your environment configuration:**

```bash
cp .env.example .env
```

3. **Edit `.env` with your Penpot credentials:**

```bash
nano .env
# or use your preferred text editor
```

Update the following values:

```env
# Penpot API Configuration
PENPOT_API_URL=https://design.penpot.app/api
PENPOT_USERNAME=your-email@example.com
PENPOT_PASSWORD=your-password

# Server Configuration
DEBUG=true
MODE=stdio

# HTTP Server (optional, for image serving)
ENABLE_HTTP_SERVER=true
HTTP_SERVER_HOST=localhost
HTTP_SERVER_PORT=8080
```

4. **Build the project:**

```bash
dotnet build
```

You should see a successful build message.

5. **Test the server runs:**

```bash
dotnet run
```

You should see output like:

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
  - list_projects: List all Penpot projects
  - get_project_files: Get files in a project
  ...
```

Press `Ctrl+C` to stop the test.

### Step 2: Configure LM Studio for MCP

1. **Open LM Studio**

2. **Navigate to Settings:**
   - Click on the gear icon (⚙️) in the top-right corner
   - Select **"Settings"** from the menu

3. **Enable MCP Support:**
   - Look for **"Model Context Protocol"** or **"MCP"** section
   - Enable the **"Enable MCP Servers"** toggle

4. **Add MCP Server Configuration:**

   LM Studio uses a configuration file to define MCP servers. The location depends on your OS:

   - **Windows:** `%APPDATA%\LM Studio\mcp-config.json`
   - **macOS:** `~/Library/Application Support/LM Studio/mcp-config.json`
   - **Linux:** `~/.config/lm-studio/mcp-config.json`

5. **Edit or create the `mcp-config.json` file:**

```json
{
  "mcpServers": {
    "penpot": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "/home/linux/RiderProjects/FnPenpotMCP/FnPenpotMCP.fsproj"
      ],
      "env": {
        "MODE": "stdio",
        "DEBUG": "true"
      },
      "description": "Penpot Design MCP Server - Access and export Penpot designs"
    }
  }
}
```

**Important Notes:**
- Replace `/home/linux/RiderProjects/FnPenpotMCP/FnPenpotMCP.fsproj` with the actual path to your project
- On **Windows**, use backslashes and escape them: `"C:\\Users\\YourName\\FnPenpotMCP\\FnPenpotMCP.fsproj"`
- The server will automatically load credentials from the `.env` file in the project directory

### Alternative: Using Pre-Built Binary

If you've published the application, you can use the executable directly:

```json
{
  "mcpServers": {
    "penpot": {
      "command": "/home/linux/RiderProjects/FnPenpotMCP/bin/Release/net9.0/publish/FnPenpotMCP",
      "args": [],
      "env": {
        "MODE": "stdio",
        "DEBUG": "false"
      },
      "description": "Penpot Design MCP Server"
    }
  }
}
```

To create a published build:

```bash
cd /home/linux/RiderProjects/FnPenpotMCP
dotnet publish -c Release -o ./bin/Release/net9.0/publish
```

### Step 3: Restart LM Studio

1. **Close LM Studio completely**
2. **Restart LM Studio**
3. **Verify MCP Server is loaded:**
   - Look for a notification or indicator that the Penpot MCP server is active
   - Check the MCP settings panel for the "penpot" server status

---

## Configuration

### Environment Variables

Configure the server behavior by editing the `.env` file:

| Variable | Default | Description |
|----------|---------|-------------|
| `PENPOT_API_URL` | `https://design.penpot.app/api` | Penpot API endpoint |
| `PENPOT_USERNAME` | (required) | Your Penpot email |
| `PENPOT_PASSWORD` | (required) | Your Penpot password |
| `DEBUG` | `true` | Enable debug logging |
| `MODE` | `stdio` | Communication mode (keep as stdio) |
| `ENABLE_HTTP_SERVER` | `true` | Enable HTTP server for images |
| `HTTP_SERVER_HOST` | `localhost` | HTTP server host |
| `HTTP_SERVER_PORT` | `8080` | HTTP server port |
| `RESOURCES_AS_TOOLS` | `true` | Expose schema resources as tools |

### Self-Hosted Penpot

If you're using a self-hosted Penpot instance, update the API URL:

```env
PENPOT_API_URL=https://your-penpot-instance.com/api
```

---

## Testing the Integration

### Method 1: Direct Testing in LM Studio

1. **Open LM Studio**
2. **Start a new chat**
3. **Test the connection:**

Type:

```
Can you list my Penpot projects?
```

The AI should use the `list_projects` tool and return your projects.

4. **Test file retrieval:**

```
Show me the files in project [PROJECT_ID]
```

Replace `[PROJECT_ID]` with an actual project ID from the previous response.

### Method 2: Verify Server Logs

If you set `DEBUG=true`, check the server logs for activity:

- The server logs to stdout, which LM Studio captures
- Look for log entries showing API calls and responses
- Errors will be clearly marked

### Common Test Commands

Try these prompts in LM Studio:

1. **List all projects:**
   ```
   What Penpot projects do I have access to?
   ```

2. **Get project files:**
   ```
   Show me all files in my [project name] project
   ```

3. **Search for a design element:**
   ```
   Find all components named "button" in file [FILE_ID]
   ```

4. **Export a design:**
   ```
   Export the object with ID [OBJECT_ID] from file [FILE_ID] as a PNG
   ```

---

## Usage Examples

### Example 1: Discovering Projects and Files

**Prompt:**
```
What design projects do I have in Penpot?
```

**Expected Response:**
The AI will call the `list_projects` tool and return a list of your projects with their IDs and names.

**Follow-up Prompt:**
```
Show me the files in the "Mobile App Design" project
```

The AI will use the project ID to fetch files.

### Example 2: Searching for Design Components

**Prompt:**
```
I need to find all "header" components in my design file [FILE_ID]
```

**Expected Response:**
The AI will use the `search_object` tool with a regex pattern to find matching objects.

### Example 3: Exporting Design Elements

**Prompt:**
```
Export the navigation bar from page [PAGE_ID] in file [FILE_ID] as PNG
```

**Expected Response:**
The AI will:
1. Use `get_file` to fetch the file data
2. Use `export_object` to generate the PNG
3. Provide a URL to view the image (if HTTP server is enabled)

### Example 4: Understanding Design Structure

**Prompt:**
```
Show me the component hierarchy for object [OBJECT_ID] in file [FILE_ID]
```

**Expected Response:**
The AI will use `get_object_tree` to return the nested structure with a visual representation.

---

## Troubleshooting

### Issue 1: MCP Server Not Starting

**Symptoms:**
- LM Studio shows "MCP server failed to start"
- No response to Penpot-related queries

**Solutions:**

1. **Check .NET installation:**
   ```bash
   dotnet --version
   ```
   Should show version 9.0 or later.

2. **Verify project path in `mcp-config.json`:**
   - Ensure the path is absolute and correct
   - Check for typos in the path

3. **Test the server manually:**
   ```bash
   cd /home/linux/RiderProjects/FnPenpotMCP
   dotnet run
   ```
   Should start without errors.

4. **Check LM Studio logs:**
   - Look for error messages in LM Studio's console/logs
   - On macOS/Linux: Check `~/.lmstudio/logs/`

### Issue 2: Authentication Failures

**Symptoms:**
- Tools return "authentication failed" errors
- CloudFlare protection messages

**Solutions:**

1. **Verify credentials in `.env`:**
   - Ensure `PENPOT_USERNAME` and `PENPOT_PASSWORD` are correct
   - Check for extra spaces or quotes

2. **Handle CloudFlare protection:**
   - Open https://design.penpot.app in your browser
   - Log in to your account
   - Complete any verification challenges
   - Try again in LM Studio (verification persists for a while)

3. **Check API URL:**
   - Ensure `PENPOT_API_URL` ends with `/api`
   - For self-hosted: verify the URL is accessible

### Issue 3: Tools Not Appearing

**Symptoms:**
- LM Studio doesn't show Penpot tools
- AI doesn't recognize Penpot commands

**Solutions:**

1. **Restart LM Studio completely:**
   - Quit the application
   - Start it again
   - Wait for MCP servers to initialize

2. **Check MCP configuration:**
   - Verify `mcp-config.json` syntax is valid (use a JSON validator)
   - Ensure the "penpot" server is listed

3. **Enable MCP in LM Studio settings:**
   - Go to Settings → MCP
   - Toggle "Enable MCP Servers" on

### Issue 4: Image Export Failures

**Symptoms:**
- Export commands fail
- Images are not generated

**Solutions:**

1. **Enable HTTP server in `.env`:**
   ```env
   ENABLE_HTTP_SERVER=true
   HTTP_SERVER_PORT=8080
   ```

2. **Check port availability:**
   - Ensure port 8080 (or your chosen port) is not in use
   - Change to a different port if needed

3. **Verify object IDs are correct:**
   - Use `search_object` first to find valid object IDs
   - Ensure you're using the right file and page IDs

### Issue 5: Slow Response Times

**Symptoms:**
- Tools take a long time to respond
- Timeouts occur

**Solutions:**

1. **Use caching effectively:**
   - The server caches file data automatically
   - Subsequent requests for the same file will be faster

2. **Reduce debug output:**
   ```env
   DEBUG=false
   ```

3. **Check network connectivity:**
   - Ensure stable internet connection
   - Self-hosted Penpot: verify server performance

### Getting Help

If you encounter issues not covered here:

1. **Check the main README:**
   - See `/home/linux/RiderProjects/FnPenpotMCP/README.md`

2. **Review SESSION-LOG.md:**
   - Technical details about the implementation

3. **Enable debug mode:**
   ```env
   DEBUG=true
   ```
   And review the output for error messages.

4. **Test the server standalone:**
   ```bash
   dotnet run
   ```
   Ensure it starts without errors before troubleshooting LM Studio integration.

---

## Advanced Configuration

### Using Custom Cache TTL

Adjust cache expiration by modifying the code (or extending the configuration):

Default cache TTL is 10 minutes (600 seconds). To change this, you would need to modify `Cache.fs`:

```fsharp
// In Cache.fs
let createCache() = MemoryCache(3600)  // 1 hour instead of 10 minutes
```

### Running Multiple MCP Servers

LM Studio supports multiple MCP servers. Add more entries to `mcp-config.json`:

```json
{
  "mcpServers": {
    "penpot": {
      "command": "dotnet",
      "args": ["run", "--project", "/path/to/FnPenpotMCP/FnPenpotMCP.fsproj"],
      "description": "Penpot Design Server"
    },
    "other-mcp-server": {
      "command": "/path/to/other-server",
      "args": [],
      "description": "Another MCP Server"
    }
  }
}
```

### Disabling HTTP Server

If you don't need image URLs (only binary image data):

```env
ENABLE_HTTP_SERVER=false
```

This reduces resource usage but limits image sharing capabilities.

### Custom Resource Schemas

The server includes two schema resources:
- `penpot-schema.json` - Penpot API schema
- `penpot-tree-schema.json` - Object tree schema

To customize these, edit the files in the `resources/` directory.

---

## Security Considerations

### Protecting Credentials

1. **Never commit `.env` to version control:**
   - The `.env` file is in `.gitignore` by default
   - Keep credentials secure

2. **Use environment variables (alternative):**
   Instead of `.env`, set system environment variables:
   
   **Linux/macOS:**
   ```bash
   export PENPOT_USERNAME="your-email@example.com"
   export PENPOT_PASSWORD="your-password"
   ```
   
   **Windows (PowerShell):**
   ```powershell
   $env:PENPOT_USERNAME="your-email@example.com"
   $env:PENPOT_PASSWORD="your-password"
   ```

3. **Restrict file permissions:**
   ```bash
   chmod 600 .env
   ```

### Network Security

- The HTTP server (if enabled) binds to `localhost` by default
- Images are only accessible locally unless you change `HTTP_SERVER_HOST`
- For production use, consider authentication on the HTTP server

---

## Performance Tips

1. **Pre-fetch frequently used files:**
   - Ask the AI to "cache file [FILE_ID]" before working with it
   - Subsequent operations will be faster

2. **Use search before export:**
   - Find the exact object IDs you need first
   - Reduces unnecessary API calls

3. **Limit tree depth:**
   - When requesting object trees, specify reasonable depth limits
   - Deep hierarchies take longer to process

4. **Batch operations:**
   - Request multiple files or objects in a single prompt when possible

---

## Next Steps

Now that you have the Penpot MCP Server integrated with LM Studio, you can:

1. **Explore your designs:**
   - Ask the AI to help you understand your design structure
   - Search for specific components across files

2. **Automate exports:**
   - Generate design assets programmatically
   - Export multiple components in batch

3. **Design analysis:**
   - Ask questions about your design hierarchy
   - Get insights into component organization

4. **Integrate with workflows:**
   - Use design data in other contexts
   - Build automation around your Penpot assets

---

## Additional Resources

- **LM Studio Documentation:** https://lmstudio.ai/docs
- **Model Context Protocol Spec:** https://spec.modelcontextprotocol.io/
- **Penpot Documentation:** https://help.penpot.app/
- **Project README:** [README.md](README.md)
- **Project Overview:** [PROJECT-OVERVIEW.md](PROJECT-OVERVIEW.md)

---

**Last Updated:** November 1, 2025  
**Version:** 1.0  
**Compatible with:** LM Studio 0.2.9+, .NET 9.0+
