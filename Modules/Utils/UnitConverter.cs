using System;

namespace Downloader.Modules.Utils;

public record UnitConvertResult(double Value, string Unit)
{
    public override string ToString()
    {
        return $"{Value} {Unit}";
    }
    
    public string ToStringF2()
    {
        return $"{Value:F2} {Unit}";
    }
};

public class UnitConverter
{
    public required long Step { get; init; }
    public required string[] Units { get; init; }

    public UnitConvertResult Convert(long value)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value);
        if (value == 0) return new UnitConvertResult(0, Units[0]);
        var level = Math.Min((int)Math.Log(value, Step), Units.Length - 1);
        return new UnitConvertResult(value / Math.Pow(Step, level), Units[level]);
    }
}

public static class FilesizeConverters
{
    public static UnitConverter iB = new UnitConverter
    {
        Step = 1024,
        Units = ["B", "KiB", "MiB", "GiB", "TiB", "PiB", "EiB", "ZiB", "YiB"]
    };

    public static UnitConverter B = new UnitConverter
    {
        Step = 1000,
        Units = ["B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"]
    };
}