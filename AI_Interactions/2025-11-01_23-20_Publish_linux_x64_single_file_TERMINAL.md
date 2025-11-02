# Terminal Session Log

Timestamp: 2025-11-01 23:20 (local)
Base directory: /home/linux/RiderProjects/FnPenpotMCP

## 1) Publish self-contained single-file for Linux x64

Command:
```
dotnet publish FnPenpotMCP.fsproj -c Release -r linux-x64 --self-contained true /p:PublishSingleFile=true -o publish/linux-x64
```

Output:
```
Restore complete (3.9s)
  FnPenpotMCP succeeded (7.1s) → publish/linux-x64/
Build succeeded in 11.6s

(Current directory: /home/linux/RiderProjects/FnPenpotMCP)
```

## 2) Verify output directory contents

Command:
```
ls -la publish/linux-x64
```

Output:
```
total 76884
drwxr-xr-x  3 linux linux     4096 Nov  1 23:20 .
drwxr-xr-x 17 linux linux     4096 Nov  1 23:20 ..
-rwxr-xr-x  1 linux linux 78687788 Nov  1 23:20 FnPenpotMCP
-rw-r--r--  1 linux linux    26344 Nov  1 23:20 FnPenpotMCP.pdb
drwxr-xr-x  2 linux linux     4096 Nov  1 23:20 resources

(Current directory: /home/linux/RiderProjects/FnPenpotMCP)
```
