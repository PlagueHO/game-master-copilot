using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using GMCopilot.ApiCore.Services;
using Microsoft.OpenApi.Models;
using GMCopilot.AccessApi.Extensions;

namespace GMCopilot.AccessApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddServiceDefaults();

        // Add the options services
        builder.AddOptions();

        // Add the data store services
        builder.AddDataStore();

        // Add authentication
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("EntraId"));

        // Add the AuthorizationService to provide authorization services to the API controllers
        builder.Services.AddScoped<IAuthorizationService, AuthorizationService>();

        // Add Health Check
        builder.Services.AddHealthChecks()
            .AddCheck<AccessApiHealthCheck>("Access API");

        // Add API Controllers
        builder.Services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Access API", Version = "v1" });
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[]{}
                }
            });
        });

        var app = builder.Build();

        app.MapDefaultEndpoints();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        // Map Health Check endpoints
        app.MapHealthChecks("/healthz");

        app.Run();
    }
}
