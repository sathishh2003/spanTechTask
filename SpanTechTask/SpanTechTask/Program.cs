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

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();


    #region JWT

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true
            };
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
             
                    Console.WriteLine($"Received Token: {context.Token}");
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    Console.WriteLine("User authenticated!");
                    logger.Info($"Token validated for {context.Principal?.Identity?.Name}");
                    return Task.CompletedTask;
                },
                OnAuthenticationFailed = context =>
                {
                    var rawToken = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                    Console.WriteLine(rawToken);

                    logger.Error($"Authentication failed: {context.Exception.Message}");
                    Console.WriteLine("User Failed!");


                    return Task.CompletedTask;
                },
                OnChallenge = context =>
                {
                    logger.Warn("JWT challenge triggered.");
                    Console.WriteLine("Token Triggred!");
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


    RegisterSwaggerUi(builder);

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

static void RegisterSwaggerUi(WebApplicationBuilder builder)
{
    builder.Services.AddSwaggerGen(options =>
    {
        options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

        options.SwaggerDoc("v1", new OpenApiInfo { Title = "SpanTechTask" });

        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            BearerFormat = "JWT",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            Description = "Enter yout JWT Access Token"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement {
            {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }});
    });

}