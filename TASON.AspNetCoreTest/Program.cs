using TASON.AspNetCoreTest;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddTason(options =>
    {
        options.SerializerOptions.Indent = 2;
        options.GetAutoRegisterObjectTypes = () =>
        {
            return 
            [
                typeof(WeatherForecast), 
            ];
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
