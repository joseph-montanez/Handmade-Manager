// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open System.Threading.Tasks
open SQLite.Net.Async
open SQLite.Net.Platform.Win32
open System
open System.IO
open System.Collections
open DataAccess



[<EntryPoint>]
let main argv = 
//    Environment.SetEnvironmentVariable("Path",
//        Environment.GetEnvironmentVariable("Path") + ";" + Path.Combine(__SOURCE_DIRECTORY__,@"..\App1\App1.Windows\bin\x64\Debug\AppX"))
//    printfn "Path: %s" (Environment.GetEnvironmentVariable("Path"))


    //let localFolder = ApplicationData.Current.LocalFolder;
    //let file = await localFolder.CreateFileAsync("test.db", CreationCollisionOption.OpenIfExists);
    //let filePath = file.Path;
    let platform = new SQLitePlatformWin32()
    let connection = Sql.connect platform (__SOURCE_DIRECTORY__ + "\\test.db")
//    let sql = """
//        CREATE TABLE Persons
//        (
//        PersonID int,
//        LastName varchar(255),
//        FirstName varchar(255),
//        Address varchar(255),
//        City varchar(255)
//        );
//    """
//    let sqlTask = connection.ExecuteAsync(sql)
//    let sqlResult : int = sqlTask |> Async.AwaitTask |> Async.RunSynchronously
    let task = Sql.initTables connection
    let result : SQLiteAsyncConnection.CreateTablesResult = task |> Async.RunSynchronously


    printfn "%A" argv
    let name = Console.ReadLine()
    0 // return an integer exit code
