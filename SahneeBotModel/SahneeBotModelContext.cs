using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SahneeBotModel.Models;
using SahneeBotModel.Models.Warning;

namespace SahneeBotModel;

public class SahneeBotModelContext : DbContext
{
    //Variables
    private readonly IConfiguration _configuration;


    /// <summary>
    /// Default constructor for DI
    /// </summary>
    /// <param name="options"></param>
    /// <param name="configuration"></param>
    public SahneeBotModelContext(DbContextOptions<SahneeBotModelContext> options, IConfiguration configuration) : base(options)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql(_configuration.GetConnectionString("SahneeBotModelContext"));
    
    //DBSets
    public DbSet<Warning> Warnings { get; set; }
    public DbSet<Roles> Roles { get; set; }

    /// <summary>
    /// Model edits
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
