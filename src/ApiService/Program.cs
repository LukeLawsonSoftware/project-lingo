using Microsoft.Extensions.AI;


var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add AI Service
builder.AddOllamaApiClient("chat")
                .AddChatClient()
                .UseFunctionInvocation();


// Add services to the container.
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();


string[] summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

app.MapGet("/weatherforecast", (IChatClient chatClient) =>
{

    ChatOptions chatOptions = new ChatOptions
    {
        Temperature = 0.5f,
        TopP = 1.0f,
        FrequencyPenalty = 0.0f,
        PresencePenalty = 0.0f,
    };

    var forecast = Enumerable.Range(1, 5).Select(index =>
        {
            var temp = Random.Shared.Next(-20, 55);
            return new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                temp,
                chatClient.GetResponseAsync($"In one word, what is the weather like in {temp} degrees Celsius?", chatOptions).Result.Text
            );
        })
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");


app.MapDefaultEndpoints();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
