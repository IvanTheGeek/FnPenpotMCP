module FnPenpotMCP.Cache

open System
open System.Collections.Concurrent

/// Represents cached data with timestamp
type private CachedEntry<'T> = {
    Timestamp: DateTime
    Data: 'T
}

/// In-memory cache implementation with TTL support
type MemoryCache(ttlSeconds: int) =
    let cache = ConcurrentDictionary<string, CachedEntry<obj>>()
    
    /// Check if a cache entry is expired
    let isExpired entry =
        (DateTime.UtcNow - entry.Timestamp).TotalSeconds > float ttlSeconds
    
    /// Get a value from cache if it exists and is not expired
    member _.Get<'T>(key: string) : 'T option =
        match cache.TryGetValue(key) with
        | (true, entry) when not (isExpired entry) ->
            Some (entry.Data :?> 'T)
        | (true, _) ->
            // Remove expired entry
            cache.TryRemove(key) |> ignore
            None
        | _ -> None
    
    /// Store a value in cache
    member _.Set<'T>(key: string, data: 'T) : unit =
        let entry = {
            Timestamp = DateTime.UtcNow
            Data = box data
        }
        cache.[key] <- entry
    
    /// Clear all cached items
    member _.Clear() : unit =
        cache.Clear()
    
    /// Get all valid (non-expired) cached files
    member _.GetAllCached<'T>() : Map<string, 'T> =
        let currentTime = DateTime.UtcNow
        cache
        |> Seq.choose (fun kvp ->
            let entry = kvp.Value
            if (currentTime - entry.Timestamp).TotalSeconds <= float ttlSeconds then
                Some (kvp.Key, entry.Data :?> 'T)
            else
                // Remove expired entry
                cache.TryRemove(kvp.Key) |> ignore
                None
        )
        |> Map.ofSeq
    
    /// Get the number of items currently in cache (including expired)
    member _.Count = cache.Count

/// Create a new memory cache with default TTL of 10 minutes
let createCache() = MemoryCache(600)

/// Create a new memory cache with custom TTL
let createCacheWithTtl ttlSeconds = MemoryCache(ttlSeconds)
