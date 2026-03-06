using ClockAngle.Api.Core;
using ClockAngle.Api.Models;

namespace ClockAngle.Api.Services;

/// <summary>
///     Parses and validates raw query-parameter values into a <see cref="TimeInput"/>.
///     The endpoint supports two mutually exclusive input styles:
///     a combined <c>time</c> string (<c>HH:mm</c>) or separate <c>hour</c> / <c>minute</c> integers.
///     On failure a typed <see cref="ClockAngle.Api.Core.Error"/> is returned rather than throwing.
/// </summary>
public interface ITimeInputParser
{
    /// <summary>
    ///     Entry point that dispatches to the appropriate parse path based on which parameters
    ///     are present. The two input styles are mutually exclusive; supplying both returns an
    ///     <see cref="ClockAngle.Api.Domain.Errors.AmbiguousParametersError"/>.
    ///     Supplying neither returns a <see cref="ClockAngle.Api.Domain.Errors.NoParametersProvidedError"/>.
    /// </summary>
    /// <param name="time">
    ///     Optional time string in <c>HH:mm</c> format (e.g. <c>"03:00"</c>).
    ///     When provided, <paramref name="hour"/> and <paramref name="minute"/> must be <c>null</c>.
    /// </param>
    /// <param name="hour">
    ///     Optional hour component (0–23). When provided, <paramref name="time"/> must be <c>null</c>
    ///     and <paramref name="minute"/> must also be provided.
    /// </param>
    /// <param name="minute">
    ///     Optional minute component (0–59). When provided, <paramref name="time"/> must be <c>null</c>
    ///     and <paramref name="hour"/> must also be provided.
    /// </param>
    /// <returns>
    ///     <see cref="ClockAngle.Api.Core.Result{T}.Ok"/> wrapping a validated <see cref="TimeInput"/>,
    ///     or <see cref="ClockAngle.Api.Core.Result{T}.Err"/> wrapping a typed validation error.
    /// </returns>
    Result<TimeInput> ParseTimeInput(string? time, int? hour, int? minute);

    /// <summary>
    ///     Parses a time string in strict <c>HH:mm</c> format (exactly two digits for each segment).
    ///     Validates that the resulting hour is 0–23 and minute is 0–59.
    /// </summary>
    /// <param name="time">The raw time string to parse (e.g. <c>"03:00"</c> or <c>"14:30"</c>).</param>
    Result<TimeInput> ParseFromString(string time);

    /// <summary>
    ///     Validates pre-parsed integer hour and minute values.
    ///     Both parameters are required; absence of either yields a typed missing-parameter error.
    ///     Present values must satisfy hour 0–23 and minute 0–59.
    /// </summary>
    /// <param name="hour">Optional hour component (0–23).</param>
    /// <param name="minute">Optional minute component (0–59).</param>
    Result<TimeInput> ParseFromParts(int? hour, int? minute);
}