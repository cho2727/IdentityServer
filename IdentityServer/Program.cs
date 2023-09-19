
using IdentityServer.Authorization;
using IdentityServer.Shared.Extensions;
using IdentityServer.Shared.Injectables;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAuthentication().AddJwtBearer();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IAuthorizationHandler, CustomAuthorizationHandler>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CustomAuthorizationPolicy", policy =>
    {
        policy.Requirements.Add(new CustomAuthorizationRequirement());
    });
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "IdentityServer API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Name = "Bearer",
        BearerFormat = "JWT",
        Description = "Please enter authorization key",
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme()
                    {
                         Reference = new OpenApiReference()
                         {
                             Type = ReferenceType.SecurityScheme,
                             Id = "Bearer"
                         }
                    },
                    Enumerable.Empty<string>().ToList()
                }
            });
    c.CustomSchemaIds(x => x.FullName?.Replace("+", "."));
});

// JSON OPTION
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
    options.SerializerOptions.WriteIndented = true;
});

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.Scan(scan => scan
                               .FromEntryAssembly()
                               .AddClasses(
                                   classes => classes.AssignableTo<ITransient>()
                                )
                               .AsSelfWithInterfaces()
                               .WithTransientLifetime()
                               .AddClasses(
                                   classes => classes.AssignableTo<IScoped>()
                                )
                               .AsSelfWithInterfaces()
                               .WithScopedLifetime()
                               .AddClasses(
                                   classes => classes.AssignableTo<ISingleton>())
                               .AsSelfWithInterfaces()
                               .WithSingletonLifetime()
                               );

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapEndPoints();
//app.UseAuthorization();
//app.MapControllers();

app.Run();
