using Infinity.Toolkit.FeatureModules;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedPrefix;
});

// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy", policy =>
    {
        //if (builder.Environment.IsDevelopment())
        //{
        //    // More permissive policy for development
        //    policy.AllowAnyOrigin()
        //          .AllowAnyMethod()
        //          .AllowAnyHeader();
        //}
        //else
        //{
            // More restrictive policy for production using configuration
            var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? ["https://localhost:5001"];
            
            policy.WithOrigins(allowedOrigins)
                  .WithMethods(["GET"]);
        //}
    });
});

builder.AddFeatureModules();
builder.Services.AddHealthChecks();
builder.Services.AddAuthentication().AddJwtBearer();
builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseStatusCodePages();
}

app.UseForwardedHeaders();
app.Use(async (context, next) =>
{
    if (context.Request.Headers.TryGetValue("X-Forwarded-Prefix", out var prefix))
    {
        context.Response.Headers.Append("X-Forwarded-Prefix", new[] { prefix.ToString() });
    }

    await next(context);
});

// Enable CORS
app.UseCors("DefaultCorsPolicy");

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapFeatureModules();

app.Run();

// Make the implicit Program class public so test projects can access it
public partial class Program { }
