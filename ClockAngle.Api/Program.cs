using ClockAngle.Api.Services;

var builder = WebApplication.CreateBuilder(args);


// Services
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Register services
builder.Services.AddSingleton<ITimeAngleCalculatorService, TimeAngleCalculatorService>();
builder.Services.AddSingleton<ITimeInputParserService, TimeInputParserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();