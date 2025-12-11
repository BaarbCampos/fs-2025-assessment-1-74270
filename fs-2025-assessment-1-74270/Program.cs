using fs_2025_assessment_1_74270.Data;
using fs_2025_assessment_1_74270.Models;
using fs_2025_assessment_1_74270.Services;
using Microsoft.Azure.Cosmos;

var builder = WebApplication.CreateBuilder(args);

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// V1 – JSON
builder.Services.AddSingleton<BikeRepository>();
builder.Services.AddSingleton<BikeQueryService>();

// Cosmos Client (V2)
builder.Services.AddSingleton(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var endpoint = config["CosmosDb:AccountEndpoint"];
    var key = config["CosmosDb:AccountKey"];
    return new CosmosClient(endpoint, key);
});

// V2 – Cosmos
builder.Services.AddSingleton<CosmosBikeRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
