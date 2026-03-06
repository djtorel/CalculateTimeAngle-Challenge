using ClockAngle.Api.Models;

namespace ClockAngle.Api.Services;

public interface ITimeAngleCalculatorService
{
    TimeAngleResponse GetTimeAngle(TimeInput timeInput);
}