using Serilog.Events;

namespace Downloader.Modules.Models;

public class LogEntry
{
    public required LogEventLevel Level { get; init; } = LogEventLevel.Information;
    public required string Message { get; init; } = "Default message";
    public required string Timestamp { get; init; } = "--:--:--";

    public override string ToString()
    {
        return $"[{Timestamp}] [{Level.ToString()}] {Message}";
    }
}