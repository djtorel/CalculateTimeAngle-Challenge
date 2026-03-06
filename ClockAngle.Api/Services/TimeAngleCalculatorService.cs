using ClockAngle.Api.Models;

namespace ClockAngle.Api.Services;

/// <summary>
///     Pure, stateless implementation of <see cref="ITimeAngleCalculatorService"/>.
///     Computes clock-hand positions using standard analogue-clock geometry;
///     no I/O or external dependencies.
/// </summary>
public class TimeAngleCalculatorService : ITimeAngleCalculatorService
{
    private const double DegreesPerHour = 30.0;
    private const double DegreesPerMinuteOnHourHand = 0.5;
    private const double DegreesPerMinute = 6.0;

    /// <inheritdoc/>
    public TimeAngleResponse GetTimeAngle(TimeInput timeInput)
    {
        return BuildResponse(timeInput, ComputeHourAngle(timeInput), ComputeMinuteAngle(timeInput));
    }

    /// <summary>
    ///     Calculates the hour-hand position in degrees clockwise from 12.
    ///     Formula: <c>(hour % 12) × 30 + minute × 0.5</c>
    /// </summary>
    /// <remarks>
    ///     The modulo 12 maps 24-hour input onto the 12-hour clock face (e.g. 15:00 → 3:00).
    ///     The 0.5°/min term accounts for the continuous movement of the hour hand between hour marks.
    /// </remarks>
    private static double ComputeHourAngle(TimeInput timeInput)
    {
        return timeInput.Hour % 12 * DegreesPerHour + timeInput.Minute * DegreesPerMinuteOnHourHand;
    }

    /// <summary>
    ///     Calculates the minute-hand position in degrees clockwise from 12.
    ///     Formula: <c>minute × 6</c>
    /// </summary>
    private static double ComputeMinuteAngle(TimeInput timeInput)
    {
        return timeInput.Minute * DegreesPerMinute;
    }

    /// <summary>
    ///     Assembles the final <see cref="TimeAngleResponse"/> from the pre-computed hand angles.
    ///     <c>TotalAngle</c> is the arithmetic sum of the two hand angles.
    /// </summary>
    private static TimeAngleResponse BuildResponse(
        TimeInput timeInput,
        double hourAngle,
        double minuteAngle)
    {
        return new TimeAngleResponse(
            hourAngle,
            minuteAngle,
            hourAngle + minuteAngle);
    }
}