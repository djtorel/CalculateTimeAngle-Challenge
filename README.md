# CalculateTimeAngle

A lightweight ASP.NET Core Web API that calculates the angular positions of the hour and minute clock hands for a given time.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

## Running the API

```bash
cd ClockAngle.Api
dotnet run
```

The browser will open automatically at the interactive API explorer (`/scalar/v1`), where you can browse the endpoint documentation and send test requests directly.

## Running the Tests

```bash
dotnet test
```

## Using the Endpoint

The endpoint accepts two mutually exclusive input styles.

**Using a time string (`HH:mm` format):**

```
GET /CalculateTimeAngle?time=03:00
```

**Using separate hour and minute values:**

```
GET /CalculateTimeAngle?hour=3&minute=0
```

### Example Response

```json
{
  "hourHandAngle": 90.0,
  "minuteHandAngle": 0.0,
  "totalAngle": 90.0
}
```

| Field             | Description                                                  |
| ----------------- | ------------------------------------------------------------ |
| `hourHandAngle`   | Hour hand position in degrees, measured clockwise from 12.   |
| `minuteHandAngle` | Minute hand position in degrees, measured clockwise from 12. |
| `totalAngle`      | Sum of `hourHandAngle` and `minuteHandAngle`.                |

### Error Responses

Invalid input returns `400 Bad Request` with a `ProblemDetails` body that includes a human-readable `detail` message and a machine-readable `errorCode` extension field.
