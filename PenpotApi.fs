module FnPenpotMCP.PenpotApi

open System
open System.IO
open System.Net
open System.Net.Http
open System.Text
open System.Text.Json
open System.Text.Json.Serialization

/// Custom exceptions for Penpot API errors
exception CloudFlareError of message: string * statusCode: int option
exception PenpotAPIError of message: string * statusCode: int option

/// Represents the Penpot API client
type PenpotAPI(baseUrl: string, debug: bool, ?email: string, ?password: string) =
    let client = new HttpClient()
    let mutable accessToken: string option = None
    let mutable profileId: string option = None
    
    let email = defaultArg email ""
    let password = defaultArg password ""
    
    do
        // Set default headers
        client.DefaultRequestHeaders.Add("Accept", "application/json, application/transit+json")
        client.DefaultRequestHeaders.Add("User-Agent", 
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36")
    
    /// Check if response indicates CloudFlare error
    let isCloudFlareError (response: HttpResponseMessage) (responseText: string) =
        let cloudflareIndicators = [
            "cloudflare"; "cf-ray"; "attention required"; "checking your browser"
            "challenge"; "ddos protection"; "security check"; "cf-browser-verification"
            "cf-challenge-running"; "please wait while we are checking your browser"
            "enable cookies and reload the page"; "this process is automatic"
        ]
        
        // Check headers
        let serverHeader = 
            match response.Headers.TryGetValues("server") with
            | (true, values) -> String.Join("", values).ToLower()
            | _ -> ""
        
        let hasCfRay = response.Headers.Contains("cf-ray")
        
        if serverHeader.Contains("cloudflare") || hasCfRay then
            true
        else
            // Check response content
            let lowerText = responseText.ToLower()
            cloudflareIndicators |> List.exists (fun indicator -> lowerText.Contains(indicator))
    
    /// Create CloudFlare error message
    let createCloudFlareMessage (statusCode: int option) =
        let baseMessage = 
            "CloudFlare protection has blocked this request. This is common on penpot.app. " +
            "To resolve this issue:\n\n" +
            "1. Open your web browser and navigate to https://design.penpot.app\n" +
            "2. Log in to your Penpot account\n" +
            "3. Complete any CloudFlare human verification challenges if prompted\n" +
            "4. Once verified, try your request again\n\n" +
            "The verification typically lasts for a period of time, after which you may need to repeat the process."
        
        match statusCode with
        | Some code -> sprintf "%s\n\nHTTP Status: %d" baseMessage code
        | None -> baseMessage
    
    /// Make an authenticated HTTP request
    let makeRequest (method: HttpMethod) (url: string) (content: HttpContent option) = async {
        try
            let request = new HttpRequestMessage(method, url)
            
            content |> Option.iter (fun c -> request.Content <- c)
            
            // Add auth token if available
            accessToken |> Option.iter (fun token ->
                request.Headers.Add("Authorization", sprintf "Token %s" token)
            )
            
            let! response = client.SendAsync(request) |> Async.AwaitTask
            let! responseText = response.Content.ReadAsStringAsync() |> Async.AwaitTask
            
            if debug then
                printfn "Request: %s %s" (method.ToString()) url
                printfn "Status: %d" (int response.StatusCode)
            
            if not response.IsSuccessStatusCode then
                if isCloudFlareError response responseText then
                    raise (CloudFlareError(createCloudFlareMessage (Some (int response.StatusCode)), Some (int response.StatusCode)))
                else
                    raise (PenpotAPIError(sprintf "API request failed: %s" responseText, Some (int response.StatusCode)))
            
            return responseText
        with
        | :? CloudFlareError as ex -> return raise ex
        | :? PenpotAPIError as ex -> return raise ex
        | ex -> return raise (PenpotAPIError(sprintf "Request failed: %s" ex.Message, None))
    }
    
    /// Parse JSON response
    let parseJson (json: string) =
        try
            JsonSerializer.Deserialize<JsonDocument>(json, JsonSerializerOptions(PropertyNameCaseInsensitive = true))
        with
        | ex -> raise (PenpotAPIError(sprintf "JSON parsing failed: %s" ex.Message, None))
    
    /// Set access token
    member _.SetAccessToken(token: string) =
        accessToken <- Some token
        // Set cookie for cookie-based auth
        let cookieContainer = new CookieContainer()
        cookieContainer.Add(Uri(baseUrl), Cookie("auth-token", token))
    
    /// Login with email and password
    member this.LoginWithPassword(?emailOverride: string, ?passwordOverride: string) = async {
        let loginEmail = defaultArg emailOverride email
        let loginPassword = defaultArg passwordOverride password
        
        if String.IsNullOrEmpty(loginEmail) || String.IsNullOrEmpty(loginPassword) then
            raise (PenpotAPIError("Email and password are required for login", None))
        
        let url = sprintf "%s/rpc/command/login-with-password" baseUrl
        let payload = sprintf """{"email":"%s","password":"%s"}""" loginEmail loginPassword
        let content = new StringContent(payload, Encoding.UTF8, "application/json")
        
        let! responseText = makeRequest HttpMethod.Post url (Some content)
        let response = parseJson responseText
        
        // Extract token from response
        let token = 
            try
                response.RootElement.GetProperty("access-token").GetString()
            with
            | _ -> response.RootElement.GetProperty("accessToken").GetString()
        
        this.SetAccessToken(token)
        
        // Try to extract profile ID
        try
            let pid = response.RootElement.GetProperty("profile-id").GetString()
            profileId <- Some pid
            if debug then printfn "Profile ID: %s" pid
        with
        | _ -> ()
        
        return token
    }
    
    /// Get profile information
    member _.GetProfile() = async {
        let url = sprintf "%s/rpc/command/get-profile" baseUrl
        let content = new StringContent("{}", Encoding.UTF8, "application/json")
        
        let! responseText = makeRequest HttpMethod.Post url (Some content)
        return parseJson responseText
    }
    
    /// List all projects
    member _.ListProjects() = async {
        let url = sprintf "%s/rpc/command/get-projects" baseUrl
        let content = new StringContent("{}", Encoding.UTF8, "application/json")
        
        let! responseText = makeRequest HttpMethod.Post url (Some content)
        return parseJson responseText
    }
    
    /// Get files in a project
    member _.GetProjectFiles(projectId: string) = async {
        let url = sprintf "%s/rpc/command/get-project-files" baseUrl
        let payload = sprintf """{"project-id":"%s"}""" projectId
        let content = new StringContent(payload, Encoding.UTF8, "application/json")
        
        let! responseText = makeRequest HttpMethod.Post url (Some content)
        return parseJson responseText
    }
    
    /// Get a file by ID
    member _.GetFile(fileId: string) = async {
        let url = sprintf "%s/rpc/command/get-file" baseUrl
        let payload = sprintf """{"id":"%s","features":["components/v2"]}""" fileId
        let content = new StringContent(payload, Encoding.UTF8, "application/json")
        
        let! responseText = makeRequest HttpMethod.Post url (Some content)
        return parseJson responseText
    }
    
    /// Export and download an object
    member _.ExportAndDownload(fileId: string, pageId: string, objectId: string, exportType: string, scale: int, saveToFile: string) = async {
        // First, create export
        let exportUrl = sprintf "%s/export/%s/%s/%s" baseUrl fileId pageId objectId
        let queryParams = sprintf "?type=%s&scale=%d" exportType scale
        let url = exportUrl + queryParams
        
        let! responseText = makeRequest HttpMethod.Get url None
        
        // Download the file
        let! bytes = client.GetByteArrayAsync(url) |> Async.AwaitTask
        do! File.WriteAllBytesAsync(saveToFile, bytes) |> Async.AwaitTask
        
        return saveToFile
    }
    
    interface IDisposable with
        member _.Dispose() =
            client.Dispose()

/// Create a new Penpot API client
let createClient baseUrl debug = new PenpotAPI(baseUrl, debug)

/// Create a new Penpot API client with credentials
let createClientWithAuth baseUrl debug email password = 
    new PenpotAPI(baseUrl, debug, email, password)
