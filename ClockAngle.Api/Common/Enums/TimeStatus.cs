namespace ClockAngle.Api.Common.Enums;

/// <summary>
///     Provides per-parameter presence enums and a helper for classifying which
///     query parameters were supplied to the endpoint. The resulting tuple is used
///     by <see cref="ClockAngle.Api.Services.TimeInputParser"/> to dispatch to the
///     correct parse path without scattered null checks.
/// </summary>
public static class TimeStatus
{
    /// <summary>Indicates whether the <c>hour</c> query parameter was supplied.</summary>
    public enum Hour
    {
        IsEmpty,
        HasValue
    }

    /// <summary>Indicates whether the <c>minute</c> query parameter was supplied.</summary>
    public enum Minute
    {
        IsEmpty,
        HasValue
    }

    /// <summary>Indicates whether the <c>time</c> query parameter was supplied.</summary>
    public enum Time
    {
        IsEmpty,
        HasValue
    }

    /// <summary>
    ///     Maps the three nullable query-parameter values to their corresponding presence statuses.
    /// </summary>
    /// <param name="time">Raw value of the <c>time</c> query parameter, or <c>null</c> if absent.</param>
    /// <param name="hour">Raw value of the <c>hour</c> query parameter, or <c>null</c> if absent.</param>
    /// <param name="minute">Raw value of the <c>minute</c> query parameter, or <c>null</c> if absent.</param>
    /// <returns>
    ///     A tuple of (<see cref="Time"/>, <see cref="Hour"/>, <see cref="Minute"/>) statuses
    ///     that can be pattern-matched to determine the correct parse path.
    /// </returns>
    public static (Time, Hour, Minute) GetTimeStatuses(
        string? time,
        int? hour,
        int? minute)
    {
        return (
            string.IsNullOrEmpty(time) ? Time.IsEmpty : Time.HasValue,
            hour.HasValue ? Hour.HasValue : Hour.IsEmpty,
            minute.HasValue ? Minute.HasValue : Minute.IsEmpty
        );
    }
}