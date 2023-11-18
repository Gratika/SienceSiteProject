using apiServer.Controllers;
using apiServer.Controllers.Redis;
using apiServer.Controllers.Search;
using apiServer.Models;
using apiServer.Models.Example;
using Microsoft.EntityFrameworkCore;
using Minio;
using SolrNet;
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
builder.Services.AddScoped<SearchController>();
builder.Services.AddScoped<PeopleController>();
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
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
Startup.Init<Example>("http://solr:8983/solr/new_core");
app.UseAuthorization();

app.MapControllers();
app.UseCors("CorsPolicy"); //CorsPolicy AllowAll

app.Run();
