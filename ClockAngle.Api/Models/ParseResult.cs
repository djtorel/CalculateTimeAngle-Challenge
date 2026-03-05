namespace ClockAngle.Api.Models;

public abstract record ParseResult
{
    // Creating skeleton of basic discriminated union for results 
    private ParseResult() { }

    public sealed record Success(TimeInput Input) : ParseResult;

    public sealed record Failure(string ErrorMessage) : ParseResult;
}