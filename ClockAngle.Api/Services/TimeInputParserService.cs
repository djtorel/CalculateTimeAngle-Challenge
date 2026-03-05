using ClockAngle.Api.Models;

namespace ClockAngle.Api.Services;

public class TimeInputParserService : ITimeInputParserService
{
    public TimeParseResult ParseTimeInput(string? time, int? hour, int? minute)
    {
        return (time, hour, minute) switch
        {
            // Check if time parameter was entered first
            (not null, _, _) => ParseFromString(time),

            // If at least one of hour/minute is provided,
            // let the parser figure out errors or missing fields
            (null, not null, _) or (null, _, not null) => ParseFromParts(hour, minute),
            _ => new TimeParseResult.Failure(
                "Provide either 'time=HH:mm' or both 'hour' and 'minute' query parameters.")
        };
    }

    public TimeParseResult ParseFromString(string time)
    {
        if (string.IsNullOrEmpty(time))
            return new TimeParseResult.Failure("The 'time' parameter cannot be null or empty.");

        var parts = time.Split(':');

        if (parts.Length != 2
            || parts[0].Length != 2
            || parts[1].Length != 2
            || !int.TryParse(parts[0], out var hour)
            || !int.TryParse(parts[1], out var minute))
            return new TimeParseResult.Failure(
                $"Invalid time format '{time}'. Expected strict HH:mm with exactly 2 digits each (e.g. '03:00' or '14:30').");

        return ValidateTimeInput(hour, minute);
    }

    public TimeParseResult ParseFromParts(int? hour, int? minute)
    {
        return (hour, minute) switch
        {
            (null, null) => new TimeParseResult.Failure(
                "Both 'hour' and 'minute' parameters are required when not using 'time'."),
            (null, _) => new TimeParseResult.Failure("The 'hour' parameter is required."),
            (_, null) => new TimeParseResult.Failure("The 'minute' parameter is required."),
            ({ } h, { } m) => ValidateTimeInput(h, m)
        };
    }

    private static TimeParseResult ValidateTimeInput(int hour, int minute)
    {
        return (hour, minute) switch
        {
            (< 0 or > 23, _) => new TimeParseResult.Failure(
                $"Hour must be between 0 and 23, but got {hour}."),
            (_, < 0 or > 59) => new TimeParseResult.Failure(
                $"Minute must be between 0 and 59, but got {minute}."),
            _ => new TimeParseResult.Success(new TimeInput(hour, minute))
        };
    }
}