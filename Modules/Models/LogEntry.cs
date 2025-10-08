using Serilog.Events;

namespace Downloader.Modules.Models;

public class LogEntry
{
    public required LogEventLevel Level { get; init; }
    public required string Message { get; init; }
    public required string Timestamp { get; init; }
}