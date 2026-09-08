using Infinity.Toolkit.FeatureModules;
using Scalar.AspNetCore;
using System.Reflection;

namespace Infinity.HelloWorld.Features.OpenApi;

[WebFeatureModule("OpenAPI Module", "1.0.0")]
public class OpenApiModule : WebFeatureModule
{
    public override void RegisterModule(IHostApplicationBuilder builder)
    {
        builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        });

        builder.Services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, serviceProvider, _) =>
            {
                document.Info.Title = builder.Configuration["OpenApi:Info:Title"];
                document.Info.Version = $"Version {Assembly.GetExecutingAssembly().GetName().Version?.ToString()}";
                document.Info.Description = $"{builder.Configuration["OpenApi:Info:Description"]} - Environment: {builder.Environment.EnvironmentName}";

                return Task.CompletedTask;
            });
        });
    }

    public override void MapEndpoints(WebApplication builder)
    {
        builder.MapOpenApi();
        builder.MapScalarApiReference();
    }
}
