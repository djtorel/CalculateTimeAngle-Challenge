using ClockAngle.Api.Core;
using ClockAngle.Api.Domain.Errors;
using ClockAngle.Api.Models;
using static ClockAngle.Api.Common.Enums.TimeStatus;

namespace ClockAngle.Api.Services;

public class TimeInputParser : ITimeInputParser
{
    public Result<TimeInput> ParseTimeInput(string? time, int? hour, int? minute)
    {
        var (timeStatus, hourStatus, minuteStatus) = GetTimeStatuses(time, hour, minute);
        return (timeStatus, hourStatus, minuteStatus) switch
        {
            // Check if time parameter was entered first
            (Time.HasValue, _, _) => ParseFromString(time),

            // If at least one of hour/minute is provided,
            // let the parser figure out errors or missing fields
            (Time.IsEmpty, Hour.HasValue, _)
                or (Time.IsEmpty, _, Minute.HasValue) => ParseFromParts(hour, minute),

            _ => Result<TimeInput>.Failure(new NoParametersProvidedError())
        };
    }

    public Result<TimeInput> ParseFromString(string? time)
    {
        return EnsureNotEmpty(time)
            .Bind(SplitOnColon)
            .Bind(ValidateSegmentLengths)
            .Bind(ParseSegmentDigits)
            .Bind(t => ValidateTimeInput(t.Hour, t.Minute));
    }


    public Result<TimeInput> ParseFromParts(int? hour, int? minute)
    {
        var (_, hourStatus, minuteStatus) = GetTimeStatuses(null, hour, minute);
        return (hourStatus, minuteStatus) switch
        {
            (Hour.IsEmpty, Minute.IsEmpty) => Result<TimeInput>.Failure(new BothParamsMissingError()),
            (Hour.IsEmpty, _) => Result<TimeInput>.Failure(new HourParamMissingError()),
            (_, Minute.IsEmpty) => Result<TimeInput>.Failure(new MinuteParamMissingError()),
            _ => ValidateTimeInput(hour!.Value, minute!.Value)
        };
    }

    private static Result<TimeInput> ValidateTimeInput(int hour, int minute)
    {
        return (hour, minute) switch
        {
            (< 0 or > 23, _) => Result<TimeInput>.Failure(new HourOutOfRangeError(hour)),
            (_, < 0 or > 59) => Result<TimeInput>.Failure(new MinuteOutOfRangeError(minute)),
            _ => Result<TimeInput>.Success(new TimeInput(hour, minute))
        };
    }

    private static Result<string> EnsureNotEmpty(string? raw)
    {
        return string.IsNullOrEmpty(raw)
            ? Result<string>.Failure(new TimeParameterEmptyError())
            : Result<string>.Success(raw);
    }

    private static Result<string[]> SplitOnColon(string raw)
    {
        var parts = raw.Split(':');
        return parts.Length == 2
            ? Result<string[]>.Success(parts)
            : Result<string[]>.Failure(new TimeFormatInvalidError(raw));
    }

    private static Result<string[]> ValidateSegmentLengths(string[] parts)
    {
        return parts[0].Length == 2 && parts[1].Length == 2
            ? Result<string[]>.Success(parts)
            : Result<string[]>.Failure(new TimeSegmentLengthError(string.Join(":", parts)));
    }

    private static Result<(int Hour, int Minute)> ParseSegmentDigits(string[] parts)
    {
        return int.TryParse(parts[0], out var hour) && int.TryParse(parts[1], out var minute)
            ? Result<(int Hour, int Minute)>.Success((hour, minute))
            : Result<(int Hour, int Minute)>.Failure(new TimeSegmentLengthError(string.Join(":", parts)));
    }
}