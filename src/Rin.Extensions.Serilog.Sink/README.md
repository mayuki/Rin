Serilog sink for writings trace info to Rin.

# Syntax description

There are 2 ways to add Serilog support. Both ways give the same result.

## Code
```csharp
Log.Logger = new LoggerConfiguration()
	.WriteTo.Console()
	.WriteTo.Rin(LogEventLevel.Information)
	.CreateLogger();
```

## Configuration
```json
{
  "Serilog": {
    "Using": [
      "Serilog.Sinks.Console",
      "Rin.Extensions.Serilog.Sink"
    ],
    "MinimumLevel": "Debug",
    "WriteTo": [
      {
        "Name": "Console"
      },
      {
        "Name": "Rin",
        "Args": {
          "restrictedToMinimumLevel": "Information"
        }
      }
    ]
  }
}
```

```csharp
var builder = WebApplication.CreateBuilder();
Log.Logger = new LoggerConfiguration()
	.ReadFrom.Configuration(builder.Configuration)
	.CreateLogger();
```