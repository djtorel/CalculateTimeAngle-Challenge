namespace ClockAngle.Api.Models;

/// <summary>
///     The computed clock-hand angles returned by the API for a given time.
///     All angles are in degrees, measured clockwise from the 12 o'clock position.
/// </summary>
public sealed record TimeAngleResponse(
    double HourHandAngle,
    double MinuteHandAngle,
    double TotalAngle);