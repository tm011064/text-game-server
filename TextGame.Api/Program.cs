using Microsoft.AspNetCore.Mvc;
using TextGame.Core.Chapters;
using TextGame.Core.Emotions;
using TextGame.Core.TerminalCommands;
using TextGame.Data.Contracts;
using TextGame.Data.Sources;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IChapterProvider, ChapterProvider>();
builder.Services.AddSingleton<ITerminalCommandProvider, TerminalCommandProvider>();
builder.Services.AddSingleton<IEmotionProvider, EmotionProvider>();

builder.Services.AddSingleton<IGlobalResourceJsonSource<TerminalCommand[]>, TerminalCommandsSource>();
builder.Services.AddSingleton<IGlobalResourceJsonSource<Emotion[]>, EmotionsSource>();
builder.Services.AddSingleton<IGameResourceJsonSource<Chapter[]>, ChaptersSource>();
builder.Services.AddSingleton<IGameSource, GameSource>();

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
