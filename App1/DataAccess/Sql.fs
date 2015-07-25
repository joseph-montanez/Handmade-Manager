namespace DataAccess

open SQLite
open SQLite.Net
open SQLite.Net.Async
open SQLite.Net.Attributes
open System
open System.Threading.Tasks

module Sql =
    [<Table(name="Materials")>]
    type Material (id : int, name : string) = 
        let mutable m_id : int = id
        let mutable m_name : string = name

        new() = Material(0, null)
        
        [<Column(name="Id")>] [<PrimaryKey>] [<AutoIncrement>]
        member this.Id
            with get() = m_id
            and set(value) = m_id <- value

        [<Column(name="Name")>]
        member this.Name
            with get() = m_name
            and set(value) = m_name <- value

        override this.ToString() = System.String.Format("[{0}] {1}", m_id, m_name)

    let connect (platform : SQLite.Net.Interop.ISQLitePlatform) (databasePath : string) : SQLiteAsyncConnection =
        let connectionString = SQLiteConnectionString(databasePath, false)
        let connectionFactory = new Func<SQLiteConnectionWithLock>(fun () -> new SQLiteConnectionWithLock(platform, connectionString));
        let connection = new SQLiteAsyncConnection(connectionFactory)
        connection

    let initTables (connection : SQLiteAsyncConnection) : Task<SQLiteAsyncConnection.CreateTablesResult> option =
        try
            let result = connection.CreateTableAsync<Material>()
            Some(result)
        with
            | :? SQLite.Net.SQLiteException->
            None
