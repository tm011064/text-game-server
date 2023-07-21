using FluentMigrator.Runner;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using TextGame.Core.Chapters;
using TextGame.Core.Emotions;
using TextGame.Core.TerminalCommands;
using TextGame.Data;
using TextGame.Data.Contracts;
using TextGame.Data.Sources;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(x =>
{
    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

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

builder.Services.AddSingleton<IQueryService, QueryService>();

builder.Services.AddApiVersioning(config =>
{
    config.DefaultApiVersion = new ApiVersion(20220718, 0);
    config.AssumeDefaultVersionWhenUnspecified = true;
});

builder.Services.AddLazyCache();

builder.Services.AddFluentMigratorCore()
    .ConfigureRunner(builder => builder.AddSQLite()
        .WithGlobalConnectionString("SqlLiteDatabase")
        .ScanIn(typeof(QueryService).Assembly).For.Migrations());

var app = builder.Build();

using var scope = app.Services.CreateScope();
scope.ServiceProvider.GetRequiredService<IMigrationRunner>().MigrateUp();


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
