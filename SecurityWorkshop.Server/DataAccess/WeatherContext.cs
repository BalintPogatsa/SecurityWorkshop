using System;
using System.Reflection.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;

namespace SecurityWorkshop.Server.DataAccess
{

  public class WeatherContext : DbContext
  {
    private readonly Action<WeatherContext, ModelBuilder> _modelCustomizer;

    public string DbPath { get; }

    #region Constructors
    public WeatherContext()
    {
      var folder = Environment.SpecialFolder.LocalApplicationData;
      var path = Environment.GetFolderPath(folder);
      DbPath = System.IO.Path.Join(path, "blogging.db");
    }

    //public WeatherContext(DbContextOptions<WeatherContext> options, Action<WeatherContext, ModelBuilder> modelCustomizer = null)
    //    : base(options)
    //{
    //  _modelCustomizer = modelCustomizer;
    //}
    #endregion

    public DbSet<Blog> Blogs => Set<Blog>();

    #region OnConfiguring
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      optionsBuilder.UseSqlite($"Data Source={DbPath}");

      //if (!optionsBuilder.IsConfigured)
      //{
      //  optionsBuilder.UseInMemoryDatabase("BloggingControllerTest")
      //  .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning));
      //}
    }
    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      
      if (_modelCustomizer is not null)
      {
        _modelCustomizer(this, modelBuilder);
      }
    }
  }
}
