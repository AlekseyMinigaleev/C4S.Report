using C4S.API.Extensions;
using C4S.DB;
using C4S.Services.Extensions;
using C4S.Services.Services.GameSyncService;
using C4S.Services.Services.JWTService;
using C4S.Shared.Models;
using FluentValidation;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using C4S.API.Helpers;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;
var jwtSection = configuration
    .GetSection("JwtConfiguration");
var jwtConfiguration = jwtSection
    .Get<JwtConfiguration>() ?? throw new ArgumentNullException(nameof(JwtConfiguration));
var jwtService = new JwtServise(jwtConfiguration);

#region services
services.AddHttpClient();
services.AddControllers();
services.AddEndpointsApiExplorer();

services.AddSwaggerGen(options =>
{
    options.IncludeXmlComments($"{AppContext.BaseDirectory}C4S.API.xml");
    options.CustomSchemaIds(ShemaClassesIdsRenamer.Selector);

    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "JWT Authentication",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    options.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
});

services.AddStorages(configuration);

services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblies(typeof(Program).GetTypeInfo().Assembly));
services.AddValidatorsFromAssemblyContaining<Program>();
services.AddValidatorsFromAssemblyContaining<ReportDbContext>();
services.AddAutoMapper(
    typeof(Program),
    typeof(IGameSyncService));

services.AddServices(configuration);

services.AddAuthorization();
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtConfiguration.Issuer,

            ValidateAudience = true,
            ValidAudience = jwtConfiguration.Audience,

            ValidateLifetime = true,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = jwtService.SymmetricSecurityKey,

            ClockSkew = TimeSpan.Zero,
        };
    });

services.Configure<JwtConfiguration>(jwtSection);

builder.Services.AddCors();

services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Expiration = TimeSpan.FromDays(90);
});
#endregion services

var app = builder.BuildWithHangfireStorage(configuration);

#region middleware
app.UseCors(options => options
    .WithOrigins("http://localhost:3000", "http://localhost:5041/swagger")
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHangfireDashboard();
app.UseHttpsRedirection();
app.MapControllers();

await app.InitApplicationAsync();
await app.RunAsync();
#endregion middleware
    