using ClockAngle.Api.Controllers;
using ClockAngle.Api.Core;
using ClockAngle.Api.Domain.Errors;
using ClockAngle.Api.Models;
using ClockAngle.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ClockAngle.Tests;

/// <summary>
///     Unit tests for <see cref="CalculateTimeAngleController" />.
///     These tests treat <see cref="ITimeInputParser" /> as a black box and mock
///     <see cref="ITimeInputParser.ParseTimeInput" /> — the only method the controller calls.
///     Routing logic inside ParseTimeInput is covered by <see cref="TimeInputParserTests" />.
/// </summary>
public class CalculateTimeAngleControllerTests
{
    private static readonly TimeInput ValidInput = new(3, 0);
    private static readonly TimeAngleResponse ValidResponse = new(90.0, 0.0, 90.0);
    private readonly CalculateTimeAngleController _calculateTimeAngleController;
    private readonly Mock<ITimeInputParser> _parserMock = new();
    private readonly Mock<ITimeAngleCalculatorService> _timeAngleCalculatorMock = new();

    public CalculateTimeAngleControllerTests()
    {
        _calculateTimeAngleController =
            new CalculateTimeAngleController(_timeAngleCalculatorMock.Object, _parserMock.Object);
    }

    // ── Successful parse yields 200 OK ──

    [Fact]
    public void Calculate_ParseSuccess_Returns200WithResponse()
    {
        _parserMock
            .Setup(p => p.ParseTimeInput("03:00", null, null))
            .Returns(Result<TimeInput>.Success(ValidInput));
        _timeAngleCalculatorMock
            .Setup(c => c.GetTimeAngle(ValidInput))
            .Returns(ValidResponse);

        var actionResult = _calculateTimeAngleController.Get("03:00");

        var ok = Assert.IsType<OkObjectResult>(actionResult);
        Assert.Equal(StatusCodes.Status200OK, ok.StatusCode);
        Assert.Equal(ValidResponse, ok.Value);
    }

    // ── Failed parse yields 400 Bad Request ──
    [Fact]
    public void Calculate_ParseFailure_Returns400WithProblemDetails()
    {
        _parserMock
            .Setup(p => p.ParseTimeInput("99:00", null, null))
            .Returns(Result<TimeInput>.Failure(new HourOutOfRangeError(99)));

        var actionResult = _calculateTimeAngleController.Get("99:00");

        var badRequest = Assert.IsType<BadRequestObjectResult>(actionResult);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequest.StatusCode);

        var problem = Assert.IsType<ProblemDetails>(badRequest.Value);
        Assert.Equal(StatusCodes.Status400BadRequest, problem.Status);
        Assert.Contains("99", problem.Detail);
        Assert.False(string.IsNullOrWhiteSpace(problem.Detail));
    }

    // ── Typed error code flows through to ProblemDetails.Extensions ──

    [Fact]
    public void Calculate_ParseFailure_ProblemDetailsCarriesTypedErrorCode()
    {
        _parserMock
            .Setup(p => p.ParseTimeInput("99:00", null, null))
            .Returns(Result<TimeInput>.Failure(new HourOutOfRangeError(99)));

        var actionResult = _calculateTimeAngleController.Get("99:00");

        var problem = Assert.IsType<ProblemDetails>(
            Assert.IsType<BadRequestObjectResult>(actionResult).Value);

        Assert.True(problem.Extensions.ContainsKey("errorCode"));
        Assert.Equal("ERR_HOUR_RANGE", problem.Extensions["errorCode"]?.ToString());
    }

    // ── Calculator is NOT called when parsing fails ──

    [Fact]
    public void Calculate_ParseFailure_DoesNotCallCalculator()
    {
        _parserMock
            .Setup(p => p.ParseTimeInput("bad", null, null))
            .Returns(Result<TimeInput>.Failure(new TimeFormatInvalidError("bad")));

        _calculateTimeAngleController.Get("bad");

        _timeAngleCalculatorMock.Verify(c => c.GetTimeAngle(It.IsAny<TimeInput>()), Times.Never);
    }

    // ── No parameters at all yields 400 ──

    [Fact]
    public void Calculate_NoParameters_Returns400()
    {
        _parserMock
            .Setup(p => p.ParseTimeInput(null, null, null))
            .Returns(Result<TimeInput>.Failure(new NoParametersProvidedError()));

        var actionResult = _calculateTimeAngleController.Get();

        Assert.IsType<BadRequestObjectResult>(actionResult);
    }

    // ── Mixed input is rejected ──

    [Fact]
    public void Calculate_TimeWithHour_Returns400WithAmbiguousParamsError()
    {
        _parserMock
            .Setup(p => p.ParseTimeInput("03:00", 3, null))
            .Returns(Result<TimeInput>.Failure(new AmbiguousParametersError()));

        var actionResult = _calculateTimeAngleController.Get("03:00", 3);

        var badRequest = Assert.IsType<BadRequestObjectResult>(actionResult);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequest.StatusCode);
        var problem = Assert.IsType<ProblemDetails>(badRequest.Value);
        Assert.Equal("ERR_AMBIGUOUS_PARAMS", problem.Extensions["errorCode"]?.ToString());
    }

    [Fact]
    public void Calculate_TimeWithMinute_Returns400WithAmbiguousParamsError()
    {
        _parserMock
            .Setup(p => p.ParseTimeInput("03:00", null, 0))
            .Returns(Result<TimeInput>.Failure(new AmbiguousParametersError()));

        var actionResult = _calculateTimeAngleController.Get("03:00", minute: 0);

        var badRequest = Assert.IsType<BadRequestObjectResult>(actionResult);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequest.StatusCode);
        var problem = Assert.IsType<ProblemDetails>(badRequest.Value);
        Assert.Equal("ERR_AMBIGUOUS_PARAMS", problem.Extensions["errorCode"]?.ToString());
    }

    [Fact]
    public void Calculate_TimeWithHourAndMinute_Returns400WithAmbiguousParamsError()
    {
        _parserMock
            .Setup(p => p.ParseTimeInput("03:00", 3, 0))
            .Returns(Result<TimeInput>.Failure(new AmbiguousParametersError()));

        var actionResult = _calculateTimeAngleController.Get("03:00", 3, 0);

        var badRequest = Assert.IsType<BadRequestObjectResult>(actionResult);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequest.StatusCode);
        var problem = Assert.IsType<ProblemDetails>(badRequest.Value);
        Assert.Equal("ERR_AMBIGUOUS_PARAMS", problem.Extensions["errorCode"]?.ToString());
    }
}
