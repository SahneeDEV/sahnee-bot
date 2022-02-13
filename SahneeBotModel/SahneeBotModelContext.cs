using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SahneeBotModel.Contract;
using SahneeBotModel.Models;

// ReSharper disable UnusedAutoPropertyAccessor.Global
#pragma warning disable CS8618

namespace SahneeBotModel;

/// <summary>
/// The model context contains all database interaction of the sahnee bot.
/// </summary>
public class SahneeBotModelContext : DbContext
{
    private readonly IConfiguration _configuration;
    private readonly IdGenerator _id;

    public SahneeBotModelContext(DbContextOptions<SahneeBotModelContext> options, IConfiguration configuration, 
        IdGenerator id) : base(options)
    {
        _configuration = configuration;
        _id = id;
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Composite key for user guild state.
        modelBuilder.Entity<UserGuildState>().HasKey(userGuildState => new
        {
            userGuildState.UserId, 
            userGuildState.GuildId
        });
        base.OnModelCreating(modelBuilder);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        // Set snowflakes when saving changes
        SetSnowflakesInChanges();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        // Set snowflakes when saving changes async
        SetSnowflakesInChanges();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    /// <summary>
    /// Goes through all changes and adds a snowflake ID to all newly created entries that have a snowflake based ID.
    /// </summary>
    private void SetSnowflakesInChanges()
    { 
        // Get all changes
        var changedEntities = ChangeTracker.Entries();
        // Go through all changes
        foreach (var changedEntity in changedEntities)
        {
            // Only set ID on snowflakes that are new.
            if (changedEntity.Entity is ISnowflake snowflake && changedEntity.State == EntityState.Added)
            {
                snowflake.Id = _id.NextId();
            }
        }
    }
}
