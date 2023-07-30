using DMCopilot.WebApi.Extensions;

namespace DMCopilot.WebApi;

/// <summary>
/// DMCopilot WebApi
/// </summary>
public sealed class Program
{
    /// <summary>
    /// Entry point
    /// </summary>
    /// <param name="args">Web application command-line arguments.</param>
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services
            .AddSingleton<ILogger>(sp => sp.GetRequiredService<ILogger<Program>>())
            .AddOptions(builder.Configuration);

        builder.Services.AddOptions();
        builder.Services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}