using fs_2025_assessment_1_74270.Data;
using fs_2025_assessment_1_74270.Models;
using fs_2025_assessment_1_74270.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// In-memory cache
builder.Services.AddMemoryCache();

// Our services
builder.Services.AddSingleton<BikeRepository>();                 // V1 - JSON
builder.Services.AddSingleton<BikeQueryService>();               // V1 - filters / cache
builder.Services.AddSingleton<CosmosBikeRepository>();           // V2 - Cosmos
builder.Services.AddHostedService<BikeUpdateBackgroundService>(); // background updates

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
