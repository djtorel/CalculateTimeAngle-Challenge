using ClockAngle.Api.Core;

namespace ClockAngle.Api.Domain.Errors;

/// <summary>Base for all time-input validation errors.</summary>
public abstract record TimeValidationError(string Code, string Message, string Field)
    : Error(Code, Message);

/// <summary>The <c>time</c> query parameter was null or whitespace.</summary>
public sealed record TimeParameterEmptyError()
    : TimeValidationError(
        "ERR_TIME_EMPTY",
        "The 'time' parameter cannot be empty.",
        "time");

/// <summary>The <c>time</c> string did not contain exactly one colon separator.</summary>
public sealed record TimeFormatInvalidError(string RawValue)
    : TimeValidationError(
        "ERR_TIME_FORMAT",
        $"Invalid time format '{RawValue}'. Expected strict HH:mm with exactly 2 digits each (e.g. '03:00' or '14:30').",
        "time");

/// <summary>The <c>time</c> string had the correct structure but segment lengths were not exactly 2.</summary>
public sealed record TimeSegmentLengthError(string RawValue)
    : TimeValidationError(
        "ERR_TIME_SEGMENT_LENGTH",
        $"Invalid time format '{RawValue}'. Expected strict HH:mm with exactly 2 digits each (e.g. '03:00' or '14:30').",
        "time");

/// <summary>Neither <c>hour</c> nor <c>minute</c> were supplied when <c>time</c> was omitted.</summary>
public sealed record BothParamsMissingError()
    : TimeValidationError(
        "ERR_PARAMS_BOTH_MISSING",
        "Both 'hour' and 'minute' parameters are required when not using 'time'.",
        "both");

/// <summary>The <c>hour</c> parameter was not supplied.</summary>
public sealed record HourParamMissingError()
    : TimeValidationError(
        "ERR_HOUR_MISSING",
        "The 'hour' parameter is required.",
        "hour");

/// <summary>The <c>minute</c> parameter was not supplied.</summary>
public sealed record MinuteParamMissingError()
    : TimeValidationError(
        "ERR_MINUTE_MISSING",
        "The 'minute' parameter is required.",
        "minute");

/// <summary>The supplied hour value was outside the range 0–23.</summary>
public sealed record HourOutOfRangeError(int ActualHour)
    : TimeValidationError(
        "ERR_HOUR_RANGE",
        $"Hour must be between 0 and 23, but got {ActualHour}.",
        "hour");

/// <summary>The supplied minute value was outside the range 0–59.</summary>
public sealed record MinuteOutOfRangeError(int ActualMinute)
    : TimeValidationError(
        "ERR_MINUTE_RANGE",
        $"Minute must be between 0 and 59, but got {ActualMinute}.",
        "minute");

/// <summary>No query parameters at all were provided to the endpoint.</summary>
public sealed record NoParametersProvidedError()
    : TimeValidationError(
        "ERR_NO_PARAMS",
        "Provide either 'time=HH:mm' or both 'hour' and 'minute' query parameters.",
        "both");