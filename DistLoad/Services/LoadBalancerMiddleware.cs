using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DistLoad.Services;

public class LoadBalancerMiddleware
{
    private readonly LoadBalancerManager _manager;
    private readonly RequestDelegate _next;

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
            var targetUri = new Uri($"{server.Address}{context.Request.Path}{context.Request.QueryString}");
            var requestMessage = new HttpRequestMessage(new HttpMethod(context.Request.Method), targetUri);

            foreach (var header in context.Request.Headers)
            {
                requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
            }

            using var client = new HttpClient();
            var response = await client.SendAsync(requestMessage);

            context.Response.StatusCode = (int)response.StatusCode;
            foreach (var header in response.Headers)
            {
                context.Response.Headers[header.Key] = header.Value.ToArray();
            }

            await response.Content.CopyToAsync(context.Response.Body);
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            await context.Response.WriteAsync("No available servers.");
        }
    }
}