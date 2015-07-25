namespace DataAccess

open SQLite
open SQLite.Net
open SQLite.Net.Async
open SQLite.Net.Attributes
open SQLite.Net.Interop
open System
open System.Collections
open System.Threading.Tasks

module Sql =

    [<Table("Materials")>]
    type Material (id : int, name : string) = 
        let mutable m_id : int = id
        let mutable m_name : string = name
        
        [<Column("Id")>][<PrimaryKey>] [<AutoIncrement>]
        member this.Id
            with get() = m_id
            and set(value) = m_id <- value

        [<Column("Name")>]
        member this.Name
            with get() = m_name
            and set(value) = m_name <- value

        override this.ToString() = System.String.Format("[{0}] {1}", m_id, m_name)
        new() = Material(0, null)


    let connect (platform : SQLite.Net.Interop.ISQLitePlatform) (databasePath : string) : SQLiteAsyncConnection =
        let connectionString = SQLiteConnectionString(databasePath, false)
        let connectionFactory = new Func<SQLiteConnectionWithLock>(fun () -> new SQLiteConnectionWithLock(platform, connectionString));
        let connection = new SQLiteAsyncConnection(connectionFactory)
        connection

    let initTables (connection : SQLiteAsyncConnection) : Async<SQLiteAsyncConnection.CreateTablesResult> =
        connection.CreateTableAsync<Material>() |> Async.AwaitTask
