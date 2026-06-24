
using GildeApp.Api.Core.Data;
using GildeApp.Api.Core.Services;
using GildeApp.Api.Core.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GildeApp.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultDatabase")));

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();   // serves UI at /swagger, reading /swagger/v1/swagger.json
            }

            builder.Services.AddScoped<IMatchService, MatchService>();
            builder.Services.AddScoped<IPlayerService, PlayerService>();
            builder.Services.AddScoped<IRuleSetService, RuleSetService>();
            builder.Services.AddScoped<ITourneyService, TourneyService>();
            builder.Services.AddScoped<IWeaponService, WeaponService>();
            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
