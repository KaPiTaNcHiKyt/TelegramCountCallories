using CountCalloriesDataAccessLayer.Configurations;
using CountCalloriesDataAccessLayer.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace CountCalloriesDataAccessLayer;

public class CountCalloriesContext(DbContextOptions<CountCalloriesContext> options) 
    : DbContext(options)
{
    public DbSet<UserEntity> Users { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        
        base.OnModelCreating(modelBuilder);
    }

    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=TelegramCount;Username=stanislav;Password=1234;");
    //     base.OnConfiguring(optionsBuilder);
    // }
}