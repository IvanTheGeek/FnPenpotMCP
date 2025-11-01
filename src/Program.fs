open System
open FnPenpotMCP.Config
open FnPenpotMCP.McpServer

/// Parse command-line arguments
let parseArgs (args: string[]) =
    let mutable port = None
    let mutable debugMode = None
    let mutable mode = 
        match Environment.GetEnvironmentVariable("MODE") with
        | null | "" -> None
        | m -> Some m
    
    let rec parse i =
        if i >= args.Length then ()
        else
            match args.[i] with
            | "--port" when i + 1 < args.Length ->
                port <- Some (Int32.Parse(args.[i + 1]))
                parse (i + 2)
            | "--debug" ->
                debugMode <- Some true
                parse (i + 1)
            | "--mode" when i + 1 < args.Length ->
                mode <- Some args.[i + 1]
                parse (i + 2)
            | _ -> parse (i + 1)
    
    parse 0
    (port, debugMode, mode)

[<EntryPoint>]
let main args =
    try
        let (portOverride, debugOverride, modeOverride) = parseArgs args
        
        let actualPort = defaultArg portOverride port
        let actualDebug = defaultArg debugOverride debug
        let actualMode = defaultArg modeOverride "stdio"
        
        printfn "==================================="
        printfn "Penpot MCP Server (F# Edition)"
        printfn "==================================="
        printfn "Mode: %s" actualMode
        printfn "Debug: %b" actualDebug
        printfn "API URL: %s" penpotApiUrl
        
        if actualMode = "sse" then
            printfn "Port: %d" actualPort
        
        printfn "==================================="
        
        // Create server state
        let serverState = createServerState false
        
        printfn "\nServer initialized successfully!"
        printfn "\nAvailable Tools:"
        printfn "  - list_projects: List all Penpot projects"
        printfn "  - get_project_files: Get files in a project"
        printfn "  - get_file: Get file by ID (with caching)"
        printfn "  - export_object: Export design object as image"
        printfn "  - get_object_tree: Get object tree with screenshot"
        printfn "  - search_object: Search for objects by name"
        printfn "  - get_rendered_component: Get rendered component by ID"
        printfn "  - get_cached_files: List cached files"
        printfn "  - get_server_info: Get server information"
        printfn "  - get_penpot_schema: Get Penpot API schema"
        printfn "  - get_penpot_tree_schema: Get Penpot tree schema"
        
        printfn "\n==================================="
        printfn "Note: Full MCP protocol integration requires"
        printfn "additional implementation based on the"
        printfn "ModelContextProtocol .NET SDK."
        printfn "==================================="
        
        // Keep server running
        printfn "\nPress Ctrl+C to exit..."
        
        // Set up clean shutdown
        use __ = Console.CancelKeyPress.Subscribe(fun _ ->
            printfn "\nShutting down server..."
            disposeServerState serverState
            printfn "Server stopped."
        )
        
        // Wait indefinitely (in production, this would be the MCP message loop)
        Threading.Thread.Sleep(Threading.Timeout.Infinite)
        
        0
    with
    | ex ->
        eprintfn "Error: %s" ex.Message
        eprintfn "Stack trace: %s" ex.StackTrace
        1
