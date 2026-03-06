using ClockAngle.Api.Models;

namespace ClockAngle.Api.Services;

public class TimeAngleCalculatorService : ITimeAngleCalculatorService
{
    private const double DegreesPerHour = 30.0;
    private const double DegreesPerMinuteOnHourHand = 0.5;
    private const double DegreesPerMinute = 6.0;

    // Add main implementation of ITimeAngleCalculatorService here
    public TimeAngleResponse GetTimeAngle(TimeInput timeInput)
    {
        return BuildResponse(timeInput, ComputerHourAngle(timeInput), ComputerMinuteAngle(timeInput));
    }

    private static double ComputerHourAngle(TimeInput timeInput)
    {
        return timeInput.Hour % 12 * DegreesPerHour + timeInput.Minute * DegreesPerMinute;
    }

    private static double ComputerMinuteAngle(TimeInput timeInput)
    {
        return timeInput.Minute * DegreesPerMinute;
    }

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