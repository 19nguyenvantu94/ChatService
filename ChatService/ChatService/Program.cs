using ChatService.DatabaseContext;
using ChatService.Helper;
using ChatService.HubService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

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
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                if (!string.IsNullOrEmpty(accessToken) &&
                    context.HttpContext.WebSockets.IsWebSocketRequest)
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

var mysqlConnection = builder.Configuration.GetConnectionString("Identitydb");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(mysqlConnection, new MySqlServerVersion(new Version(8, 0, 36))));

var redis = builder.Configuration.GetConnectionString("Redis") ?? "";

//var redisConnection = Configuration["ConnectionStrings:Redis"] ?? "";

await RedisHelper.InitAsync(redis);

Console.WriteLine("Redis connect string");


var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.MapHub<CallHub>("/CallHub");
app.MapHub<MessagingHub>("/MessagingHub");

app.MapControllers();



app.Run();
