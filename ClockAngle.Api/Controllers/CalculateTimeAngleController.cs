using System.Diagnostics;
using ClockAngle.Api.Models;
using ClockAngle.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClockAngle.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CalculateTimeAngleController(
    ITimeAngleCalculatorService angleCalculatorService,
    ITimeInputParserService inputParserService) : ControllerBase
{
    [ProducesResponseType(typeof(TimeAngleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public IActionResult Get(
        [FromQuery] string? time = null,
        [FromQuery] int? hour = null,
        [FromQuery] int? minute = null)
    {
        var result = inputParserService.ParseTimeInput(time, hour, minute);

        return result switch
        {
            TimeParseResult.Success(var input) => Ok(angleCalculatorService.GetTimeAngle(input)),
            TimeParseResult.Failure(var message) => BadRequest(new ProblemDetails
            {
                Title = "Invalid time input",
                Detail = message,
                Status = StatusCodes.Status400BadRequest
            }),
            _ => throw new UnreachableException("Unexpected TimeParseResult subtype")
        };
    }
}