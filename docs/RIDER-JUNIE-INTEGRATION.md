# Integrating Penpot MCP Server with JetBrains Rider and Junie

This guide explains how to integrate the Penpot MCP Server with JetBrains Rider's AI Assistant (Junie), enabling AI-powered interactions with your Penpot designs directly from your IDE.

> 💡 **Looking for other platforms?**
> - **[LM Studio Integration](LMSTUDIO-INTEGRATION.md)** - General guide for LM Studio (all platforms)
> - **[LM Studio on Linux Mint](LMSTUDIO-LINUX-MINT.md)** - Beginner-friendly Linux Mint guide with TLDR
> - **[Project Overview](PROJECT-OVERVIEW.md)** - Learn what this project does

## Table of Contents

- [Overview](#overview)
- [Prerequisites](#prerequisites)
- [Installation Steps](#installation-steps)
- [Configuration](#configuration)
- [Testing the Integration](#testing-the-integration)
- [Using Penpot Tools with Junie](#using-penpot-tools-with-junie)
- [Troubleshooting](#troubleshooting)
- [Best Practices](#best-practices)
- [Advanced Usage](#advanced-usage)

---

## Overview

JetBrains Rider includes Junie, an AI assistant that supports the Model Context Protocol (MCP). By integrating the Penpot MCP Server, you can:

- **Query design files** directly from your IDE
- **Search for components** across Penpot projects
- **Export design assets** without leaving Rider
- **Analyze design structures** while coding
- **Automate design workflows** integrated with development tasks

**Integration Method:** The Penpot MCP Server runs as a local service and communicates with Rider/Junie through the MCP protocol over stdio (standard input/output).

### Benefits

- **Seamless workflow:** Access designs while coding UI components
- **Context-aware assistance:** Junie can reference design specs when helping with code
- **Automated exports:** Generate assets on-demand during development
- **Design-code sync:** Keep design references current while implementing features

---

## Prerequisites

### Required Software

1. **JetBrains Rider** - Version 2024.2 or later (with Junie AI Assistant)
   - Download from: https://www.jetbrains.com/rider/
   - Ensure AI Assistant plugin is enabled
   - License: Professional or Ultimate subscription with AI features

2. **.NET 9.0 SDK** or later
   - Download from: https://dotnet.microsoft.com/download/dotnet/9.0
   - Verify installation: `dotnet --version`

3. **Penpot Account**
   - Create an account at: https://penpot.app/
   - Have your email and password ready

### Rider Configuration

- **AI Assistant enabled:** Go to Settings → Tools → AI Assistant
- **MCP support:** Available in Rider 2024.2+ (check for updates if needed)

### System Requirements

- **Operating System:** Windows 10/11, macOS 11+, or Linux
- **RAM:** At least 8GB (16GB recommended for smooth IDE + AI operation)
- **Storage:** 1GB for .NET SDK and dependencies

---

## Installation Steps

### Step 1: Set Up the Penpot MCP Server

1. **Navigate to the project directory:**

```bash
cd /home/linux/RiderProjects/FnPenpotMCP
```

If you haven't cloned the project yet, the project is already in your Rider workspace.

2. **Create environment configuration:**

```bash
cp .env.example .env
```

3. **Configure Penpot credentials:**

Open `.env` in Rider or your favorite editor:

```env
# Penpot API Configuration
PENPOT_API_URL=https://design.penpot.app/api
PENPOT_USERNAME=your-email@example.com
PENPOT_PASSWORD=your-password

# Server Configuration
DEBUG=true
MODE=stdio

# HTTP Server for image serving
ENABLE_HTTP_SERVER=true
HTTP_SERVER_HOST=localhost
HTTP_SERVER_PORT=8080

# Other settings
RESOURCES_AS_TOOLS=true
```

4. **Build the project:**

In Rider:
- Right-click on `FnPenpotMCP` project
- Select **Build** → **Build 'FnPenpotMCP'**

Or via terminal:

```bash
dotnet build
```

5. **Verify the server runs:**

In Rider terminal or external terminal:

```bash
dotnet run
```

You should see:

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
  ...
```

Press `Ctrl+C` to stop.

### Step 2: Configure Rider for MCP Integration

#### Option A: Using Rider's MCP Configuration UI (Recommended)

1. **Open Rider Settings:**
   - **Windows/Linux:** File → Settings (or `Ctrl+Alt+S`)
   - **macOS:** Rider → Preferences (or `Cmd+,`)

2. **Navigate to AI Assistant:**
   - Go to **Tools → AI Assistant → Model Context Protocol**

3. **Add MCP Server:**
   - Click **"+"** to add a new MCP server
   - Configure:
     - **Name:** `Penpot Design Server`
     - **Command:** `dotnet`
     - **Arguments:** `run --project /home/linux/RiderProjects/FnPenpotMCP/FnPenpotMCP.fsproj`
     - **Working Directory:** `/home/linux/RiderProjects/FnPenpotMCP`
     - **Environment Variables:**
       - `MODE=stdio`
       - `DEBUG=true`

4. **Enable the server:**
   - Check the box next to "Penpot Design Server"
   - Click **Apply** and **OK**

#### Option B: Manual Configuration File

If Rider doesn't have the UI yet, you can configure MCP servers manually:

1. **Locate Rider's configuration directory:**
   - **Windows:** `%APPDATA%\JetBrains\Rider2024.2\`
   - **macOS:** `~/Library/Application Support/JetBrains/Rider2024.2/`
   - **Linux:** `~/.config/JetBrains/Rider2024.2/`

2. **Create or edit `mcp-servers.json`:**

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
      "cwd": "/home/linux/RiderProjects/FnPenpotMCP",
      "env": {
        "MODE": "stdio",
        "DEBUG": "true"
      },
      "enabled": true,
      "description": "Penpot Design MCP Server - Access and export Penpot designs"
    }
  }
}
```

**Important:**
- Replace paths with your actual project location
- On Windows, use double backslashes: `C:\\Users\\YourName\\...`
- Rider version number in path may vary (2024.2, 2024.3, etc.)

3. **Restart Rider:**
   - Close Rider completely
   - Reopen your project

### Step 3: Using Pre-Built Binary (Optional)

For better performance, publish a release build:

1. **Publish the project:**

```bash
cd /home/linux/RiderProjects/FnPenpotMCP
dotnet publish ./FnPenpotMCP.fsproj -c Release -o ./publish
```

2. **Update MCP configuration to use the binary:**

```json
{
  "mcpServers": {
    "penpot": {
      "command": "/home/linux/RiderProjects/FnPenpotMCP/publish/FnPenpotMCP",
      "args": [],
      "cwd": "/home/linux/RiderProjects/FnPenpotMCP",
      "env": {
        "MODE": "stdio",
        "DEBUG": "false"
      },
      "enabled": true,
      "description": "Penpot Design MCP Server"
    }
  }
}
```

This eliminates the `dotnet run` overhead and starts faster.

### Step 4: Verify Integration

1. **Open the Junie panel in Rider:**
   - View → Tool Windows → AI Assistant
   - Or press `Alt+1` (Windows/Linux) / `Cmd+1` (macOS)

2. **Check for MCP server status:**
   - Look for an indicator showing MCP servers are loaded
   - You may see "Penpot Design Server" listed as active

3. **Test basic connectivity:**
   - In the Junie chat, type: `Can you list my Penpot projects?`
   - Junie should use the MCP server to fetch your projects

---

## Configuration

### Environment Variables

All configuration is done via the `.env` file in the project root:

| Variable | Default | Description |
|----------|---------|-------------|
| `PENPOT_API_URL` | `https://design.penpot.app/api` | Penpot API endpoint |
| `PENPOT_USERNAME` | (required) | Your Penpot email address |
| `PENPOT_PASSWORD` | (required) | Your Penpot password |
| `DEBUG` | `true` | Enable debug logging (visible in Rider's output) |
| `MODE` | `stdio` | Communication mode (must be stdio for Rider) |
| `ENABLE_HTTP_SERVER` | `true` | Enable HTTP server for serving exported images |
| `HTTP_SERVER_HOST` | `localhost` | HTTP server bind address |
| `HTTP_SERVER_PORT` | `8080` | HTTP server port |
| `RESOURCES_AS_TOOLS` | `true` | Expose schema resources as MCP tools |

### Self-Hosted Penpot

If you're running your own Penpot instance:

```env
PENPOT_API_URL=https://your-penpot-server.com/api
PENPOT_USERNAME=your-email@example.com
PENPOT_PASSWORD=your-password
```

### Rider-Specific Settings

- **Auto-start MCP servers:** Rider can automatically start MCP servers when opening a project
- **Debug output:** Set `DEBUG=true` to see MCP communication in Rider's console
- **Timeout settings:** Configure in Rider's AI Assistant settings if needed

---

## Testing the Integration

### Basic Connectivity Test

1. **Open Junie in Rider:**
   - View → Tool Windows → AI Assistant

2. **Send a test query:**

```
List my Penpot projects
```

**Expected behavior:**
- Junie should invoke the `list_projects` tool
- You'll see a list of your Penpot projects with IDs and names

### Verify Tool Availability

Ask Junie:

```
What Penpot-related tools do you have access to?
```

Junie should list:
- `list_projects`
- `get_project_files`
- `get_file`
- `export_object`
- `get_object_tree`
- `search_object`
- `get_rendered_component`
- `get_cached_files`
- `get_server_info`
- `get_penpot_schema`
- `get_penpot_tree_schema`

### Debug Logs

To see what's happening under the hood:

1. **Open Rider's Output panel:**
   - View → Tool Windows → Build
   - Or the "Run" panel if the server is running

2. **Look for MCP server logs:**
   - With `DEBUG=true`, you'll see API requests and responses
   - Check for any error messages

### Common Test Scenarios

1. **List projects:**
   ```
   Show me all my Penpot projects
   ```

2. **Get files in a project:**
   ```
   Get all files in project [PROJECT_ID]
   ```

3. **Search for a component:**
   ```
   Find all objects named "button" in file [FILE_ID]
   ```

4. **Export an object:**
   ```
   Export object [OBJECT_ID] from file [FILE_ID] as PNG
   ```

---

## Using Penpot Tools with Junie

### Workflow 1: Exploring Design Structure

**Scenario:** You're implementing a UI component and need to understand the design structure.

**Steps:**

1. **Find the design file:**
   ```
   List my Penpot projects
   ```

2. **Get files in the project:**
   ```
   Show me files in project "Mobile App" (PROJECT_ID: abc123)
   ```

3. **Search for the component:**
   ```
   Find all components with "header" in their name in file xyz789
   ```

4. **Get the component tree:**
   ```
   Show me the object tree for component [COMPONENT_ID] in file xyz789
   ```

**Result:** You have a complete understanding of the design hierarchy and properties.

### Workflow 2: Exporting Design Assets

**Scenario:** You need design assets for development.

**Steps:**

1. **Locate the design element:**
   ```
   Search for "logo" in file [FILE_ID]
   ```

2. **Export the asset:**
   ```
   Export the logo component (ID: [OBJECT_ID]) from file [FILE_ID] as PNG
   ```

3. **Junie will provide:**
   - A URL to the exported image (if HTTP server is enabled)
   - The image embedded in the response
   - File details (format, size, etc.)

4. **Save or use the asset:**
   - Download from the provided URL
   - Or ask Junie to save it to a specific location

### Workflow 3: Design-Code Synchronization

**Scenario:** Ensuring your code matches the design specs.

**Steps:**

1. **Get the design file:**
   ```
   Fetch file [FILE_ID] and cache it
   ```

2. **Query component properties:**
   ```
   What are the dimensions and colors of the button component [OBJECT_ID]?
   ```

3. **Compare with code:**
   ```
   Does my Button.tsx component match these design specs?
   [Paste your code or ask Junie to check the file]
   ```

4. **Generate code from design:**
   ```
   Generate React/CSS code for the component tree of [OBJECT_ID]
   ```

**Result:** Junie can help you write code that matches the design specifications.

### Workflow 4: Batch Operations

**Scenario:** Export multiple assets at once.

**Prompt:**

```
I need to export all icon components from file [FILE_ID]:
1. Search for all objects with "icon" in the name
2. Export each as PNG at 2x scale
3. Save them to the assets folder
```

Junie will:
- Use `search_object` to find all icons
- Loop through and `export_object` for each
- Provide URLs or save locations for all exports

### Workflow 5: Design Documentation

**Scenario:** Generate documentation from design files.

**Prompt:**

```
Create a markdown document listing all components in file [FILE_ID] with:
- Component names
- Component types
- Hierarchy structure
- Screenshots of each component
```

Junie will:
- Fetch the file structure
- Extract component information
- Export screenshots
- Generate formatted markdown

---

## Troubleshooting

### Issue 1: MCP Server Not Starting

**Symptoms:**
- Junie says tools are unavailable
- No response to Penpot queries
- Rider shows "MCP server failed to start"

**Solutions:**

1. **Check .NET installation:**
   ```bash
   dotnet --version
   ```
   Should show 9.0 or later.

2. **Verify project builds:**
   - Right-click project in Rider → Build
   - Check for compilation errors

3. **Test server manually:**
   ```bash
   cd /home/linux/RiderProjects/FnPenpotMCP
   dotnet run
   ```
   Should start without errors.

4. **Check Rider logs:**
   - Help → Show Log in Explorer/Finder
   - Look for MCP-related errors in `idea.log`

5. **Verify MCP configuration:**
   - Settings → Tools → AI Assistant → Model Context Protocol
   - Ensure "Penpot Design Server" is enabled
   - Check command and arguments are correct

### Issue 2: Authentication Errors

**Symptoms:**
- "Authentication failed" messages
- CloudFlare protection warnings
- 401/403 HTTP errors

**Solutions:**

1. **Verify credentials:**
   - Open `.env` file
   - Check `PENPOT_USERNAME` and `PENPOT_PASSWORD`
   - Remove any extra spaces or quotes

2. **Handle CloudFlare protection:**
   - Open https://design.penpot.app in your browser
   - Log in to your account
   - Complete any CAPTCHA or verification
   - Try again in Rider/Junie

3. **Check API endpoint:**
   - Ensure `PENPOT_API_URL` is correct
   - Should end with `/api`
   - Test in browser: visit the API URL

### Issue 3: Tools Not Recognized by Junie

**Symptoms:**
- Junie doesn't use Penpot tools
- "I don't have access to that" responses
- Tools not listed when asking Junie

**Solutions:**

1. **Restart Rider:**
   - Close Rider completely
   - Wait a few seconds
   - Reopen the project

2. **Check AI Assistant status:**
   - Settings → Tools → AI Assistant
   - Ensure "Enable AI Assistant" is checked
   - Verify your AI subscription is active

3. **Verify MCP server is running:**
   - Check Rider's status bar for MCP indicators
   - Look in the Run/Debug tool window for server output

4. **Re-add the MCP server:**
   - Settings → Tools → AI Assistant → Model Context Protocol
   - Remove and re-add "Penpot Design Server"
   - Apply and restart Rider

### Issue 4: Image Export Failures

**Symptoms:**
- Export commands fail
- No images generated
- "Export failed" errors

**Solutions:**

1. **Enable HTTP server:**
   ```env
   ENABLE_HTTP_SERVER=true
   HTTP_SERVER_PORT=8080
   ```

2. **Check port availability:**
   ```bash
   # Linux/macOS
   lsof -i :8080
   
   # Windows
   netstat -ano | findstr :8080
   ```
   If port is in use, change to another port.

3. **Verify object IDs:**
   - Use `search_object` first to get valid IDs
   - Ensure page ID is correct
   - Check file ID is accurate

4. **Test export manually:**
   ```bash
   # In Rider terminal
   dotnet run
   ```
   Then test Penpot API directly via the server logs.

### Issue 5: Slow Performance

**Symptoms:**
- Tools take a long time to respond
- Junie times out
- Rider becomes unresponsive

**Solutions:**

1. **Use published binary:**
   ```bash
  dotnet publish ./FnPenpotMCP.fsproj -c Release -o ./publish
  ```
   Update MCP config to use `./publish/FnPenpotMCP`

2. **Disable debug logging:**
   ```env
   DEBUG=false
   ```

3. **Leverage caching:**
   - Files are cached automatically after first fetch
   - Ask Junie to "cache file [FILE_ID]" before heavy operations

4. **Check network:**
   - Ensure stable internet connection
   - Test Penpot API speed in browser

5. **Allocate more memory to Rider:**
   - Help → Change Memory Settings
   - Increase to at least 4GB (8GB recommended)

### Issue 6: Environment Variables Not Loading

**Symptoms:**
- Server can't find credentials
- "Configuration not found" errors

**Solutions:**

1. **Check `.env` location:**
   - Must be in project root: `/home/linux/RiderProjects/FnPenpotMCP/.env`
   - Not in `bin/` or `obj/` directories

2. **Verify `.env` format:**
   ```env
   # Correct
   PENPOT_USERNAME=user@example.com
   
   # Incorrect (no quotes needed)
   PENPOT_USERNAME="user@example.com"
   ```

3. **Set working directory:**
   - In MCP config, ensure `cwd` points to project root
   - The server loads `.env` relative to working directory

### Getting Help

If issues persist:

1. **Enable verbose logging:**
   ```env
   DEBUG=true
   ```

2. **Check server output:**
   - View → Tool Windows → Run
   - Look for detailed error messages

3. **Review documentation:**
   - [README.md](../README.md) - General information
   - [PROJECT-OVERVIEW.md](PROJECT-OVERVIEW.md) - User guide
   - [SESSION-LOG.md](SESSION-LOG.md) - Technical details

4. **Test independently:**
   - Run the server standalone
   - Verify Penpot API access in browser
   - Check .NET and Rider versions

---

## Best Practices

### 1. Organize Your Workflow

- **Cache files early:** Ask Junie to fetch and cache files you'll use repeatedly
- **Use descriptive names:** Name Penpot objects clearly for easier searching
- **Create shortcuts:** Save common queries as snippets in Rider

### 2. Efficient Asset Management

- **Batch exports:** Request multiple exports in one prompt
- **Use consistent naming:** Match design component names to code file names
- **Version control:** Commit exported assets to Git for tracking

### 3. Design-Code Consistency

- **Regular checks:** Periodically verify code matches design specs
- **Automated validation:** Use Junie to compare component properties
- **Documentation:** Keep design references in code comments

### 4. Security

- **Protect `.env`:**
  ```bash
  chmod 600 .env
  ```
- **Don't commit credentials:** Ensure `.env` is in `.gitignore`
- **Use tokens:** If Penpot supports API tokens, use them instead of passwords

### 5. Performance Optimization

- **Limit depth:** When requesting object trees, use reasonable depth limits
- **Filter searches:** Use specific regex patterns in searches
- **Cache strategically:** Pre-cache files you know you'll need

### 6. Integration with Development

**Use Junie to:**
- Generate component code from designs
- Validate CSS values match design specs
- Create design documentation automatically
- Export assets during build processes

**Example workflow:**

```
1. Fetch the design file for the current sprint
2. Cache it for quick access
3. As you implement each component:
   - Ask Junie for component specs
   - Export assets as needed
   - Verify your code matches the design
4. At the end: Generate a design-implementation report
```

---

## Advanced Usage

### Custom Scripts Integration

Create F# scripts that use the MCP server:

```fsharp
// ExportAllIcons.fsx
#r "nuget: FSharp.Data"

// Use Junie to:
// 1. Get all icon components
// 2. Export each at multiple scales
// 3. Organize in asset directories
```

Run via Junie:

```
Run my ExportAllIcons.fsx script and use the Penpot MCP server to fetch icons from file [FILE_ID]
```

### Automated Documentation

**Prompt:**

```
Create comprehensive design documentation:
1. List all projects and files
2. For each file, extract component hierarchy
3. Export screenshots of major components
4. Generate a markdown doc with:
   - Table of contents
   - Component index
   - Visual references
   - Property specifications
Save to docs/design-specs.md
```

### CI/CD Integration

Use the MCP server in build pipelines:

```yaml
# .github/workflows/export-assets.yml
name: Export Design Assets

on:
  schedule:
    - cron: '0 0 * * *'  # Daily

jobs:
  export:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '9.0.x'
      - name: Run asset export
        run: |
          cd FnPenpotMCP
          dotnet run -- export-all
```

### Multi-Project Management

Manage multiple Penpot projects:

**Prompt:**

```
I have 3 Penpot projects: "Web App", "Mobile App", "Marketing Site"
For each project:
1. List all files
2. Export the design system components
3. Create a component inventory
4. Check for inconsistencies across projects
```

### Design System Validation

**Prompt:**

```
Validate design system consistency:
1. Fetch the design system file
2. List all color tokens
3. Find all button variants
4. Check if all projects use these standards
5. Report any deviations
```

---

## Integration with Other Rider Features

### Using with Live Templates

Create Rider live templates that reference Penpot designs:

1. **Settings → Editor → Live Templates**
2. **Create new template:**
   ```tsx
   import { $COMPONENT$ } from '@/components';
   
   // Design reference: Penpot file $FILE_ID$, object $OBJECT_ID$
   // Generated with Junie AI + Penpot MCP
   
   export const $NAME$ = () => {
     return <$COMPONENT$ $PROPS$ />;
   };
   ```

3. **Use with Junie:**
   ```
   Create a React component using the "button-primary" design from Penpot file [FILE_ID]
   ```

### Code Inspections

Create custom inspections that validate against Penpot designs:

```
Junie, check if my Button.tsx component props match the design properties of object [OBJECT_ID]
```

### Debugging with Design Context

When debugging UI issues:

```
I'm debugging a layout issue in the Header component.
Can you:
1. Fetch the header design from Penpot file [FILE_ID]
2. Show me the expected dimensions and spacing
3. Compare with my current CSS
4. Suggest fixes
```

---

## Comparison: Rider/Junie vs LM Studio

| Feature | Rider/Junie | LM Studio |
|---------|-------------|-----------|
| **IDE Integration** | ✅ Native | ❌ External |
| **Code Context** | ✅ Full project awareness | ❌ Limited |
| **Workflow** | Seamless in IDE | Separate window |
| **Performance** | Fast (local) | Fast (local) |
| **Debugging** | ✅ IDE tools | ❌ None |
| **Code Generation** | ✅ Project-aware | ⚠️ Generic |
| **Asset Export** | ✅ To project folders | ⚠️ Manual save |
| **Best For** | Active development | Design exploration |

---

## Next Steps

Now that you've integrated Penpot MCP with Rider/Junie:

1. **Experiment with workflows:**
   - Try different prompts and use cases
   - Find what works best for your project

2. **Customize configuration:**
   - Adjust cache settings
   - Configure HTTP server as needed

3. **Create automations:**
   - Build scripts for common tasks
   - Integrate with CI/CD

4. **Share with team:**
   - Document your workflows
   - Create team templates
   - Set up shared Penpot projects

5. **Provide feedback:**
   - Report issues or suggestions
   - Contribute improvements

---

## Additional Resources

- **JetBrains Rider:** https://www.jetbrains.com/rider/
- **Junie AI Assistant:** https://www.jetbrains.com/ai/
- **Model Context Protocol:** https://spec.modelcontextprotocol.io/
- **Penpot:** https://penpot.app/
- **Project README:** [README.md](../README.md)
- **LM Studio Integration:** [LMSTUDIO-INTEGRATION.md](LMSTUDIO-INTEGRATION.md)
- **Project Overview:** [PROJECT-OVERVIEW.md](PROJECT-OVERVIEW.md)

---

## Frequently Asked Questions

### Q: Can I use this with other JetBrains IDEs?

**A:** Yes! The same MCP configuration works with:
- IntelliJ IDEA (with AI Assistant)
- WebStorm
- PyCharm
- Any JetBrains IDE that supports Junie/AI Assistant

Just adjust the project paths accordingly.

### Q: Does this work with GitHub Copilot in Rider?

**A:** GitHub Copilot and Junie/AI Assistant are separate. This integration is specifically for Junie. However, you can use both simultaneously.

### Q: Can multiple developers share the same MCP server?

**A:** Each developer should run their own MCP server instance with their own Penpot credentials. The server is designed to run locally per-user.

### Q: Will this slow down my IDE?

**A:** Minimal impact. The MCP server runs as a separate process and only activates when you use Penpot-related prompts with Junie.

### Q: Can I use this in offline mode?

**A:** No, the server needs internet access to communicate with Penpot's API. However, caching helps reduce API calls after initial fetches.

---

**Last Updated:** November 1, 2025  
**Version:** 1.0  
**Compatible with:** JetBrains Rider 2024.2+, .NET 9.0+, Junie AI Assistant
