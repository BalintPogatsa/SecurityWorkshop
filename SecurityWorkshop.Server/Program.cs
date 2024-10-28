
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using SecurityWorkshop.Server.DataAccess;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace SecurityWorkshop.Server
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var builder = WebApplication.CreateBuilder(args);

      // Add services to the container.

      builder.Services.AddControllers();
      // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
      builder.Services.AddEndpointsApiExplorer();
      builder.Services.AddSwaggerGen();

      builder.Services.AddHttpLogging(options => { });

      builder.Services.AddTransient<IClaimsTransformation, ClaimsTransformer>();

      builder.Services.AddAuthentication()
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
          options.Authority = @"https://localhost:8181/realms/quickstart";
          options.Audience = "account";
          options.MapInboundClaims = false;
        })
        .AddOpenIdConnect(options =>
        {
          options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
          options.SignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;
          options.Authority = @"https://localhost:8181/realms/quickstart";
          options.ClientId = "8814267c-25fc-459e-b0a6-f6d7ed056f12";
          options.ClientSecret = "asdfélaksdjfaélskdfjéalskdjf";
          options.ResponseType = OpenIdConnectResponseType.Code;
          options.SaveTokens = true;
          options.MapInboundClaims = false;
          options.Scope.Add("api://8814267c-25fc-459e-b0a6-f6d7ed056f12/games:all");
          options.TokenValidationParameters.NameClaimType = JwtRegisteredClaimNames.Name;
          options.TokenValidationParameters.RoleClaimType = "roles";
        })
        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);
      ;

      builder.Services.AddAuthorizationBuilder()
       .AddPolicy("user_access", builder =>
       {
         builder.RequireClaim(ClaimTypes.Role, "user", "admin");
       })
       .AddPolicy("admin_access", builder =>
       {
         builder.RequireClaim(ClaimTypes.Role, "admin");
       });

      var app = builder.Build();

      app.UseDefaultFiles();
      app.UseStaticFiles();

      // Configure the HTTP request pipeline.
      if (app.Environment.IsDevelopment())
      {
        app.UseSwagger();
        app.UseSwaggerUI();
      }

      app.UseHttpLogging();

      app.UseHttpsRedirection();

      app.UseAuthorization();


      app.MapControllers();
      app.MapFallbackToFile("/index.html");

      app.Run();

      

      using var context = new WeatherContext();

      context.Database.EnsureDeleted();
      context.Database.EnsureCreated();

      context.AddRange(
          new Blog { Name = "Blog1", Url = "http://blog1.com" },
          new Blog { Name = "Blog2", Url = "http://blog2.com" });

      context.SaveChanges();
    }
  }
}
