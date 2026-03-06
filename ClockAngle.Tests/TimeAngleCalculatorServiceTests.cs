using ClockAngle.Api.Models;
using ClockAngle.Api.Services;

namespace ClockAngle.Tests;

/// <summary>
///     Unit tests for <see cref="TimeAngleCalculatorService"/>.
///     Verifies correct hour-hand angles (including 24-hour-to-12-hour modulo mapping and
///     0.5°/min creep), minute-hand angles, and the total-angle sum across boundary and
///     representative mid-range cases.
/// </summary>
public sealed class TimeAngleCalculatorServiceTests
{
    private readonly ITimeAngleCalculatorService _timeAngleCalculatorService = new TimeAngleCalculatorService();

    // ── Hour hand tests ──
    [Theory]
    [InlineData(12, 0, 0.0)] // 12:00 — both hands at 12
    [InlineData(0, 0, 0.0)] // 00:00 — same position as 12:00
    [InlineData(3, 0, 90.0)] // 3:00  — quarter past 12
    [InlineData(6, 0, 180.0)] // 6:00  — half-way
    [InlineData(9, 0, 270.0)] // 9:00  — three-quarters
    [InlineData(15, 0, 90.0)] // 15:00 — 24h clock, same as 3:00
    [InlineData(23, 0, 330.0)] // 23:00 — same as 11:00
    public void Calculate_HourHandAngle_IsCorrect(int hour, int minute, double expected)
    {
        var result = _timeAngleCalculatorService.GetTimeAngle(new TimeInput(hour, minute));
        Assert.Equal(expected, result.HourHandAngle);
    }

    // ── Minute hand tests ──
    [Theory]
    [InlineData(0, 0.0)] // On the 12
    [InlineData(15, 90.0)] // Quarter past
    [InlineData(30, 180.0)] // Half past
    [InlineData(45, 270.0)] // Quarter to
    [InlineData(59, 354.0)] // One minute before the hour
    public void Calculate_MinuteHandAngle_IsCorrect(int minute, double expected)
    {
        var result = _timeAngleCalculatorService.GetTimeAngle(new TimeInput(0, minute));
        Assert.Equal(expected, result.MinuteHandAngle);
    }

    // ── Hour hand creep (0.5 degrees per minute) ──
    [Fact]
    public void Calculate_HourHand_AdvancesHalfDegreePerMinute()
    {
        var at3_00 = _timeAngleCalculatorService.GetTimeAngle(new TimeInput(3, 0));
        var at3_30 = _timeAngleCalculatorService.GetTimeAngle(new TimeInput(3, 30));
        var at3_60 = _timeAngleCalculatorService.GetTimeAngle(new TimeInput(4, 0));

        Assert.Equal(90.0, at3_00.HourHandAngle);
        Assert.Equal(105.0, at3_30.HourHandAngle); // 90 + 30*0.5
        Assert.Equal(120.0, at3_60.HourHandAngle); // 90 + 60*0.5
    }

    // ── Total angle (sum) ─────────────────────────────────────────────────────

    [Theory]
    [InlineData(12, 0, 0.0)] // 0  + 0   = 0
    [InlineData(3, 0, 90.0)] // 90 + 0   = 90   (the spec example)
    [InlineData(3, 30, 285.0)] // 105 + 180 = 285
    [InlineData(6, 0, 180.0)] // 180 + 0  = 180
    [InlineData(11, 59, 713.5)] // 359.5 + 354 = 713.5  (edge case near midnight)
    [InlineData(0, 0, 0.0)] // midnight
    public void Calculate_TotalAngle_IsCorrect(int hour, int minute, double expected)
    {
        var result = _timeAngleCalculatorService.GetTimeAngle(new TimeInput(hour, minute));
        Assert.Equal(expected, result.TotalAngle);
    }

    // ── Total = HourHandAngle + MinuteHandAngle ───────────────────────────────

    [Theory]
    [InlineData(1, 15)]
    [InlineData(10, 48)]
    [InlineData(23, 59)]
    public void Calculate_TotalAngle_EqualsHourPlusMinuteAngle(int hour, int minute)
    {
        var result = _timeAngleCalculatorService.GetTimeAngle(new TimeInput(hour, minute));
        Assert.Equal(result.HourHandAngle + result.MinuteHandAngle, result.TotalAngle);
    }
}