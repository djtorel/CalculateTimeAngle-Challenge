using ClockAngle.Api.Core;
using ClockAngle.Api.Domain.Errors;
using ClockAngle.Api.Models;
using static ClockAngle.Api.Common.Enums.TimeStatus;

namespace ClockAngle.Api.Services;

/// <summary>
///     Concrete implementation of <see cref="ITimeInputParser"/>.
///     Dispatches to a string-based or integer-parts-based parse path depending on
///     which query parameters are present, then validates ranges via a shared helper.
/// </summary>
public class TimeInputParser : ITimeInputParser
{
    /// <inheritdoc/>
    public Result<TimeInput> ParseTimeInput(string? time, int? hour, int? minute)
    {
        var (timeStatus, hourStatus, minuteStatus) = GetTimeStatuses(time, hour, minute);
        return (timeStatus, hourStatus, minuteStatus) switch
        {
            // Reject mixed input types. The two input conventions are mutually exclusive.
            (Time.HasValue, Hour.HasValue, _) or (Time.HasValue, _, Minute.HasValue)
                => Result<TimeInput>.Failure(new AmbiguousParametersError()),

            // Check if time parameter was entered alone
            (Time.HasValue, Hour.IsEmpty, Minute.IsEmpty) => ParseFromString(time),

            // If at least one of hour/minute is provided,
            // let the parser figure out errors or missing fields
            (Time.IsEmpty, Hour.HasValue, _)
                or (Time.IsEmpty, _, Minute.HasValue) => ParseFromParts(hour, minute),

            _ => Result<TimeInput>.Failure(new NoParametersProvidedError())
        };
    }

    /// <inheritdoc/>
    public Result<TimeInput> ParseFromString(string? time)
    {
        return EnsureNotEmpty(time)
            .Bind(SplitOnColon)
            .Bind(ValidateSegmentLengths)
            .Bind(ParseSegmentDigits)
            .Bind(t => ValidateTimeInput(t.Hour, t.Minute));
    }


    /// <inheritdoc/>
    public Result<TimeInput> ParseFromParts(int? hour, int? minute)
    {
        var (_, hourStatus, minuteStatus) = GetTimeStatuses(null, hour, minute);
        return (hourStatus, minuteStatus) switch
        {
            // Check if both time and minute are empty and produce error if so.
            (Hour.IsEmpty, Minute.IsEmpty) => Result<TimeInput>.Failure(new BothParamsMissingError()),

            // If Hour is missing produce a relevant error
            (Hour.IsEmpty, _) => Result<TimeInput>.Failure(new HourParamMissingError()),

            // If Minute is empty then throw a relevant error
            (_, Minute.IsEmpty) => Result<TimeInput>.Failure(new MinuteParamMissingError()),

            // Everything should be good, validate inputs.
            _ => ValidateTimeInput(hour!.Value, minute!.Value)
        };
    }

    /// <summary>
    ///     Validates that <paramref name="hour"/> is 0–23 and <paramref name="minute"/> is 0–59,
    ///     returning a successful <see cref="TimeInput"/> or a typed range error.
    /// </summary>
    private static Result<TimeInput> ValidateTimeInput(int hour, int minute)
    {
        return (hour, minute) switch
        {
            // Hour should be between 0 and 23
            (< 0 or > 23, _) => Result<TimeInput>.Failure(new HourOutOfRangeError(hour)),

            // Minute should be between 0 and 59
            (_, < 0 or > 59) => Result<TimeInput>.Failure(new MinuteOutOfRangeError(minute)),

            // Everything should be good, send success
            _ => Result<TimeInput>.Success(new TimeInput(hour, minute))
        };
    }

    /// <summary>
    ///     Guards against a null or whitespace <c>time</c> string before further parsing begins.
    /// </summary>
    private static Result<string> EnsureNotEmpty(string? raw)
    {
        return string.IsNullOrEmpty(raw)
            ? Result<string>.Failure(new TimeParameterEmptyError())
            : Result<string>.Success(raw);
    }

    /// <summary>
    ///     Splits the raw string on <c>':'</c> and requires exactly two segments (HH and mm).
    /// </summary>
    private static Result<string[]> SplitOnColon(string raw)
    {
        var parts = raw.Split(':');
        return parts.Length == 2
            ? Result<string[]>.Success(parts)
            : Result<string[]>.Failure(new TimeFormatInvalidError(raw));
    }

    /// <summary>
    ///     Enforces strict two-digit formatting: each segment must be exactly 2 characters long
    ///     (e.g. <c>"03"</c> not <c>"3"</c>).
    /// </summary>
    private static Result<string[]> ValidateSegmentLengths(string[] parts)
    {
        return parts[0].Length == 2 && parts[1].Length == 2
            ? Result<string[]>.Success(parts)
            : Result<string[]>.Failure(new TimeSegmentLengthError(string.Join(":", parts)));
    }

    /// <summary>
    ///     Attempts to parse both segments as integers. Non-numeric characters (e.g. letters)
    ///     that pass the length check are caught here and treated as a segment-length error.
    /// </summary>
    private static Result<(int Hour, int Minute)> ParseSegmentDigits(string[] parts)
    {
        return int.TryParse(parts[0], out var hour) && int.TryParse(parts[1], out var minute)
            ? Result<(int Hour, int Minute)>.Success((hour, minute))
            : Result<(int Hour, int Minute)>.Failure(new TimeSegmentLengthError(string.Join(":", parts)));
    }
}