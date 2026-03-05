namespace ClockAngle.Api.Models;

// Initial type for CalculateTimeAngle response
public sealed record TimeAngleResponse(
    double HourHandAngle,
    double MinuteHandAngle,
    double TotalAngle);