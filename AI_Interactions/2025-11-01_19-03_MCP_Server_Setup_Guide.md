# MCP Server Setup Guide for JetBrains Rider

**Date:** 2025-11-01 19:03  
**Session Type:** Documentation and Interaction Logging

---

## Original User Prompt

```
1. Provide clear, step-by-step instructions to add the GitHub-hosted MCP server in JetBrains Rider (AI Assistant) and explain how Junie will use it.
2. Create AI_Interactions log file with the full conversation and my response.
- Include original user prompts, my responses, any clarifying questions/answers (none yet) — in chronological order.
3. Create companion TERMINAL log file capturing all terminal commands and outputs from this session (e.g., git status, add, commit).
4. Stage and commit the new log files with a descriptive commit message.
5. Provide optional per‑project `mcp.json` guidance and verification/troubleshooting notes.
```

---

## Junie's Response

### Part 1: Step-by-Step Instructions for Adding GitHub-Hosted MCP Server in JetBrains Rider

#### What is MCP (Model Context Protocol)?

MCP is a protocol that allows AI assistants to connect to external data sources and tools. In the context of JetBrains Rider's AI Assistant, MCP servers can provide additional context, tools, and capabilities that extend what the AI can access and do.

#### Prerequisites

1. **JetBrains Rider** with AI Assistant enabled
2. **MCP server** hosted on GitHub (typically as a repository with server implementation)
3. **Node.js/Python/Other runtime** (depending on the MCP server implementation)

---

### Configuration Method 1: Global MCP Settings

#### Step 1: Locate Rider's MCP Configuration Directory

MCP configuration for JetBrains IDEs is typically stored in:

**Linux/macOS:**
```
~/.config/JetBrains/Rider{VERSION}/mcp/
```

**Windows:**
```
%APPDATA%\JetBrains\Rider{VERSION}\mcp\
```

Replace `{VERSION}` with your Rider version (e.g., `Rider2024.3`).

#### Step 2: Create or Edit `mcp_settings.json`

Create a file named `mcp_settings.json` in the MCP configuration directory with the following structure:

```json
{
  "mcpServers": {
    "github-server-name": {
      "command": "node",
      "args": [
        "/path/to/cloned/mcp-server/dist/index.js"
      ],
      "env": {
        "GITHUB_TOKEN": "your_github_token_here"
      }
    }
  }
}
```

**Key Configuration Elements:**

- **`github-server-name`**: A unique identifier for your MCP server
- **`command`**: The runtime executable (e.g., `node`, `python3`, or direct path to executable)
- **`args`**: Array of arguments, typically the path to the server's entry point
- **`env`**: Environment variables required by the server (e.g., API tokens, configuration)

#### Step 3: Clone the GitHub MCP Server Repository

```bash
# Clone the MCP server repository
git clone https://github.com/username/mcp-server-repo.git /path/to/mcp-server

# Navigate to the directory
cd /path/to/mcp-server

# Install dependencies (for Node.js-based servers)
npm install

# Build if necessary
npm run build
```

#### Step 4: Restart JetBrains Rider

After creating/editing the configuration file:
1. Close all Rider instances
2. Restart Rider
3. The AI Assistant should automatically detect and connect to the configured MCP server

---

### Configuration Method 2: Per-Project MCP Settings

For project-specific MCP server configurations, you can create a `.mcp.json` file in your project root.

#### Creating `.mcp.json` in Project Root

```json
{
  "mcpServers": {
    "penpot-server": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "FnPenpotMCP.fsproj"
      ],
      "env": {
        "PENPOT_API_TOKEN": "${env:PENPOT_API_TOKEN}"
      }
    }
  }
}
```

**Benefits of Per-Project Configuration:**

- Project-specific servers that only run when working on that project
- Version-controlled configuration (can be committed to repository)
- Team members can share the same MCP server setup
- Easier to manage different servers for different projects

**Note:** The `.mcp.json` filename may vary depending on Rider version. Check the official documentation or use `mcp.json` without the leading dot if the above doesn't work.

---

### How Junie Uses MCP Servers

When an MCP server is properly configured in Rider:

1. **Automatic Connection**: Junie (the AI Assistant) establishes a connection to the MCP server when Rider starts or when entering a project with per-project configuration.

2. **Tool Discovery**: Junie queries the MCP server for available tools/functions it provides.

3. **Context Enhancement**: The MCP server can provide:
   - Access to external APIs (e.g., GitHub, Penpot, databases)
   - Project-specific context and data
   - Custom tooling and utilities
   - Real-time information from external sources

4. **Tool Invocation**: When responding to your queries, Junie can:
   - Call MCP server tools to fetch data
   - Execute operations through the server
   - Combine MCP server capabilities with built-in IDE features
   - Provide more informed and context-aware responses

5. **Seamless Integration**: From your perspective, Junie's responses appear enhanced with additional capabilities, but the MCP server communication happens transparently in the background.

---

### Verification Steps

#### Check if MCP Server is Connected

1. **Open Rider AI Assistant**
2. **Look for MCP indicators** in the AI Assistant panel (may show connected servers)
3. **Test a query** that would require the MCP server's capabilities

#### Debug Connection Issues

If the MCP server doesn't connect:

1. **Check Rider Logs**:
   - Go to `Help → Show Log in Explorer/Finder`
   - Look for `idea.log` and search for "MCP" or error messages

2. **Verify Server Executable**:
   ```bash
   # Test running the server directly
   node /path/to/mcp-server/dist/index.js
   ```

3. **Validate JSON Syntax**:
   - Use a JSON validator to ensure `mcp_settings.json` or `.mcp.json` is valid
   - Check for trailing commas, missing quotes, etc.

4. **Check Environment Variables**:
   - Ensure all required environment variables are set
   - For tokens, verify they're valid and have necessary permissions

5. **Review Server Logs**:
   - The MCP server itself may produce logs
   - Check the server's output directory or console output

---

### Troubleshooting Common Issues

#### Issue: "MCP Server Not Found"

**Solutions:**
- Verify the `command` path is correct and executable
- Ensure the runtime (Node.js, Python, etc.) is in your system PATH
- Try using absolute paths instead of relative paths

#### Issue: "Authentication Failed"

**Solutions:**
- Verify environment variables are correctly set in the `env` section
- Check that API tokens haven't expired
- Ensure tokens have the required permissions/scopes

#### Issue: "Server Starts But Tools Not Available"

**Solutions:**
- Verify the server implements the MCP protocol correctly
- Check that the server's tool definitions are properly exported
- Review server logs for initialization errors

#### Issue: "Configuration Not Loading"

**Solutions:**
- Ensure the configuration file is in the correct directory
- Check file permissions (should be readable)
- Verify JSON syntax is valid
- Restart Rider completely (not just close/reopen project)

---

### Optional: Per-Project mcp.json Example for FnPenpotMCP

Based on the current project structure, here's a suggested `.mcp.json` for the FnPenpotMCP project:

```json
{
  "mcpServers": {
    "penpot-local": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "/home/linux/RiderProjects/FnPenpotMCP/FnPenpotMCP.fsproj",
        "--configuration",
        "Release"
      ],
      "env": {
        "PENPOT_API_TOKEN": "${env:PENPOT_API_TOKEN}",
        "PENPOT_BASE_URL": "${env:PENPOT_BASE_URL}"
      },
      "description": "Local Penpot MCP server for design file operations"
    }
  }
}
```

**Using Environment Variable References:**
- `${env:VAR_NAME}` syntax allows referencing system environment variables
- This keeps secrets out of the configuration file
- Set environment variables in your shell profile or Rider's environment settings

**Alternative with Published Executable:**

If you've published the FnPenpotMCP server as a self-contained executable:

```json
{
  "mcpServers": {
    "penpot-published": {
      "command": "/home/linux/RiderProjects/FnPenpotMCP/publish/FnPenpotMCP",
      "args": [],
      "env": {
        "PENPOT_API_TOKEN": "${env:PENPOT_API_TOKEN}",
        "PENPOT_BASE_URL": "${env:PENPOT_BASE_URL}"
      },
      "description": "Published Penpot MCP server (self-contained)"
    }
  }
}
```

---

### Advanced Configuration Options

#### Multiple MCP Servers

You can configure multiple MCP servers simultaneously:

```json
{
  "mcpServers": {
    "github-server": {
      "command": "node",
      "args": ["/path/to/github-mcp/index.js"],
      "env": {
        "GITHUB_TOKEN": "${env:GITHUB_TOKEN}"
      }
    },
    "penpot-server": {
      "command": "dotnet",
      "args": ["run", "--project", "FnPenpotMCP.fsproj"],
      "env": {
        "PENPOT_API_TOKEN": "${env:PENPOT_API_TOKEN}"
      }
    },
    "database-server": {
      "command": "python3",
      "args": ["/path/to/db-mcp/server.py"],
      "env": {
        "DB_CONNECTION_STRING": "${env:DB_CONNECTION_STRING}"
      }
    }
  }
}
```

#### Server-Specific Settings

Some MCP servers may support additional configuration through environment variables or command-line arguments. Consult the specific server's documentation for available options.

---

### Security Considerations

1. **Never commit secrets**: Don't include API tokens directly in `.mcp.json` if it's version-controlled
2. **Use environment variables**: Always reference sensitive data via `${env:VAR_NAME}`
3. **Limit permissions**: Grant MCP servers only the minimum required API permissions
4. **Review server code**: Since MCP servers have access to your IDE context, review the code of third-party servers
5. **Use HTTPS**: Ensure MCP servers communicating over networks use secure connections

---

### Additional Resources

- **MCP Protocol Specification**: [https://github.com/modelcontextprotocol/specification](https://github.com/modelcontextprotocol/specification)
- **JetBrains AI Assistant Documentation**: Check JetBrains website for the latest Rider AI Assistant features
- **MCP Server Examples**: [https://github.com/modelcontextprotocol/servers](https://github.com/modelcontextprotocol/servers)

---

## Session Summary

This session involved creating comprehensive documentation for setting up MCP servers in JetBrains Rider. The guide covers:

1. ✅ Step-by-step instructions for global and per-project MCP configuration
2. ✅ Explanation of how Junie uses MCP servers
3. ✅ Verification and troubleshooting procedures
4. ✅ Project-specific examples for FnPenpotMCP
5. ✅ Security considerations and advanced configuration options
6. ✅ Interaction logging according to guidelines

Files created:
- `AI_Interactions/2025-11-01_19-03_MCP_Server_Setup_Guide.md` (this file)
- `AI_Interactions/2025-11-01_19-03_MCP_Server_Setup_Guide_TERMINAL.md` (companion terminal log)

---

**End of Interaction Log**
