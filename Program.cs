global using financing_api.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Principal;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using financing_api.Data;
using financing_api.Services.PlaidService;
using financing_api.Services.TransactionsService;
using financing_api.Services.AccountService;
using Going.Plaid;
using financing_api.Shared;
using financing_api.PlaidInterface;
using financing_api.Services.CategoryService;
using financing_api.DAL;
using financing_api.Logger;
using financing_api.DbLogger;
using Microsoft.Extensions.Configuration.Yaml;

var builder = WebApplication.CreateBuilder(args);
var configBuilder = new ConfigurationBuilder();
var services = builder.Services;
var allowMyOrigins = "AllowMyOrigins";

builder.Logging.ClearProviders();

var connectionString = builder.Configuration.GetConnectionString("AzureAppConfiguration");
configBuilder.AddAzureAppConfiguration(connectionString);



// You can put your plaid secrets here. But really you can put them
// configBuilder.AddYamlFile("secrets.yaml", optional: true);

var configuration = configBuilder.Build();

// Add Going.Plaid services
services.AddHttpClient();

//builder.Configuration.AddYamlFile("secrets.yaml", optional: true);
services.Configure<PlaidCredentials>(builder.Configuration.GetSection(PlaidOptions.SectionKey));
services.Configure<PlaidOptions>(builder.Configuration.GetSection(PlaidOptions.SectionKey));
services.AddSingleton<PlaidClient>();
services.AddSingleton<IConfiguration>(configuration);

var client = new PlaidClient(Going.Plaid.Environment.Sandbox);

// Add logging
builder.Logging.AddConsole();
builder.Logging.AddEventSourceLogger();

// Add services to the container.
services.AddDbContext<DataContext>(
    options =>
    {
        options.UseSqlServer(configuration["DbConnectionString"]);
    },
        ServiceLifetime.Transient
);

services.AddControllers();

// Turn off claim mapping for Microsoft middleware
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition(
        "oauth2",
        new OpenApiSecurityScheme
        {
            Description =
                "Standard Authorization header using the Bearer scheme, e.g. \"bearer {token} \"",
            In = ParameterLocation.Header,
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey
        }
    );
    c.OperationFilter<SecurityRequirementsOperationFilter>();
});
services.AddSwaggerGenNewtonsoftSupport();
services.AddAutoMapper(typeof(Program).Assembly);
services.AddScoped<IUserService, UserService>();
services.AddScoped<IPlaidService, PlaidService>();
services.AddScoped<ITransactionsService, TransactionsService>();
services.AddScoped<IAccountService, AccountService>();
services.AddScoped<ICategoryService, CategoryService>();
services.AddScoped<IPlaidApi, PlaidApi>();

services.AddTransient<ILogging, Logging>();
services.AddTransient<TransactionDAL>();

// Authentication
services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(configuration["AppSettings:Key"])
            ),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });


services.AddHttpContextAccessor();
services.AddSingleton<IPrincipal>(
    provider => provider.GetService<IHttpContextAccessor>().HttpContext.User
);


services.AddCors(options =>
{
    options.AddPolicy(
        allowMyOrigins,
        builder =>
        {
            builder
                .WithOrigins("http://localhost:3000", "https://localhost:3000", "http://localhost:5000", "https://localhost:5000", "https://financing-app.azurewebsites.net", "https://money-clarity.azurewebsites.net", "https://finance-management.azurewebsites.net")
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
    );
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(allowMyOrigins);

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
