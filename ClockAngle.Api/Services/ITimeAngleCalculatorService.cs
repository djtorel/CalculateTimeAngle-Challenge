using ClockAngle.Api.Models;

namespace ClockAngle.Api.Services;

/// <summary>
///     Computes the angular positions of the hour and minute clock hands for a given time.
///     All angles are in degrees, measured clockwise from the 12 o'clock position.
/// </summary>
public interface ITimeAngleCalculatorService
{
    /// <summary>
    ///     Calculates the hour-hand angle, minute-hand angle, and their sum for the supplied time.
    /// </summary>
    /// <param name="timeInput">A validated time value (hour 0–23, minute 0–59).</param>
    /// <returns>
    ///     A <see cref="TimeAngleResponse"/> containing the individual hand angles and their sum.
    /// </returns>
    TimeAngleResponse GetTimeAngle(TimeInput timeInput);
}