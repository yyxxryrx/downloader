using System.IO;
using Windows.Storage;
using Microsoft.Data.Sqlite;

namespace Downloader.Modules.Services;

public class SqliteService
{
    public SqliteConnection Connection { get; } = new(new SqliteConnectionStringBuilder
    {
        DataSource = Path.Join(new DirectoryInfo(ApplicationData.Current.LocalFolder.Path).FullName, "database.db"),
        Mode = SqliteOpenMode.ReadWriteCreate,
        ForeignKeys = true,
    }.ToString());

    public SqliteService()
    {
        Connection.Open();
        const string createTable = @"CREATE TABLE IF NOT EXISTS DownloadHistory(
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Filename TEXT NOT NULL,
            Url TEXT NOT NULL,
            Browser TEXT,
            DateTime INTEGER NOT NULL,
            FileSize INTEGER NOT NULL,
            Status INTEGER NOT NULL DEFAULT 0,
    );";
        using var command = Connection.CreateCommand();
        command.CommandText = createTable;
        command.ExecuteNonQuery();
    }
}