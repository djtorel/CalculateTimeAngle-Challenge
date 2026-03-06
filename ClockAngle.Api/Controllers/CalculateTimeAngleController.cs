using ClockAngle.Api.Models;
using ClockAngle.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClockAngle.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CalculateTimeAngleController(
    ITimeAngleCalculatorService angleCalculatorService,
    ITimeInputParser inputParser) : ControllerBase
{
    /// <summary>
    ///     Calculates the angular positions of the clock hands for a given time.
    ///     The two input styles are mutually exclusive; supplying both returns <c>400 Bad Request</c>.
    /// </summary>
    /// <param name="time">
    ///     Time string in strict <c>HH:mm</c> format (e.g. <c>03:00</c> or <c>14:30</c>).
    ///     Mutually exclusive with <paramref name="hour"/> and <paramref name="minute"/>.
    /// </param>
    /// <param name="hour">
    ///     Hour component on a 24-hour clock (0–23).
    ///     Requires <paramref name="minute"/>; mutually exclusive with <paramref name="time"/>.
    /// </param>
    /// <param name="minute">
    ///     Minute component (0–59).
    ///     Requires <paramref name="hour"/>; mutually exclusive with <paramref name="time"/>.
    /// </param>
    /// <returns>
    ///     <c>200 OK</c> with a <see cref="TimeAngleResponse"/> on success, or
    ///     <c>400 Bad Request</c> with a <see cref="ProblemDetails"/> body containing
    ///     a human-readable <c>detail</c> message and a machine-readable <c>errorCode</c> extension.
    /// </returns>
    [HttpGet]
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