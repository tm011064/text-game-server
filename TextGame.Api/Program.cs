using Dapper;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Serialization;
using TextGame.Api.Auth;
using TextGame.Api.Controllers.Authentication.Events;
using TextGame.Core.Chapters;
using TextGame.Core.Emotions;
using TextGame.Core.Users.Events;
using TextGame.Core.TerminalCommands;
using TextGame.Data;
using TextGame.Data.Contracts.Chapters;
using TextGame.Data.Contracts.Emotions;
using TextGame.Data.Contracts.TerminalCommands;
using TextGame.Data.Sources;
using TextGame.Data.Sources.ResourceFiles;
using TextGame.Core.Setup;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using TextGame.Api.Transformers;
using TextGame.Core.Games;
using TextGame.Api.Middleware.Exceptions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddControllers(options =>
    {
        options.Conventions.Add(
            new RouteTokenTransformerConvention(new KebabCaseTransformer()));
        options.Filters.Add<SecurityTokenExceptionFilter>();
    })
    .AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
      {
        {
          new OpenApiSecurityScheme
          {
            Reference = new OpenApiReference
              {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
              },
              Scheme = "oauth2",
              Name = "Bearer",
              In = ParameterLocation.Header,

            },
            new List<string>()
          }
        });
});

builder.Services.AddSingleton<IChapterProvider, ChapterProvider>();
builder.Services.AddSingleton<ITerminalCommandProvider, TerminalCommandProvider>();
builder.Services.AddSingleton<IEmotionProvider, EmotionProvider>();
builder.Services.AddSingleton<IGameProvider, GameProvider>();
builder.Services.AddSingleton<SeedDataService>();

builder.Services.AddSingleton<IGlobalResourceJsonSource<TerminalCommand[]>, TerminalCommandsSource>();
builder.Services.AddSingleton<IGlobalResourceJsonSource<Emotion[]>, EmotionsSource>();
builder.Services.AddSingleton<IGameResourceJsonSource<IChapter[]>, ChaptersSource>();
builder.Services.AddSingleton<IGameSource, GameSource>();

builder.Services.AddSingleton<IQueryService, QueryService>();

SqlMapper.AddTypeHandler(new DateTimeOffsetTypeHandler());

builder.Services.AddSingleton<IJwtTokenValidator, JwtTokenValidator>();
builder.Services.AddSingleton<IJwtTokenFactory, JwtTokenFactory>();
builder.Services.AddSingleton<IRefreshTokenFactory, RefreshTokenFactory>();

builder.Services.AddMediatR(x => x
    .RegisterServicesFromAssemblyContaining<AuthenticateUserRequest>()
    .RegisterServicesFromAssemblyContaining<CreateUserRequest>());

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

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

using var scope = app.Services.CreateScope();
scope.ServiceProvider.GetRequiredService<IMigrationRunner>().MigrateUp();

var seedDataService = scope.ServiceProvider.GetRequiredService<SeedDataService>();

await seedDataService.InsertResourceFileGamesIfNotExist();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    await seedDataService.CreateTestUserIfNotExists("test", "test");
    await seedDataService.CreateTestUserIfNotExists("admin", "admin");
}

app.UseCors(x => x
    .WithOrigins("http://localhost:3000")
    .AllowAnyMethod()
    .WithHeaders("Content-Type", "Authorization", "Access-Control-Allow-Credentials")
    .AllowCredentials());

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<JwtMiddleware>();

app.Run();
