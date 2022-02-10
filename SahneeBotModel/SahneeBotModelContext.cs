using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SahneeBotModel.Models;

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
    
    /// <summary>
    /// All warnings.
    /// </summary>
    public DbSet<Warning> Warnings { get; set; }
    /// <summary>
    /// All roles configured.
    /// </summary>
    public DbSet<Role> Roles { get; set; }
    /// <summary>
    /// All user & guild specific state.
    /// </summary>
    public DbSet<UserGuildState> UserGuildStates { get; set; }
    /// <summary>
    /// All user specific state.
    /// </summary>
    public DbSet<UserState> UserStates { get; set; }
    /// <summary>
    /// All guild specific state.
    /// </summary>
    public DbSet<GuildState> GuildStates { get; set; }

    /// <summary>
    /// Model edits
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
