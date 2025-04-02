using Prometheus;
using Server2.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ServerMetricsService>();  
builder.Services.AddControllers();  

var app = builder.Build();

var metricsService = app.Services.GetRequiredService<ServerMetricsService>();

app.MapGet("/", async () =>
{
    metricsService.IncreaseRequests();
    await Task.Delay(Random.Shared.Next(50, 300));  
    metricsService.DecreaseRequests();
    return Results.Ok("Response from Server 1");
});

app.MapControllers(); 

app.UseMetricServer();
app.Run("http://localhost:5002");
