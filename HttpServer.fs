module FnPenpotMCP.HttpServer

open System
open System.Collections.Concurrent
open System.Threading
open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful
open Suave.RequestErrors

/// Represents an image with its data and format
type ImageData = {
    Data: byte[]
    Format: string
}

/// In-memory image storage
let private images = ConcurrentDictionary<string, ImageData>()

/// Get content type based on image format
let private getContentType (format: string) =
    match format.ToLower() with
    | "svg" -> "image/svg+xml"
    | "png" -> "image/png"
    | "jpg" | "jpeg" -> "image/jpeg"
    | _ -> sprintf "image/%s" format

/// Handler for serving images
let private imageHandler imageId =
    match images.TryGetValue(imageId) with
    | (true, imageData) ->
        Writers.setMimeType (getContentType imageData.Format)
        >=> Successful.ok imageData.Data
    | _ ->
        NOT_FOUND "Image not found"

/// Suave app for serving images
let private app =
    choose [
        pathScan "/images/%s.%s" (fun (imageId, _ext) -> imageHandler imageId)
        pathScan "/images/%s" (fun imageId -> imageHandler imageId)
        NOT_FOUND "Not found"
    ]

/// Image server state
type ImageServer(host: string, port: int) =
    let mutable serverThread: Thread option = None
    let mutable actualPort = port
    let mutable baseUrl = ""
    let mutable cts: CancellationTokenSource option = None
    
    /// Check if server is running
    member _.IsRunning = Option.isSome serverThread
    
    /// Get the base URL with actual port
    member _.BaseUrl = baseUrl
    
    /// Get the actual port being used
    member _.Port = actualPort
    
    /// Start the server in a background thread
    member this.Start() =
        if this.IsRunning then
            baseUrl
        else
            let cancellationTokenSource = new CancellationTokenSource()
            cts <- Some cancellationTokenSource
            
            // Configure Suave
            let config = 
                { defaultConfig with
                    bindings = [ HttpBinding.createSimple HTTP host port ]
                    cancellationToken = cancellationTokenSource.Token }
            
            // Start server in background thread
            let thread = new Thread(fun () ->
                try
                    startWebServer config app
                with
                | :? OperationCanceledException -> ()
                | ex -> printfn "Server error: %s" ex.Message
            )
            thread.IsBackground <- true
            thread.Start()
            
            serverThread <- Some thread
            
            // Get actual port (if port was 0, Suave assigns a random one)
            // For now, we'll use the configured port
            actualPort <- port
            baseUrl <- sprintf "http://%s:%d" host actualPort
            
            printfn "Image server started at %s" baseUrl
            baseUrl
    
    /// Stop the server
    member this.Stop() =
        match cts with
        | Some cancellationTokenSource ->
            cancellationTokenSource.Cancel()
            cancellationTokenSource.Dispose()
            cts <- None
        | None -> ()
        
        serverThread <- None
        printfn "Image server stopped"
    
    /// Add an image to the server
    member this.AddImage(imageId: string, imageData: byte[], format: string) =
        images.[imageId] <- { Data = imageData; Format = format }
        sprintf "%s/images/%s.%s" baseUrl imageId format
    
    /// Remove an image from the server
    member _.RemoveImage(imageId: string) =
        images.TryRemove(imageId) |> ignore
    
    interface IDisposable with
        member this.Dispose() =
            this.Stop()

/// Create a new image server
let createServer host port = new ImageServer(host, port)
