using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ChatBackend; // Adjust based on your actual namespace

public class TokenValidationMiddleware
{
    private readonly RequestDelegate _next;

    public TokenValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Check if the request has an Authorization header
        if (context.Request.Headers.TryGetValue("Authorization", out var token))
        {
            // Extract the token (assumed format: "Bearer <token>")
            var actualToken = token.ToString().Split(" ")[1];

            // Get the FirebaseService from the request services
            var firebaseService = context.RequestServices.GetService<FirebaseService>();

            // Validate the token
            var (isValid, decodedToken, errorMessage) = await firebaseService.VerifyFirebaseTokenAsync(actualToken);
            if (!isValid)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized; // Unauthorized
                await context.Response.WriteAsync("Unauthorized: " + errorMessage);
                return;
            }
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized; // Unauthorized
            await context.Response.WriteAsync("Unauthorized: No token provided");
            return;
        }
        // Call the next middleware in the pipeline
        await _next(context);
    }
}
