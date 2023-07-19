using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using TextGame.Core.Rooms;
using TextGame.Data.Contracts;
using TextGame.Data.Sources;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IChapterProvider, ChapterProvider>();
builder.Services.AddSingleton<IGameContextProvider, GameContextProvider>();
builder.Services.AddSingleton<IGameContextItemJsonSource<TerminalCommand[]>, TerminalCommandsSource>();
builder.Services.AddSingleton<IGameContextItemJsonSource<Emotion[]>, EmotionsSource>();
builder.Services.AddSingleton<IGameContextItemJsonSource<Chapter[]>, ChaptersSource>();
builder.Services.AddSingleton<IGameContextSource, GameContextSource>();

builder.Services.AddApiVersioning(config =>
{
    config.DefaultApiVersion = new ApiVersion(20220718, 0);
    config.AssumeDefaultVersionWhenUnspecified = true;
});

builder.Services.AddLazyCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
