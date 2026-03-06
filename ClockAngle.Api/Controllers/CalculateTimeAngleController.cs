using ClockAngle.Api.Models;
using ClockAngle.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClockAngle.Api.Controllers;

/// <summary>
///     Supports two call styles via a single endpoint:
///     GET /CalculateTimeAngle?time=03:00
///     GET /CalculateTimeAngle?hour=3&amp;minute=0
/// </summary>
[ApiController]
[Route("[controller]")]
public class CalculateTimeAngleController(
    ITimeAngleCalculatorService angleCalculatorService,
    ITimeInputParser inputParser) : ControllerBase
{
    [ProducesResponseType(typeof(TimeAngleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public IActionResult Get(
        [FromQuery] string? time = null,
        [FromQuery] int? hour = null,
        [FromQuery] int? minute = null)
    {
        var result = inputParser.ParseTimeInput(time, hour, minute);

        return result.Match<IActionResult>(
            input => Ok(angleCalculatorService.GetTimeAngle(input)),
            error => BadRequest(new ProblemDetails
            {
                Title = "Invalid time input",
                Detail = error.Message,
                Status = StatusCodes.Status400BadRequest,
                Extensions = { ["errorCode"] = error.Code }
            })
        );
    }
}