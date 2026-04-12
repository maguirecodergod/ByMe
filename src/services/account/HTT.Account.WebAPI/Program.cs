using HTT.AspNetCore.OpenApi.Contracts;
using HTT.AspNetCore.OpenApi.Extensions;
using System.ComponentModel;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

builder.Services.AddHttOpenApi(builder.Configuration, opt => opt.UiProvider = CApiDocsUiProvider.All);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttOpenApi();

app.UseCors("AllowAll");

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast",
    Results<Ok<WeatherForecast[]>, ValidationProblem> (
        [FromQuery, Description("Number of days to return. Allowed range: 1-14.")] int days = 5,
        [FromQuery, Description("Optional minimum temperature in Celsius.")] int? minTempC = null,
        [FromQuery, Description("Optional maximum temperature in Celsius.")] int? maxTempC = null) =>
{
    var errors = new Dictionary<string, string[]>();
    if (days is < 1 or > 14)
        errors["days"] = ["The 'days' query parameter must be between 1 and 14."];
    if (minTempC.HasValue && maxTempC.HasValue && minTempC > maxTempC)
        errors["temperatureRange"] = ["'minTempC' must be less than or equal to 'maxTempC'."];

    if (errors.Count > 0)
        return TypedResults.ValidationProblem(errors);

    var forecast = Enumerable.Range(1, days).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .Where(x => !minTempC.HasValue || x.TemperatureC >= minTempC)
        .Where(x => !maxTempC.HasValue || x.TemperatureC <= maxTempC)
        .ToArray();

    return TypedResults.Ok(forecast);
})
.WithName("GetWeatherForecast")
.WithTags("Weather Forecast")
.WithSummary("Get weather forecasts for upcoming days.")
.WithDescription("""
Returns generated weather forecasts.

This endpoint is intended as a sample API for frontend integration and API documentation testing.
Use query parameters to control the number of returned days and optionally filter by temperature range.
""")
.Produces<WeatherForecast[]>(StatusCodes.Status200OK, "application/json")
.ProducesValidationProblem(StatusCodes.Status400BadRequest, "application/problem+json");

app.Run();

record WeatherForecast(
    DateOnly Date,
    int TemperatureC,
    string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
