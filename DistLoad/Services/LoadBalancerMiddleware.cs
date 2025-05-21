using Microsoft.AspNetCore.Http;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using DistLoad.Services;
using System.Collections.Concurrent;

public class LoadBalancerMiddleware
{
    private readonly LoadBalancerManager _manager;
    private readonly RequestDelegate _next;

    // тепер публічна
    public static readonly ConcurrentDictionary<string, int> ServerCounters = new();

    public LoadBalancerMiddleware(RequestDelegate next, LoadBalancerManager manager)
    {
        _next = next;
        _manager = manager;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/swagger") ||
            context.Request.Path.StartsWithSegments("/metrics"))
        {
            await _next(context);
            return;
        }

        var server = await _manager.GetNextServerAsync();

        if (server != null)
        {
            ServerCounters.AddOrUpdate(server.Id, 1, (_, prev) => prev + 1);
        }

        // пропускаємо реальну проксі-передачу
        await _next(context);
    }
}
