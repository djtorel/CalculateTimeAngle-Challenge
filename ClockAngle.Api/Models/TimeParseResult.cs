namespace ClockAngle.Api.Models;

public abstract record TimeParseResult
{
    // Creating skeleton of basic discriminated union for results 
    private TimeParseResult()
    {
    }

    public sealed record Success(TimeInput Input) : TimeParseResult;

    public sealed record Failure(string ErrorMessage) : TimeParseResult;
}