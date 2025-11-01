# Using Penpot MCP Server with LM Studio on Linux Mint

**A Complete Beginner's Guide for Linux Mint Users**

## 🚀 TLDR - Quick Start

**Goal:** Connect your Penpot designs to LM Studio AI on Linux Mint.

**Time Required:** 15-20 minutes

**Quick Steps:**
1. Install .NET 9.0 SDK → `sudo apt install dotnet-sdk-9.0`
2. Navigate to project → `cd /home/linux/RiderProjects/FnPenpotMCP`
3. Configure credentials → Copy `.env.example` to `.env` and add your Penpot email/password
4. Build project → `dotnet build`
5. Add to LM Studio → Edit `~/.config/lm-studio/mcp-config.json`
6. Restart LM Studio and test

**Skip to:** [Detailed Instructions](#detailed-instructions) if you need explanations.

---

## Table of Contents

- [What You'll Need](#what-youll-need)
- [Detailed Instructions](#detailed-instructions)
- [Testing Your Setup](#testing-your-setup)
- [Common Issues and Solutions](#common-issues-and-solutions)
- [Understanding What You Did](#understanding-what-you-did)
- [Next Steps](#next-steps)

---

## What You'll Need

### Before You Start

✅ **Linux Mint** - Any recent version (20+)  
✅ **Penpot Account** - Free at [penpot.app](https://penpot.app)  
✅ **LM Studio** - Download from [lmstudio.ai](https://lmstudio.ai)  
✅ **Internet Connection** - For downloads and Penpot API access  
✅ **Terminal Access** - Don't worry, we'll guide you!

### What Gets Installed

- **.NET 9.0 SDK** (~500 MB) - Required to run the F# server
- **Project Dependencies** - Downloaded automatically during build

---

## Detailed Instructions

### Part 1: Install Required Software

#### Step 1.1: Install .NET 9.0 SDK

**What this does:** Installs Microsoft's .NET framework needed to run F# programs.

Open the Terminal application:
- Press `Ctrl+Alt+T` on your keyboard, OR
- Click the Menu → Terminal

Copy and paste this command (right-click to paste in terminal):

```bash
sudo apt update && sudo apt install -y dotnet-sdk-9.0
```

**What's happening:**
- `sudo` = Run with administrator privileges (you'll need to enter your password)
- `apt update` = Refresh the list of available software
- `apt install` = Install a program
- `-y` = Automatically say "yes" to prompts

**Expected output:**
```
[sudo] password for your-username: 
Reading package lists... Done
Building dependency tree... Done
...
Setting up dotnet-sdk-9.0 ...
```

**Verify installation:**

```bash
dotnet --version
```

You should see: `9.0.x` (where x is any number)

**If this fails:** The .NET 9.0 repository might not be configured. Run:

```bash
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb
sudo apt update
sudo apt install -y dotnet-sdk-9.0
```

---

#### Step 1.2: Install LM Studio

**What this does:** Installs the AI application that will use your Penpot designs.

1. **Download LM Studio:**
   - Open Firefox or your web browser
   - Go to: https://lmstudio.ai/
   - Click "Download for Linux"
   - Save the file (usually `LM-Studio-x.x.x.AppImage`)

2. **Make it executable:**

Navigate to your Downloads folder in terminal:

```bash
cd ~/Downloads
```

**What's happening:**
- `cd` = Change directory (like opening a folder)
- `~` = Your home folder shortcut
- `/Downloads` = The Downloads folder

Make the file runnable:

```bash
chmod +x LM-Studio-*.AppImage
```

**What's happening:**
- `chmod` = Change file permissions
- `+x` = Add execute permission (makes it runnable)
- `*` = Wildcard (matches any version number)

3. **Run LM Studio:**

```bash
./LM-Studio-*.AppImage
```

**What's happening:**
- `./` = Run a program in the current folder
- The program starts

**Optional:** Move to Applications folder for easy access:

```bash
sudo mv LM-Studio-*.AppImage /usr/local/bin/lmstudio
```

Now you can run it anytime with: `lmstudio`

---

### Part 2: Configure the Penpot MCP Server

#### Step 2.1: Navigate to the Project

**What this does:** Opens the folder containing the Penpot MCP server code.

```bash
cd /home/linux/RiderProjects/FnPenpotMCP
```

**What's happening:**
- Moving to the project directory where all the code lives

**Verify you're in the right place:**

```bash
ls
```

**What's happening:**
- `ls` = List files in current folder

You should see files like: `Config.fs`, `README.md`, `.env.example`, etc.

---

#### Step 2.2: Create Configuration File

**What this does:** Creates a file with your Penpot login credentials.

```bash
cp .env.example .env
```

**What's happening:**
- `cp` = Copy a file
- `.env.example` = Template file (source)
- `.env` = New file with your actual settings (destination)

---

#### Step 2.3: Edit Configuration with Your Credentials

**What this does:** Adds your Penpot email and password so the server can log in.

Open the file in a text editor:

```bash
nano .env
```

**What's happening:**
- `nano` = Simple text editor in terminal
- Opens the `.env` file for editing

**Find these lines and update them:**

```env
PENPOT_USERNAME=your-email@example.com
PENPOT_PASSWORD=your-password
```

Change to your actual Penpot credentials:

```env
PENPOT_USERNAME=jane.doe@gmail.com
PENPOT_PASSWORD=MySecurePassword123
```

**Save and exit:**
- Press `Ctrl+X` (to exit)
- Press `Y` (to confirm save)
- Press `Enter` (to confirm filename)

**Important Security Note:**
- This file contains your password
- Never share this file or commit it to GitHub
- Keep it private!

---

#### Step 2.4: Build the Project

**What this does:** Compiles the F# code into a runnable program.

```bash
dotnet build
```

**What's happening:**
- `dotnet build` = Compile the F# source code
- Downloads required libraries (first time only)
- Creates executable program in `bin/` folder

**Expected output:**

```
Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:10.9
```

**This takes:** 10-15 seconds on first run, faster afterwards.

**If you see errors:** Check that you're in the correct folder (`/home/linux/RiderProjects/FnPenpotMCP`).

---

#### Step 2.5: Test the Server

**What this does:** Verifies the server starts correctly before connecting to LM Studio.

```bash
dotnet run
```

**What's happening:**
- Runs the compiled program
- Starts the Penpot MCP server
- Shows available tools

**Expected output:**

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
  - get_file: Get file by ID (with caching)
  ...

Press Ctrl+C to exit...
```

**Stop the server:**
- Press `Ctrl+C`

**What's happening:**
- Sends interrupt signal to stop the program

✅ **If you see the above, everything is working!**

---

### Part 3: Connect to LM Studio

#### Step 3.1: Create MCP Configuration Directory

**What this does:** Creates the folder where LM Studio looks for MCP server settings.

```bash
mkdir -p ~/.config/lm-studio
```

**What's happening:**
- `mkdir` = Make directory (create folder)
- `-p` = Create parent folders if needed
- `~/.config/lm-studio` = Hidden config folder in your home directory

---

#### Step 3.2: Create MCP Configuration File

**What this does:** Tells LM Studio how to start the Penpot server.

```bash
nano ~/.config/lm-studio/mcp-config.json
```

**Paste this content** (right-click → Paste in terminal):

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

**Save and exit:**
- Press `Ctrl+X`
- Press `Y`
- Press `Enter`

**What each part means:**

- `"penpot"` = Name of this MCP server
- `"command": "dotnet"` = Program to run
- `"args"` = Arguments passed to dotnet
- `"--project"` = Tells dotnet which project to run
- `"/home/linux/..."` = Full path to your project file
- `"env"` = Environment variables for the server
- `"MODE": "stdio"` = Use standard input/output for communication
- `"DEBUG": "true"` = Show detailed logs

**Important:** Make sure the path matches your actual project location!

---

#### Step 3.3: Restart LM Studio

**What this does:** Makes LM Studio load the new MCP server configuration.

1. **Close LM Studio completely** (if running):
   - Click X button, OR
   - In terminal: `killall lmstudio` (if running from terminal)

2. **Start LM Studio again:**
   ```bash
   lmstudio
   ```
   Or click the LM Studio icon if you installed it to Applications.

3. **Wait for startup:**
   - LM Studio will automatically detect and load the Penpot MCP server
   - Look for notification: "MCP Server 'penpot' connected"

---

### Part 4: Enable MCP in LM Studio

#### Step 4.1: Open Settings

1. Click the **Settings** icon (⚙️) in LM Studio
2. Look for **"Developer"** or **"Advanced"** section
3. Find **"Model Context Protocol (MCP)"** settings

#### Step 4.2: Enable MCP Servers

1. Toggle **"Enable MCP Servers"** to ON
2. You should see **"penpot"** listed as an available server
3. Status should show **"Running"** or **"Connected"**

---

## Testing Your Setup

### Test 1: List Your Projects

1. **Start a new chat in LM Studio**
2. **Load a model** (if not already loaded)
3. **Type this prompt:**

```
Can you list my Penpot projects?
```

**Expected result:**
- The AI should respond with a list of your Penpot projects
- You'll see project names and IDs

**If it works:** 🎉 Success! Your integration is working!

### Test 2: Get Project Files

**Type:**

```
Show me all files in my first project
```

**Expected result:**
- List of design files in that project

### Test 3: Search for Components

First, note a file ID from the previous test, then:

```
Search for components with "button" in the name in file [FILE_ID]
```

Replace `[FILE_ID]` with an actual ID.

---

## Common Issues and Solutions

### Issue: "dotnet: command not found"

**Meaning:** .NET SDK is not installed or not in your PATH.

**Solution:**

```bash
sudo apt update
sudo apt install -y dotnet-sdk-9.0
```

Then verify:

```bash
dotnet --version
```

---

### Issue: "Build failed" errors

**Possible causes:**
1. Not in the correct directory
2. Project files are corrupted
3. Missing dependencies

**Solution:**

```bash
# Make sure you're in the project folder
cd /home/linux/RiderProjects/FnPenpotMCP

# Clean and rebuild
dotnet clean
dotnet restore
dotnet build
```

**What's happening:**
- `dotnet clean` = Remove old build files
- `dotnet restore` = Re-download dependencies
- `dotnet build` = Build fresh

---

### Issue: "Authentication failed" in LM Studio

**Meaning:** Wrong credentials or CloudFlare protection.

**Solution 1 - Check credentials:**

```bash
nano .env
```

Verify `PENPOT_USERNAME` and `PENPOT_PASSWORD` are correct.

**Solution 2 - Handle CloudFlare:**

1. Open Firefox and go to: https://design.penpot.app
2. Log in to your Penpot account
3. Complete any verification challenges
4. Try again in LM Studio

---

### Issue: LM Studio doesn't show Penpot tools

**Meaning:** MCP server didn't load or isn't enabled.

**Solution:**

1. **Check configuration file exists:**
   ```bash
   cat ~/.config/lm-studio/mcp-config.json
   ```
   Should show your JSON configuration.

2. **Verify path is correct:**
   ```bash
   ls /home/linux/RiderProjects/FnPenpotMCP/FnPenpotMCP.fsproj
   ```
   Should show the file exists.

3. **Check LM Studio settings:**
   - Settings → Developer → MCP
   - Ensure "Enable MCP Servers" is ON

4. **Restart LM Studio completely:**
   ```bash
   killall lmstudio
   lmstudio
   ```

---

### Issue: Permission denied errors

**Meaning:** File or folder doesn't have correct permissions.

**Solution:**

```bash
# Fix .env permissions
chmod 600 .env

# Fix project folder permissions
chmod -R 755 /home/linux/RiderProjects/FnPenpotMCP
```

**What's happening:**
- `chmod 600` = Only you can read/write the file
- `chmod -R 755` = You have full access, others can read/execute
- `-R` = Recursive (applies to all files inside)

---

## Understanding What You Did

### The Big Picture

```
You (Linux Mint)
    ↓
LM Studio (AI Application)
    ↓
Penpot MCP Server (F# Program)
    ↓
Penpot API (design.penpot.app)
    ↓
Your Penpot Designs
```

**Flow of a request:**

1. You ask LM Studio: "Show my projects"
2. LM Studio sees "penpot" MCP server is available
3. LM Studio calls the `list_projects` tool
4. Penpot MCP Server receives the request
5. Server logs into Penpot with your credentials
6. Server calls Penpot API
7. Penpot returns project data
8. Server formats the data
9. LM Studio receives formatted data
10. AI presents results to you

### Key Files and What They Do

| File/Folder | Purpose |
|-------------|---------|
| `.env` | Your credentials and settings |
| `FnPenpotMCP.fsproj` | Project definition file |
| `*.fs` files | F# source code (the program) |
| `bin/` folder | Compiled program (created after `dotnet build`) |
| `~/.config/lm-studio/mcp-config.json` | Tells LM Studio how to start the server |

### Terminal Commands Cheat Sheet

| Command | What It Does | Example |
|---------|--------------|---------|
| `cd` | Change directory | `cd ~/Documents` |
| `ls` | List files | `ls -la` (shows hidden files) |
| `pwd` | Print working directory | `pwd` |
| `cp` | Copy file | `cp file1.txt file2.txt` |
| `nano` | Edit text file | `nano myfile.txt` |
| `cat` | Show file contents | `cat .env` |
| `mkdir` | Make directory | `mkdir newfolder` |
| `chmod` | Change permissions | `chmod 755 file.txt` |
| `sudo` | Run as admin | `sudo apt install ...` |

---

## Next Steps

### 1. Explore Your Designs

Try these prompts in LM Studio:

- "What design files do I have?"
- "Find all button components across my projects"
- "Show me the structure of the navigation bar"
- "Export the header design as PNG"

### 2. Learn More

Read the other documentation:

- **[README.md](README.md)** - Technical details
- **[PROJECT-OVERVIEW.md](PROJECT-OVERVIEW.md)** - What the project does
- **[LMSTUDIO-INTEGRATION.md](LMSTUDIO-INTEGRATION.md)** - General LM Studio guide
- **[RIDER-JUNIE-INTEGRATION.md](RIDER-JUNIE-INTEGRATION.md)** - Using with JetBrains Rider

### 3. Customize Settings

Edit `.env` to change:
- Debug mode (`DEBUG=false` for less output)
- HTTP server port
- API URL (for self-hosted Penpot)

### 4. Automate Workflows

Now that you have AI access to your designs, you can:
- Generate design documentation automatically
- Export assets in batch
- Search across all your design files
- Analyze component usage

---

## Additional Resources

### Official Documentation

- **LM Studio:** https://lmstudio.ai/docs
- **Penpot:** https://help.penpot.app/
- **Model Context Protocol:** https://spec.modelcontextprotocol.io/
- **.NET on Linux:** https://learn.microsoft.com/dotnet/core/install/linux

### Linux Mint Resources

- **Linux Mint Forums:** https://forums.linuxmint.com/
- **Terminal Basics:** Linux Mint → Help → Terminal Commands
- **File Manager:** Nemo (built into Linux Mint)

### Getting Help

**If you get stuck:**

1. Check the [Troubleshooting](#common-issues-and-solutions) section
2. Enable debug mode in `.env`: `DEBUG=true`
3. Run server manually to see errors: `dotnet run`
4. Check LM Studio logs in `~/.lmstudio/logs/`

**Community Help:**

- LM Studio Discord
- Penpot Community Forum
- F# Software Foundation

---

## Summary

You've successfully:

✅ Installed .NET 9.0 SDK on Linux Mint  
✅ Installed and configured LM Studio  
✅ Set up the Penpot MCP Server with your credentials  
✅ Connected LM Studio to your Penpot designs  
✅ Tested the integration with real queries  

**You can now use AI to interact with your Penpot designs through natural language!**

---

**Last Updated:** November 1, 2025  
**Linux Mint Version:** Compatible with 20, 21, 22+  
**Tested With:** LM Studio 0.2.9+, .NET 9.0+

**Questions?** See the [Common Issues](#common-issues-and-solutions) section or check [PROJECT-OVERVIEW.md](PROJECT-OVERVIEW.md).
