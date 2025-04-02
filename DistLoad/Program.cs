using DistLoad.Metrics;
using DistLoad.Models;
using DistLoad.Services;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

var servers = new List<ServerInstance>
{
    new() { Id = "1", Address = "http://localhost:5001" },
    new() { Id = "2", Address = "http://localhost:5002" },
    new() { Id = "3", Address = "http://localhost:5003" }
};

builder.Services.AddSingleton<List<ServerInstance>>(servers);
builder.Services.AddSingleton<LoadBalancerManager>();
builder.Services.AddSingleton<PrometheusExporter>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<LoadBalancerMiddleware>();
app.UseMetricServer();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

var exporter = app.Services.GetRequiredService<PrometheusExporter>();


Task.Run(exporter.LogMetricsLoop);

app.Run();