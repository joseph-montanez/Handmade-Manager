namespace DataAccess

open SQLite.Net.Async
open SQLite.Net.Interop
open System.Threading.Tasks

module Sql =
    val connect : ISQLitePlatform -> string -> SQLiteAsyncConnection
    val initTables : SQLiteAsyncConnection -> Task<SQLiteAsyncConnection.CreateTablesResult> option