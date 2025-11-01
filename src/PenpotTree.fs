module FnPenpotMCP.PenpotTree

open System.Text.Json

/// Represents a node in the Penpot object tree
type TreeNode = {
    Id: string
    Name: string
    Type: string
    Fields: Map<string, JsonElement>
    Children: TreeNode list
}

/// Extract a specific field from a JSON element
let private tryGetField (element: JsonElement) (fieldName: string) =
    try
        match element.ValueKind with
        | JsonValueKind.Object ->
            match element.TryGetProperty(fieldName) with
            | (true, value) -> Some value
            | _ -> None
        | _ -> None
    with
    | _ -> None

/// Get nested property using dot notation (e.g., "selrect.x")
let private getNestedProperty (element: JsonElement) (path: string) =
    let parts = path.Split('.')
    let rec traverse (current: JsonElement) (remainingParts: string list) =
        match remainingParts with
        | [] -> Some current
        | head :: tail ->
            match tryGetField current head with
            | Some next -> traverse next tail
            | None -> None
    traverse element (Array.toList parts)

/// Build a tree node with selected fields
let rec private buildTreeNode (objData: JsonElement) (includeFields: string list) (depth: int) (maxDepth: int) =
    let id = tryGetField objData "id" |> Option.map (fun e -> e.GetString()) |> Option.defaultValue ""
    let name = tryGetField objData "name" |> Option.map (fun e -> e.GetString()) |> Option.defaultValue ""
    let objType = tryGetField objData "type" |> Option.map (fun e -> e.GetString()) |> Option.defaultValue ""
    
    // Extract requested fields
    let fields =
        includeFields
        |> List.choose (fun fieldName ->
            match getNestedProperty objData fieldName with
            | Some value -> Some (fieldName, value)
            | None -> None
        )
        |> Map.ofList
    
    // Process children if not at max depth
    let children =
        if maxDepth = -1 || depth < maxDepth then
            match tryGetField objData "shapes" with
            | Some shapesElement when shapesElement.ValueKind = JsonValueKind.Array ->
                shapesElement.EnumerateArray()
                |> Seq.map (fun child -> buildTreeNode child includeFields (depth + 1) maxDepth)
                |> Seq.toList
            | _ -> []
        else
            []
    
    { Id = id; Name = name; Type = objType; Fields = fields; Children = children }

/// Find an object by ID in the pages index
let private findObjectInPages (pagesIndex: JsonElement) (objectId: string) =
    pagesIndex.EnumerateObject()
    |> Seq.tryPick (fun page ->
        let pageId = page.Name
        match tryGetField page.Value "objects" with
        | Some objects ->
            match tryGetField objects objectId with
            | Some obj -> Some (pageId, obj)
            | None -> None
        | None -> None
    )

/// Convert tree node to JSON-serializable dictionary
let rec private treeNodeToDict (node: TreeNode) =
    let mutable dict = Map.empty
    
    dict <- dict.Add("id", JsonSerializer.SerializeToElement(node.Id))
    dict <- dict.Add("name", JsonSerializer.SerializeToElement(node.Name))
    dict <- dict.Add("type", JsonSerializer.SerializeToElement(node.Type))
    
    // Add custom fields
    for kvp in node.Fields do
        dict <- dict.Add(kvp.Key, kvp.Value)
    
    // Add children if any
    if not (List.isEmpty node.Children) then
        let childrenDicts = node.Children |> List.map treeNodeToDict
        dict <- dict.Add("shapes", JsonSerializer.SerializeToElement(childrenDicts))
    
    dict

/// Get object subtree with specific fields
let getObjectSubtreeWithFields (fileData: JsonDocument) (objectId: string) (includeFields: string list) (depth: int) =
    try
        let data = fileData.RootElement
        
        // Get pages index
        match tryGetField data "data" with
        | None -> Error "No 'data' field found in file"
        | Some dataElement ->
            match tryGetField dataElement "pages-index" with
            | None ->
                match tryGetField dataElement "pagesIndex" with
                | None -> Error "No 'pages-index' or 'pagesIndex' field found"
                | Some pagesIndex ->
                    match findObjectInPages pagesIndex objectId with
                    | None -> Error (sprintf "Object with ID '%s' not found" objectId)
                    | Some (pageId, objData) ->
                        let tree = buildTreeNode objData includeFields 0 depth
                        let treeDict = treeNodeToDict tree
                        Ok (treeDict, pageId)
            | Some pagesIndex ->
                match findObjectInPages pagesIndex objectId with
                | None -> Error (sprintf "Object with ID '%s' not found" objectId)
                | Some (pageId, objData) ->
                    let tree = buildTreeNode objData includeFields 0 depth
                    let treeDict = treeNodeToDict tree
                    Ok (treeDict, pageId)
    with
    | ex -> Error (sprintf "Error processing tree: %s" ex.Message)

/// Search for objects by name using a predicate
let searchObjects (fileData: JsonDocument) (predicate: string -> bool) =
    try
        let data = fileData.RootElement
        let matches = System.Collections.Generic.List<_>()
        
        match tryGetField data "data" with
        | None -> []
        | Some dataElement ->
            let pagesIndex = 
                match tryGetField dataElement "pages-index" with
                | Some pi -> Some pi
                | None -> tryGetField dataElement "pagesIndex"
            
            match pagesIndex with
            | None -> []
            | Some pagesIndex ->
                pagesIndex.EnumerateObject()
                |> Seq.iter (fun page ->
                    let pageId = page.Name
                    let pageName = 
                        tryGetField page.Value "name"
                        |> Option.map (fun e -> e.GetString())
                        |> Option.defaultValue "Unnamed"
                    
                    match tryGetField page.Value "objects" with
                    | Some objects ->
                        objects.EnumerateObject()
                        |> Seq.iter (fun obj ->
                            let objName = 
                                tryGetField obj.Value "name"
                                |> Option.map (fun e -> e.GetString())
                                |> Option.defaultValue ""
                            
                            if predicate objName then
                                let objType = 
                                    tryGetField obj.Value "type"
                                    |> Option.map (fun e -> e.GetString())
                                    |> Option.defaultValue "unknown"
                                
                                matches.Add({|
                                    Id = obj.Name
                                    Name = objName
                                    PageId = pageId
                                    PageName = pageName
                                    ObjectType = objType
                                |})
                        )
                    | None -> ()
                )
                List.ofSeq matches
    with
    | _ -> []
