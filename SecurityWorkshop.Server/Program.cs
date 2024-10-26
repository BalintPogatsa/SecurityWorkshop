
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
        });

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
