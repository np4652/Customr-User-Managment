using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
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
AppSettings appSettings = new();
builder.Configuration.Bind(appSettings);
builder.Services.AddScoped<IDbContext>(sp => new DbContext($"{builder.Configuration.GetConnectionString("DbContext")}"));
builder.Services.AddAuthentication(option =>
{
    option = new Microsoft.AspNetCore.Authentication.AuthenticationOptions
    {
        DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme,
        DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme,
        DefaultScheme = JwtBearerDefaults.AuthenticationScheme
    };
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = appSettings.JWTConfig?.Issuer,
        ValidAudience = appSettings.JWTConfig?.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.JWTConfig?.Secretkey ?? string.Empty))
    };
});
builder.Services.AddScoped<ISignInManager, SignInManager>();
builder.Services.AddSingleton<IPasswordManager, PasswordManager>();
builder.Services.AddSingleton<IGoogleAuthenticatorManager, GoogleAuthenticatorManager>();

builder.Services.AddSingleton<ITokenManager, TokenManager>(cng => new TokenManager(appSettings.JWTConfig));
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
