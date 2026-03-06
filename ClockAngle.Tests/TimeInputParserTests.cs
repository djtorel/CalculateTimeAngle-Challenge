using ClockAngle.Api.Core;
using ClockAngle.Api.Domain.Errors;
using ClockAngle.Api.Models;
using ClockAngle.Api.Services;

namespace ClockAngle.Tests;

/// <summary>
///     Unit tests for <see cref="TimeInputParser" />, covering the dispatch logic in
///     <see cref="TimeInputParser.ParseTimeInput" /> and the individual parse paths.
/// </summary>
public class TimeInputParserTests
{
    private readonly TimeInputParser _parser = new();

    // ── ParseTimeInput routing ──

    [Fact]
    public void ParseTimeInput_WithTimeString_RoutesToStringParser()
    {
        // A format error (ERR_TIME_FORMAT) can only originate from the string-parse path.
        var result = _parser.ParseTimeInput("bad", null, null);

        var err = Assert.IsType<Result<TimeInput>.Err>(result);
        Assert.Equal("ERR_TIME_FORMAT", err.Error.Code);
    }

    [Fact]
    public void ParseTimeInput_WithHourAndMinute_RoutesToPartsParser()
    {
        // A minute-missing error (ERR_MINUTE_MISSING) can only originate from the parts-parse path.
        var result = _parser.ParseTimeInput(null, 3, null);

        var err = Assert.IsType<Result<TimeInput>.Err>(result);
        Assert.Equal("ERR_MINUTE_MISSING", err.Error.Code);
    }

    [Fact]
    public void ParseTimeInput_WithOnlyHour_RoutesToPartsParser()
    {
        var result = _parser.ParseTimeInput(null, 5, null);

        var err = Assert.IsType<Result<TimeInput>.Err>(result);
        Assert.Equal("ERR_MINUTE_MISSING", err.Error.Code);
    }

    [Fact]
    public void ParseTimeInput_WithNoParams_ReturnsNoParametersError()
    {
        var result = _parser.ParseTimeInput(null, null, null);

        var err = Assert.IsType<Result<TimeInput>.Err>(result);
        Assert.Equal("ERR_NO_PARAMS", err.Error.Code);
    }

    [Fact]
    public void ParseTimeInput_WithAmbiguousParams_ReturnsAmbiguousError()
    {
        var result = _parser.ParseTimeInput("03:00", 3, null);

        var err = Assert.IsType<Result<TimeInput>.Err>(result);
        Assert.Equal("ERR_AMBIGUOUS_PARAMS", err.Error.Code);
    }
}
