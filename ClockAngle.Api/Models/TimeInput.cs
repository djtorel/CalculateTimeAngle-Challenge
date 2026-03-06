namespace ClockAngle.Api.Models;

/// <summary>
///     A validated time value used as input to <see cref="ClockAngle.Api.Services.ITimeAngleCalculatorService"/>.
///     Instances are only created after all parsing and range validation has passed.
/// </summary>
/// <param name="Hour">Hour component on a 24-hour clock (0–23).</param>
/// <param name="Minute">Minute component (0–59).</param>
public sealed record TimeInput(int Hour, int Minute);