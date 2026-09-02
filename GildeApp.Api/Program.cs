using GildeApp.Api.Core.Data;
using GildeApp.Api.Core.Services;
using GildeApp.Api.Core.Services.Interfaces;
using GildeApp.Api.Hubs;
using Microsoft.EntityFrameworkCore;

namespace GildeApp.Api
{
    public class Program
    {
        private const string WebAppCors = "WebAppCors";

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultDatabase")));

            builder.Services.AddControllers();

            // AddSwaggerGen/UseSwagger/UseSwaggerUI need the Swashbuckle.AspNetCore
            // package, which this project has never referenced -- that is why it would
            // not build. Microsoft.AspNetCore.OpenApi is referenced, so the document is
            // served from /openapi/v1.json instead. For the Swagger UI page back, run
            //     dotnet add GildeApp.Api package Swashbuckle.AspNetCore
            // and put the three calls back.
            builder.Services.AddOpenApi();
            builder.Services.AddSignalR();

            // The MVC app and the mobile app are separate origins. AllowCredentials is
            // required for SignalR, and it cannot be combined with AllowAnyOrigin, so
            // the allowed origins are listed in configuration instead.
            var allowedOrigins = builder.Configuration
                .GetSection("Cors:AllowedOrigins")
                .Get<string[]>() ?? Array.Empty<string>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(WebAppCors, policy =>
                {
                    policy.WithOrigins(allowedOrigins)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            builder.Services.AddScoped<IMatchService, MatchService>();
            builder.Services.AddScoped<IPlayerService, PlayerService>();
            builder.Services.AddScoped<IRuleSetService, RuleSetService>();
            builder.Services.AddScoped<ITourneyService, TourneyService>();
            builder.Services.AddScoped<IWeaponService, WeaponService>();
            builder.Services.AddSingleton<IBoutScheduler, BoutScheduler>();
            builder.Services.AddScoped<BoardBroadcaster>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();   // /openapi/v1.json
            }

            app.UseHttpsRedirection();

            app.UseCors(WebAppCors);

            app.UseAuthorization();

            app.MapControllers();
            app.MapHub<BoardHub>("/hubs/board");

            app.Run();
        }
    }
}
