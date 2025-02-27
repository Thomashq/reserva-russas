using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    public DbSet<Account> Account { get; set; }
    public DbSet<Student> Student { get; set; }
    public DbSet<Servant> Servant { get; set; }
    public DbSet<Manager> Manager { get; set; }
    public DbSet<Reservation> Reservation { get; set; }
    public DbSet<Rooms> Room { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<Manager>()
            .HasOne(m => m.Account)
            .WithMany()
            .HasForeignKey(m => m.AccountId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<Student>()
            .HasOne(s => s.Account)
            .WithMany()
            .HasForeignKey(s => s.AccountId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<Servant>()
            .HasOne(s => s.Account)
            .WithMany()
            .HasForeignKey(s => s.AccountId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<Rooms>()
            .HasOne(s => s.Manager)
            .WithMany()
            .HasForeignKey(s => s.ManagerId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }

}

