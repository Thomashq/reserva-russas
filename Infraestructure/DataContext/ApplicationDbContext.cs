using Microsoft.EntityFrameworkCore;
using RR.Core.Entities.Base;
using System.Reflection;
using RR.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace RR.Infraestructure.DataContext
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Account> Account { get; set; }
        public DbSet<Manager> Manager { get; set; } 
        public DbSet<Reservation> Reservation { get; set; }
        public DbSet<Rooms> Rooms { get; set; }
        public DbSet<Servant> Servant { get; set; }
        public DbSet<Student> Student { get; set; }
        public DbSet<StudentAdvisor> StudentAdvisor { get; set; } 
        public DbSet<StudentPermission> StudentPermission { get; set; } 
        public DbSet<ReservationException> ReservationException { get; set; }
        public DbSet<ReservationSeries> ReservationSeries { get; set; }
        public DbSet<RoomDetails> RoomDetails { get; set; }
        public DbSet<Equipment> Equipment { get; set; }
        public DbSet<RoomEquipment> RoomEquipment { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            ConfigureGlobalSettings(modelBuilder);

            ConfigureBaseEntityProperties(modelBuilder);
        }

        private void ConfigureGlobalSettings(ModelBuilder modelBuilder)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entity.GetTableName();
                if (!string.IsNullOrEmpty(tableName))
                {
                    entity.SetTableName(ToSnakeCase(tableName));
                }

                foreach (var property in entity.GetProperties())
                {
                    var columnName = property.GetColumnName();
                    if (!string.IsNullOrEmpty(columnName) && columnName == property.Name)
                    {
                        property.SetColumnName(ToSnakeCase(columnName));
                    }
                }
            }
        }

        private void ConfigureBaseEntityProperties(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(IBaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .Property(nameof(IBaseEntity.CreatedAt))
                        .HasColumnName("created_at")
                        .IsRequired()
                        .HasDefaultValueSql("NOW()");

                    modelBuilder.Entity(entityType.ClrType)
                        .Property(nameof(IBaseEntity.UpdatedAt))
                        .HasColumnName("updated_at")
                        .IsRequired()
                        .HasDefaultValueSql("NOW()");

                    modelBuilder.Entity(entityType.ClrType)
                        .Property(nameof(IBaseEntity.IsActive))
                        .HasColumnName("is_active")
                        .IsRequired()
                        .HasDefaultValue(true);

                    modelBuilder.Entity(entityType.ClrType)
                        .HasIndex(nameof(IBaseEntity.IsActive))
                        .HasDatabaseName($"ix_{ToSnakeCase(entityType.GetTableName() ?? entityType.ClrType.Name)}_is_active");

                    modelBuilder.Entity(entityType.ClrType)
                        .HasIndex(nameof(IBaseEntity.CreatedAt))
                        .HasDatabaseName($"ix_{ToSnakeCase(entityType.GetTableName() ?? entityType.ClrType.Name)}_created_at");
                }
            }
        }

        private string ToSnakeCase(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            return string.Concat(
                input.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x : x.ToString()))
                .ToLower();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<IBaseEntity>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                }
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            var entries = ChangeTracker.Entries<IBaseEntity>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                }
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }

            return base.SaveChanges();
        }
    }
}
