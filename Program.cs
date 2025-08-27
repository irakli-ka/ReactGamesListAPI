using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using ReactGamesListAPI.Data;
using ReactGamesListAPI.Data.Interfaces;
using ReactGamesListAPI.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173", "https://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var userId = builder.Configuration["UserId"];
var password = builder.Configuration["Password"];
var baseConnStr = builder.Configuration.GetConnectionString("SQLDbConnection");

var sqlConnectionString = baseConnStr?
    .Replace("{UserId}", userId)
    .Replace("{Password}", password);

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(sqlConnectionString));
builder.Services.AddScoped<IGameRepo, GameRepo>();
builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);
builder.Services.AddAuthorization();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowReactApp");
app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers();
app.Run();