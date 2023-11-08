using apiServer.Controllers;
using apiServer.Models;
using Microsoft.EntityFrameworkCore;
using Minio;
using StackExchange.Redis;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.

builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddScoped<EmailController>();
builder.Services.AddScoped<TokensController>();
builder.Services.AddScoped<GenerateRandomStringControlle>();
builder.Services.AddScoped<MinioController>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IConnectionMultiplexer>(x =>
{
    return ConnectionMultiplexer.Connect("redis:6379");
});

builder.Services.AddDbContext<ArhivistDbContext>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder
            .WithOrigins("http://localhost:5001", "http://localhost:5000")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});
//builder.Services.MakeBucket.Run(minioClient, bucketName).Wait();

//builder.Services.AddMinio(options =>
//{
//    options.WithEndpoint("localhost");
//    options.WithCredentials("ROOTUSER", "CHANGEME123");
//});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();
app.UseCors("CorsPolicy");

app.Run();
