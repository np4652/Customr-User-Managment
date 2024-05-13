using Microsoft.Extensions.DependencyInjection;
using UserManagement.Domain.Base;
using UserManagement.Domain.Interfaces;
using UserManagement.Infrastructure;
using UserManagement.Infrastructure.Repositories;
using UserManagement.Infrastructure.Services;
using UserManagment.WebAPI.Modals;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IDbContext>(sp => new DbContext($"{builder.Configuration.GetConnectionString("DbContext")}"));
builder.Services.AddScoped<ISignInManager, SignInManager>();
builder.Services.AddSingleton<IPasswordManager, PasswordManager>();
builder.Services.AddSingleton<IGoogleAuthenticatorManager, GoogleAuthenticatorManager>();
AppSettings appSettings = new();
builder.Configuration.Bind(appSettings);
builder.Services.AddSingleton<ITokenManager, TokenManager>(cng => new TokenManager(appSettings.JWTConfig));
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
