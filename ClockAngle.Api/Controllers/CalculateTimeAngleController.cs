using Microsoft.AspNetCore.Mvc;

namespace ClockAngle.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CalculateTimeAngleController() : ControllerBase
{
    // Add controller logic here
    public IActionResult Get(
        [FromQuery] string? time, 
        [FromQuery] int? hour, 
        [FromQuery] int? minute)
    {
        return Ok(new { time, hour, minute });
    }
}