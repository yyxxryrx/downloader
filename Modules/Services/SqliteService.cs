using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Downloader.Modules.Models;
using Microsoft.Data.Sqlite;
using Serilog;

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
            DateTime DATETIME NOT NULL,
            FileSize INTEGER NOT NULL,
            Status TEXT NOT NULL DEFAULT 0
    );";
        using var command = Connection.CreateCommand();
        command.CommandText = createTable;
        command.ExecuteNonQuery();
    }

    public async Task<long> Add(string filename, string url, string browser, long fileSize, DateTime datetime)
    {
        try
        {
            await using var command = Connection.CreateCommand();
            command.CommandText =
                "INSERT INTO DownloadHistory(Filename, Url, Browser, DateTime, FileSize) VALUES(@filename, @url, @browser, @datetime, @filesize);";
            command.Parameters.AddWithValue("@filename", filename);
            command.Parameters.AddWithValue("@url", url);
            command.Parameters.AddWithValue("@browser", browser);
            command.Parameters.AddWithValue("@filesize", fileSize);
            command.Parameters.AddWithValue("@datetime", datetime);
            await command.ExecuteNonQueryAsync();
            Log.Information("Added {Filename} to download history", filename);
            command.CommandText = "SELECT seq FROM sqlite_sequence WHERE name = 'DownloadHistory'";
            var reader = await command.ExecuteScalarAsync();
            return Convert.ToInt64(reader);
        }
        catch (Exception e)
        {
            Log.Error(e, "Error adding {Filename} to download history", filename);
            return -1;
        }
    }

    public async Task<List<HistoryEntry>> GetAll()
    {
        await using var command = Connection.CreateCommand();
        command.CommandText = "SELECT * FROM DownloadHistory ORDER BY DateTime DESC;";
        await using var reader = await command.ExecuteReaderAsync();
        var result = new List<HistoryEntry>();
        while (await reader.ReadAsync())
        {
            var entry = new HistoryEntry
            {
                Id = reader.GetInt32(0),
                Filename = reader.GetString(1),
                Url = reader.GetString(2),
                Browser = reader.GetString(3),
                DateTime = reader.GetDateTime(4),
                FileSize = reader.GetInt64(5),
                Status = Enum.Parse<DownloadStatus>(reader.GetString(6))
            };
            result.Add(entry);
        }

        return result;
    }
}