using C4S.API.Extensions;
using C4S.DB;
using C4S.Helpers.ApiHeplers.Swagger;
using C4S.Services.Extensions;
using C4S.Services.Interfaces;
using FluentValidation;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using S4C.YandexGateway.DeveloperPage;
using System.Reflection;
using �4S.API.Extensions;
using �4S.API.SettingsModels;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;
var jwtSection = configuration
    .GetSection("JWT");
var jwtConfiguration = jwtSection
    .Get<JWTConfiguration>() ?? throw new ArgumentNullException(nameof(JWTConfiguration));

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
    typeof(IDeveloperPageGetaway),
    typeof(IGameDataService));
services.AddServices(configuration);
services.AddAuthorization();
services.Configure<JWTConfiguration>(jwtSection);
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
            IssuerSigningKey = jwtConfiguration.GetSymmetricSecurityKey(),
        };
    });
builder.Services.AddCors();
#endregion services

var app = builder.BuildWithHangfireStorage(configuration);

#region middleware
app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHangfireDashboard();
app.UseHttpsRedirection();
app.MapControllers();
app.UseCors(options => options.WithOrigins("http://localhost:3000", "http://localhost:5041/swagger").AllowAnyMethod().AllowAnyHeader());

await app.InitApplicationAsync();
await app.RunAsync();
#endregion middleware
