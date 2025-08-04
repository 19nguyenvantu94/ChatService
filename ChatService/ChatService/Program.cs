using ChatService;
using ChatService.DatabaseContext;
using ChatService.Helper;
using ChatService.HubService;
using ChatService.ServiceDefaults;
using ChatService.TrackerRedis;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Minio;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .SetIsOriginAllowed(origin => true); // Cho tất cả domain
    });
});

var configSection = builder.Configuration.GetSection("MinioClient");

var settings = new MinIoClientSettings();
configSection.Bind(settings);

builder.Services.AddMinio(configureClient => configureClient
       .WithEndpoint(settings.Endpoint)
       .WithSSL(true)
       .WithCredentials(settings.AccessKey, settings.SecretKey));

builder.AddDefaultAuthentication();

builder.Services.AddAntiforgery();

builder.Services.AddAuthorization();

var withApiVersioning = builder.Services.AddApiVersioning();

builder.AddDefaultOpenApi(withApiVersioning);

var mysqlConnection = builder.Configuration.GetConnectionString("Identitydb");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(mysqlConnection, new MySqlServerVersion(new Version(8, 0, 36))));

// Đăng ký IConnectionMultiplexer
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = builder.Configuration.GetConnectionString("Redis");
    return ConnectionMultiplexer.Connect(configuration);
});

Console.WriteLine("Redis connect string");

builder.Services.AddScoped<PresenceService>();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();
app.UseDefaultOpenApi();

app.UseAuthorization();
app.MapHub<CallHub>("/CallHub");
app.MapHub<MessagingHub>("/MessagingHub");

app.MapControllers();

app.Run();
