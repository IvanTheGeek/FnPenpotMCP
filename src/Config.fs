module FnPenpotMCP.Config

open System
open System.IO

// Load environment variables at module initialization (optional; don't fail if .env missing)
do
    try
        // Look for .env in current working directory; ignore if not found
        DotNetEnv.Env.Load() |> ignore
    with _ -> ()

// Helper to get environment variable with default
let private getEnvOrDefault key defaultValue =
    match Environment.GetEnvironmentVariable(key) with
    | null | "" -> defaultValue
    | value -> value

let private getEnvBool key defaultValue =
    match Environment.GetEnvironmentVariable(key) with
    | null | "" -> defaultValue
    | value -> value.ToLower() = "true"

let private getEnvInt key defaultValue =
    match Environment.GetEnvironmentVariable(key) with
    | null | "" -> defaultValue
    | value -> 
        match Int32.TryParse(value) with
        | (true, i) -> i
        | _ -> defaultValue

// Server configuration
let port = getEnvInt "PORT" 5000
let debug = getEnvBool "DEBUG" true
let resourcesAsTools = getEnvBool "RESOURCES_AS_TOOLS" true

// HTTP server for exported images
let enableHttpServer = getEnvBool "ENABLE_HTTP_SERVER" true
let httpServerHost = getEnvOrDefault "HTTP_SERVER_HOST" "localhost"
let httpServerPort = getEnvInt "HTTP_SERVER_PORT" 0

// Penpot API configuration
let penpotApiUrl = getEnvOrDefault "PENPOT_API_URL" "https://design.penpot.app/api"
let penpotUsername = getEnvOrDefault "PENPOT_USERNAME" ""
let penpotPassword = getEnvOrDefault "PENPOT_PASSWORD" ""

// Resources path
// Use AppContext.BaseDirectory for single-file publish compatibility
let resourcesPath = 
    let baseDir = AppContext.BaseDirectory
    Path.Combine(baseDir, "resources")
