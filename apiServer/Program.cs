using apiServer.Controllers;
using apiServer.Controllers.Authentication;
using apiServer.Controllers.ForModels;
using apiServer.Controllers.Minio;
using apiServer.Controllers.Redis;
using apiServer.Controllers.Search;
using apiServer.Controllers.Solr;
using apiServer.Models;
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
builder.Services.AddScoped<GenerateRandomStringController>();
builder.Services.AddScoped<FilesController>();
builder.Services.AddScoped<SearchController>();
builder.Services.AddScoped<PeopleController>();
builder.Services.AddScoped<SolrArticleController>();
builder.Services.AddScoped<OrderingController>();
builder.Services.AddScoped<FiltersController>();
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
Startup.Init<Articles>("http://solr:8983/solr/new_core");
Startup.Init<People>("http://solr:8983/solr/new_core");
Startup.Init<Scientific_theories>("http://solr:8983/solr/new_core");
app.UseAuthorization();

app.MapControllers();
app.UseCors("CorsPolicy"); //CorsPolicy AllowAll

app.Run();
