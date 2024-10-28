using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecurityWorkshop.Server.DataAccess;
using System;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace SecurityWorkshop.Server.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class WeatherForecastController : ControllerBase
  {
    private static readonly string[] Summaries = new[]
    {
          "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
      };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
      _logger = logger;
    }

    [Authorize("user_access")]
    [HttpGet("forecast")]
    public IEnumerable<WeatherForecast> Get()
    {
      return Enumerable.Range(1, 5).Select(index => new WeatherForecast
      {
        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
        TemperatureC = Random.Shared.Next(-20, 55),
        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
      })
      .ToArray();
    }

    [Authorize("admin_access")]
    [HttpGet("data")]
    public IEnumerable<WeatherForecast> GetData()
    {
      var myhash = MD5.Create();

      var hashedString = myhash.ComputeHash(new byte[] { });

      return Enumerable.Range(1, 5).Select(index => new WeatherForecast
      {
        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-6 + index)),
        TemperatureC = Random.Shared.Next(-20, 55),
        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
      })
      .ToArray();
    }

    [HttpGet("datafilter")]
    public List<WeatherForecast> GetData([FromQuery] string search)
    {
      using (var context = new WeatherContext())
      {
        var blogs = context.Blogs.FromSql($"SELECT * FROM Blogs WHERE Name LIKE {search} OR Description LIKE {search}").ToList();
        Debug.WriteLine("Blogs in db: " + blogs.Count());

        return new List<WeatherForecast>();
      }
      
    }
  }
}
