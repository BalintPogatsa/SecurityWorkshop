
using Microsoft.AspNetCore.Authentication.JwtBearer;

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

      builder.Services.AddAuthentication()
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
          options.Authority = "https://localhost:8181/realms/quickstart";
          options.Audience = "api://8814267c-25fc-459e-b0a6-f6d7ed056f12";
          options.MapInboundClaims = false;
        });

      builder.Services.AddAuthorizationBuilder()
       .AddPolicy("read_access", builder =>
       {
         builder.RequireClaim("scp", "workshop:read", "workshop:all");
       })
       .AddPolicy("write_access", builder =>
       {
         builder.RequireClaim("scp", "workshop:write", "workshop:all");
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

      app.UseHttpsRedirection();

      app.UseAuthorization();


      app.MapControllers();

      app.MapFallbackToFile("/index.html");

      app.Run();
    }
  }
}
