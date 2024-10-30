
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
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

      builder.Services.AddHttpLogging(logging => {
        logging.LoggingFields = HttpLoggingFields.All;
        // Only for testing
        //logging.RequestHeaders.Add("Authorization");
        //logging.ResponseHeaders.Add("WWW-Authenticate");
      });

      builder.Services.AddTransient<IClaimsTransformation, ClaimsTransformer>();

      builder.Services.AddAuthentication(options =>
        {
          options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
          options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
          options.MetadataAddress = "https://localhost:8181/realms/quickstart/.well-known/openid-configuration";
          options.Authority = @"https://localhost:8181/realms/quickstart";
          options.Audience = "account";
          options.MapInboundClaims = false;
          options.TokenValidationParameters = new TokenValidationParameters
          {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
          };
        })
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

      

      
    }
  }
}
