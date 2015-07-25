#r @"..\packages\SQLite.Net-PCL.2.5.1\lib\portable-win8+net45+wp8+wpa81+MonoAndroid1+MonoTouch1\SQLite.Net.dll"
#r @"..\packages\SQLite.Net.Async-PCL.2.5.1\lib\portable-win8+net45+wp8+wpa81+MonoAndroid1+MonoTouch1\SQLite.Net.Async.dll"
#r @"..\packages\SQLite.Net.Platform.WinRT.2.5.1\lib\Windows8\SQLite.Net.Platform.WinRT.dll"

#r "System.Runtime"
#r "System.Threading"
#r "System.Threading.Tasks"
#r "System"
#r "System.IO"
#r "System.Collections"
#r "System.Reflection"

#load "Sql.fs"

open DataAccess
open System.Threading.Tasks;
open SQLite.Net.Async;
open SQLite.Net.Platform.WinRT;
open System;
open System.IO;

Environment.SetEnvironmentVariable("Path",
    Environment.GetEnvironmentVariable("Path") + ";" + Path.Combine(__SOURCE_DIRECTORY__,@"..\App1\App1.Windows\bin\x64\Debug\AppX"))
printfn "Path: %s" (Environment.GetEnvironmentVariable("Path"))


//let localFolder = ApplicationData.Current.LocalFolder;
//let file = await localFolder.CreateFileAsync("test.db", CreationCollisionOption.OpenIfExists);
//let filePath = file.Path;
let platform = new SQLitePlatformWinRT()
let connection = Sql.connect platform (__SOURCE_DIRECTORY__ + "\\test.db")
let result = Sql.initTables connection


match result with
    | Some (task : Task<SQLiteAsyncConnection.CreateTablesResult>) ->
        let cwd = __SOURCE_DIRECTORY__ + "\\test.db"
        printfn "Database should be in %s" cwd 
        let created : SQLiteAsyncConnection.CreateTablesResult = task |> Async.AwaitTask |> Async.RunSynchronously
        let results = created.Results.Keys
        printfn "Database should be in %s" (results.ToString())
        //let created : Async<SQLiteAsyncConnection.CreateTablesResult> = Async.AwaitTask <| task
        //let created : SQLiteAsyncConnection.CreateTablesResult = task.Start() |> Async.AwaitTask
        //created.Result
        
        printfn "I waited for the task like a gentleman"
    | None ->
        printfn "There is none"
        ()