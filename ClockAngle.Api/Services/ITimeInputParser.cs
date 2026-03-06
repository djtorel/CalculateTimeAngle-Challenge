using ClockAngle.Api.Core;
using ClockAngle.Api.Models;

namespace ClockAngle.Api.Services;

public interface ITimeInputParser
{
    Result<TimeInput> ParseTimeInput(string? time, int? hour, int? minute);

    Result<TimeInput> ParseFromString(string time);

    Result<TimeInput> ParseFromParts(int? hour, int? minute);
}