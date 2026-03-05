using ClockAngle.Api.Models;

namespace ClockAngle.Api.Services;

public interface ITimeInputParserService
{
    TimeParseResult ParseTimeInput(string? time, int? hour, int? minute);

    TimeParseResult ParseFromString(string time);

    TimeParseResult ParseFromParts(int? hour, int? minute);
}