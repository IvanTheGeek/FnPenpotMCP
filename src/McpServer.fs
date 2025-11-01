module FnPenpotMCP.McpServer

open System
open System.IO
open System.Security.Cryptography
open System.Text
open System.Text.Json
open System.Text.RegularExpressions
open FnPenpotMCP.Config
open FnPenpotMCP.Cache
open FnPenpotMCP.HttpServer
open FnPenpotMCP.PenpotApi
open FnPenpotMCP.PenpotTree

/// Result type for tool operations
type ToolResult = 
    | Success of JsonDocument
    | Error of string
    | ImageResult of byte[] * string  // bytes, format

/// MCP Server state
type McpServerState = {
    Api: PenpotAPI
    FileCache: MemoryCache
    ImageServer: ImageServer option
    RenderedComponents: System.Collections.Concurrent.ConcurrentDictionary<string, byte[] * string>
}

/// Create MD5 hash for image IDs
let private md5Hash (input: string) =
    use md5 = MD5.Create()
    let bytes = Encoding.UTF8.GetBytes(input)
    let hash = md5.ComputeHash(bytes)
    BitConverter.ToString(hash).Replace("-", "").ToLower()

/// Handle API errors
let private handleApiError (ex: Exception) =
    match ex with
    | :? CloudFlareError as cfe ->
        JsonSerializer.SerializeToDocument({|
            error = "CloudFlare Protection"
            message = cfe.Message
            error_type = "cloudflare_protection"
            status_code = None
            instructions = [|
                "Open your web browser and navigate to https://design.penpot.app"
                "Log in to your Penpot account"
                "Complete any CloudFlare human verification challenges if prompted"
                "Once verified, try your request again"
            |]
        |})
    | :? PenpotAPIError as pae ->
        JsonSerializer.SerializeToDocument({|
            error = "Penpot API Error"
            message = pae.Message
            error_type = "api_error"
            status_code = None
        |})
    | _ ->
        JsonSerializer.SerializeToDocument({| error = ex.Message |})

/// Tool: List all projects
let listProjects (state: McpServerState) = async {
    try
        let! projects = state.Api.ListProjects()
        return Success projects
    with
    | ex -> return Success (handleApiError ex)
}

/// Tool: Get project files
let getProjectFiles (state: McpServerState) (projectId: string) = async {
    try
        let! files = state.Api.GetProjectFiles(projectId)
        return Success files
    with
    | ex -> return Success (handleApiError ex)
}

/// Tool: Get file by ID (with caching)
let getFile (state: McpServerState) (fileId: string) = async {
    try
        let! fileData = state.Api.GetFile(fileId)
        state.FileCache.Set(fileId, fileData)
        return Success fileData
    with
    | ex -> return Success (handleApiError ex)
}

/// Helper: Get cached file
let private getCachedFile (state: McpServerState) (fileId: string) = async {
    match state.FileCache.Get<JsonDocument>(fileId) with
    | Some cached -> return Some cached
    | None ->
        try
            let! fileData = state.Api.GetFile(fileId)
            state.FileCache.Set(fileId, fileData)
            return Some fileData
        with
        | _ -> return None
}

/// Tool: Export object as image
let exportObject (state: McpServerState) (fileId: string) (pageId: string) (objectId: string) (exportType: string) (scale: int) = async {
    let tempDir = Path.GetTempPath()
    let tempFile = Path.Combine(tempDir, sprintf "%s.%s" objectId exportType)
    
    try
        let! outputPath = state.Api.ExportAndDownload(fileId, pageId, objectId, exportType, scale, tempFile)
        let! bytes = File.ReadAllBytesAsync(outputPath) |> Async.AwaitTask
        
        // Add to HTTP server if enabled
        match state.ImageServer with
        | Some server when server.IsRunning ->
            let imageId = md5Hash (sprintf "%s:%s:%s" fileId pageId objectId)
            let url = server.AddImage(imageId, bytes, exportType)
            // Store in rendered components
            state.RenderedComponents.[imageId] <- (bytes, exportType)
        | _ -> ()
        
        // Clean up temp file
        try File.Delete(tempFile) with | _ -> ()
        
        return ImageResult (bytes, exportType)
    with
    | ex ->
        // Clean up temp file
        try if File.Exists(tempFile) then File.Delete(tempFile) with | _ -> ()
        return Success (handleApiError ex)
}

/// Tool: Get object tree with screenshot
let getObjectTree (state: McpServerState) (fileId: string) (objectId: string) (fields: string list) (depth: int) (format: string) = async {
    try
        match! getCachedFile state fileId with
        | None -> 
            return Success (JsonSerializer.SerializeToDocument({| error = "Failed to get file data" |}))
        | Some fileData ->
            match getObjectSubtreeWithFields fileData objectId fields depth with
            | Result.Error errorMsg ->
                return Success (JsonSerializer.SerializeToDocument({| error = errorMsg |}))
            | Result.Ok (treeDict, pageId) ->
                let mutable result = Map.empty<string, obj>
                result <- result.Add("tree", treeDict)
                
                // Try to export image
                try
                    let! imageResult = exportObject state fileId pageId objectId "png" 1
                    match imageResult with
                    | ImageResult (bytes, imgFormat) ->
                        let imageId = md5Hash (sprintf "%s:%s" fileId objectId)
                        state.RenderedComponents.[imageId] <- (bytes, imgFormat)
                        
                        let imageUri = sprintf "render_component://%s" imageId
                        
                        // Check if HTTP server is available
                        match state.ImageServer with
                        | Some server when server.IsRunning ->
                            let httpUrl = server.AddImage(imageId, bytes, imgFormat)
                            result <- result.Add("image", {|
                                uri = httpUrl
                                mcp_uri = imageUri
                                format = imgFormat
                            |})
                        | _ ->
                            result <- result.Add("image", {|
                                uri = imageUri
                                format = imgFormat
                            |})
                    | _ -> ()
                with
                | ex ->
                    result <- result.Add("image_error", ex.Message)
                
                // Handle format
                if format.ToLower() = "yaml" then
                    // For YAML, return a simple representation
                    result <- result.Add("yaml_note", "YAML output not fully supported in F# version")
                
                return Success (JsonSerializer.SerializeToDocument(result))
    with
    | ex -> return Success (handleApiError ex)
}

/// Tool: Search for objects by name
let searchObject (state: McpServerState) (fileId: string) (query: string) = async {
    try
        match! getCachedFile state fileId with
        | None ->
            return Success (JsonSerializer.SerializeToDocument({| error = "Failed to get file data" |}))
        | Some fileData ->
            let regex = Regex(query, RegexOptions.IgnoreCase)
            let predicate = fun (name: string) -> regex.IsMatch(name)
            let matches = searchObjects fileData predicate
            
            return Success (JsonSerializer.SerializeToDocument({| objects = matches |}))
    with
    | ex -> return Success (handleApiError ex)
}

/// Tool: Get rendered component by ID
let getRenderedComponent (state: McpServerState) (componentId: string) =
    match state.RenderedComponents.TryGetValue(componentId) with
    | (true, (bytes, format)) -> ImageResult (bytes, format)
    | _ -> Error (sprintf "Component with ID %s not found" componentId)

/// Tool: Get cached files
let getCachedFiles (state: McpServerState) =
    let cached = state.FileCache.GetAllCached<JsonDocument>()
    let result = {|
        count = cached.Count
        file_ids = cached |> Map.toList |> List.map fst
    |}
    Success (JsonSerializer.SerializeToDocument(result))

/// Tool: Get server info
let getServerInfo (state: McpServerState) =
    let mutable info = Map.empty<string, obj>
    info <- info.Add("status", "online")
    info <- info.Add("name", "Penpot MCP Server (F#)")
    info <- info.Add("description", "Model Context Provider for Penpot")
    info <- info.Add("api_url", penpotApiUrl)
    
    match state.ImageServer with
    | Some server when server.IsRunning ->
        info <- info.Add("image_server", server.BaseUrl)
    | _ -> ()
    
    Success (JsonSerializer.SerializeToDocument(info))

/// Load schema from resources
let private loadSchema (filename: string) =
    try
        let path = Path.Combine(resourcesPath, filename)
        if File.Exists(path) then
            let json = File.ReadAllText(path)
            Success (JsonSerializer.Deserialize<JsonDocument>(json))
        else
            Error (sprintf "Schema file not found: %s" filename)
    with
    | ex -> Error (sprintf "Failed to load schema: %s" ex.Message)

/// Tool: Get Penpot schema
let getPenpotSchema() =
    loadSchema "penpot-schema.json"

/// Tool: Get Penpot tree schema
let getPenpotTreeSchema() =
    loadSchema "penpot-tree-schema.json"

/// Create MCP server state
let createServerState (testMode: bool) =
    // Initialize API
    let api = createClientWithAuth penpotApiUrl debug penpotUsername penpotPassword
    
    // Initialize cache
    let fileCache = createCache()
    
    // Initialize HTTP server if enabled and not in test mode
    let imageServer =
        if enableHttpServer && not testMode then
            try
                let server = createServer httpServerHost httpServerPort
                let url = server.Start()
                printfn "Image server started at %s" url
                Some server
            with
            | ex ->
                printfn "Warning: Failed to start image server: %s" ex.Message
                None
        else
            None
    
    {
        Api = api
        FileCache = fileCache
        ImageServer = imageServer
        RenderedComponents = System.Collections.Concurrent.ConcurrentDictionary<string, byte[] * string>()
    }

/// Dispose server state
let disposeServerState (state: McpServerState) =
    (state.Api :> IDisposable).Dispose()
    match state.ImageServer with
    | Some server -> (server :> IDisposable).Dispose()
    | None -> ()
