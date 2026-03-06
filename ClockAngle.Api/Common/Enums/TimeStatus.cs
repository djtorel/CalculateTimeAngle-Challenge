namespace ClockAngle.Api.Common.Enums;

public static class TimeStatus
{
    public enum Hour
    {
        IsEmpty,
        HasValue
    }

    public enum Minute
    {
        IsEmpty,
        HasValue
    }

    public enum Time
    {
        IsEmpty,
        HasValue
    }

    public static (Time, Hour, Minute) GetTimeStatuses(
        string? time,
        int? hour,
        int? minute)
    {
        return (
            string.IsNullOrEmpty(time) ? Time.IsEmpty : Time.HasValue,
            hour.HasValue ? Hour.HasValue : Hour.IsEmpty,
            minute.HasValue ? Minute.HasValue : Minute.IsEmpty
        );
    }
}