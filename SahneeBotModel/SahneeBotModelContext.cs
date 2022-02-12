using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SahneeBotModel.Contract;
using SahneeBotModel.Models;

namespace SahneeBotModel;

public class SahneeBotModelContext : DbContext
{
    //Variables
    private readonly IConfiguration _configuration;
    private readonly IdGenerator _id;


    /// <summary>
    /// Default constructor for DI
    /// </summary>
    /// <param name="options"></param>
    /// <param name="configuration"></param>
    public SahneeBotModelContext(DbContextOptions<SahneeBotModelContext> options, IConfiguration configuration) : base(options)
    {
        _configuration = configuration;
        var machineId = long.Parse(configuration["MachineId"]);
        _id = new IdGenerator(machineId);
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
        modelBuilder.Entity<UserGuildState>().HasKey(userGuildState => new
        {
            userGuildState.UserId, 
            userGuildState.GuildId
        });
        base.OnModelCreating(modelBuilder);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        SetSnowflakesInChanges();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        SetSnowflakesInChanges();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void SetSnowflakesInChanges()
    {
        /*var changedEntities = ChangeTracker.Entries();

        foreach (var changedEntity in changedEntities)
        {
            if (changedEntity.Entity is ISnowflake snowflake && changedEntity.State == EntityState.Added)
            {
                snowflake.Id = _id.NextId();
            }
        }*/
    }
}
