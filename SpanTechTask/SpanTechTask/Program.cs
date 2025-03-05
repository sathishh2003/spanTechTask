using SpanTechTask.Repository;
using SpanTechTask.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Web;

var logger = NLog.LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();
try
{

    var builder = WebApplication.CreateBuilder(args);

    builder.Logging.ClearProviders(); // Remove default logging providers
    builder.Host.UseNLog(); // Use NLog

    // Add services to the container.
    #region JWT

    builder.Services.AddAuthentication("Bearer").AddJwtBearer("Bearer",options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"])),
                RequireSignedTokens = false
            };
              options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    logger.Error($"Authentication failed: {context.Exception.Message}");
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    logger.Info($"Token validated for {context.Principal?.Identity?.Name}");
                    return Task.CompletedTask;
                },
                OnChallenge = context =>
                {
                    logger.Warn("JWT challenge triggered.");
                    return Task.CompletedTask;
                }
            };
        });


    #endregion


    #region  CORS_Policy
    builder.Services.AddCors(builder =>
        builder.AddDefaultPolicy(policy =>
        {
            policy.AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowAnyOrigin();
        }));
    #endregion

    #region INJECTION
    builder.Services.AddScoped<EmployeeRepository>();
    builder.Services.AddScoped<EmployeeService>();
    builder.Services.AddScoped<TokenService>();
    builder.Services.AddSingleton<Base64EncryptionService>();
    builder.Services.AddScoped<LoginRepositroy>();
    builder.Services.AddScoped<AdminRepository>();
    builder.Services.AddScoped<AdminService>();
    #endregion

    builder.Services.AddAuthorization();

    builder.Services.AddControllers();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        var jwtSecurityScheme = new OpenApiSecurityScheme
        {
            BearerFormat = "JWT",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = JwtBearerDefaults.AuthenticationScheme,
            Description = "Enter yout JWT Access Token",
            Reference = new OpenApiReference
            {
                Id = JwtBearerDefaults.AuthenticationScheme,
                Type = ReferenceType.SecurityScheme
            }
        };
        options.AddSecurityDefinition("Bearer", jwtSecurityScheme);
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
        { jwtSecurityScheme,Array.Empty<string>() }
        });
    });

    var app = builder.Build();


    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthentication();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();

}
catch (Exception ex)
{
    logger.Error(ex, "Application stopped due to an exception");
}
finally
{
    NLog.LogManager.Shutdown();
       
}