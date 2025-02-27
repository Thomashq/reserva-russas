using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure;

public class DataContext:DbContext
{
  public DataContext(DbContextOptions<DataContext> options) : base(options) { }  

  public DbSet<Account> Account {get;set;}
  public DbSet<Student> Student {get;set;}
  public DbSet<Servant> Servant {get;set;}
  public DbSet<Manager> Manager {get;set;}
  public DbSet<Reservation> Reservation {get;set;}
  public DbSet<Rooms> Room {get;set;}

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    //Configuração automática para todas as classes que herdam de BaseModel
    foreach (var entityType in modelBuilder.Model.GetEntityTypes())
    {
        if (typeof(BaseModel).IsAssignableFrom(entityType.ClrType))
        {
            var entity = modelBuilder.Entity(entityType.ClrType);
            entity.Property<long>("Id").HasColumnName($"{entityType.ClrType.Name}Id");
        }
    }
  }
}
