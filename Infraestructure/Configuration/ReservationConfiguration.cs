using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RR.Core.Entities;

namespace RR.Infraestructure.Configuration
{
    // ------------------------------
    // Reservation (OCCURRENCE)
    // ------------------------------
    public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
    {
        public void Configure(EntityTypeBuilder<Reservation> builder)
        {
            builder.ToTable("reservation");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasColumnName("id")
                .IsRequired()
                .ValueGeneratedOnAdd();

            // FKs básicas
            builder.Property(x => x.RoomId)
                .HasColumnName("room_id")
                .IsRequired();

            builder.Property(x => x.AccountId)
                .HasColumnName("account_id")
                .IsRequired();

            // Campos textuais
            builder.Property(x => x.Title)
                .HasColumnName("title")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasColumnName("description")
                .HasMaxLength(1000);

            // Série / origem / movimento
            builder.Property(x => x.SeriesId)
                .HasColumnName("series_id");

            builder.Property(x => x.Origin)
                .HasColumnName("origin") // 0=Manual 1=SeriesGenerated 2=ExceptionMove
                .IsRequired();

            builder.Property(x => x.MovedFromReservationId)
                .HasColumnName("moved_from_reservation_id");

            // Status
            builder.Property(x => x.Status)
                .HasColumnName("status")
                .IsRequired();

            // Intervalo (LOCAL)
            builder.Property(x => x.StartTime)
                .HasColumnName("start_time")
                .HasColumnType("timestamp without time zone")
                .IsRequired();

            builder.Property(x => x.EndTime)
                .HasColumnName("end_time")
                .HasColumnType("timestamp without time zone")
                .IsRequired();

            // Relacionamentos essenciais
            builder.HasOne(r => r.Series)
                .WithMany(s => s.Reservations)
                .HasForeignKey(r => r.SeriesId)
                .HasConstraintName("fk_reservation_series_id")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.MovedFromReservation)
                .WithMany()
                .HasForeignKey(r => r.MovedFromReservationId)
                .HasConstraintName("fk_reservation_moved_from_id")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(r => r.Exceptions)
                .WithOne(e => e.Reservation)
                .HasForeignKey(e => e.ReservationId)
                .HasConstraintName("fk_reservation_exception_reservation_id")
                .OnDelete(DeleteBehavior.Cascade);

            // Índices
            builder.HasIndex(x => x.RoomId)
                .HasDatabaseName("ix_reservation_room_id");

            builder.HasIndex(x => x.AccountId)
                .HasDatabaseName("ix_reservation_account_id");

            builder.HasIndex(x => x.StartTime)
                .HasDatabaseName("ix_reservation_start_time");

            builder.HasIndex(x => x.EndTime)
                .HasDatabaseName("ix_reservation_end_time");

            builder.HasIndex(x => new { x.RoomId, x.StartTime, x.EndTime })
                .HasDatabaseName("ix_reservation_room_time_range");

            builder.HasIndex(x => new { x.SeriesId, x.StartTime })
                .HasDatabaseName("ix_reservation_series_start");

            // Idempotência das ocorrências geradas por série
            builder.HasIndex(x => new { x.SeriesId, x.StartTime, x.RoomId })
                .IsUnique()
                .HasDatabaseName("ux_reservation_series_start_room");

            // Constraint: EndTime > StartTime
            builder.ToTable(t => t.HasCheckConstraint(
                "ck_reservation_end_after_start",
                "end_time > start_time"));
        }
    }

    public class ReservationSeriesConfiguration : IEntityTypeConfiguration<ReservationSeries>
    {
        public void Configure(EntityTypeBuilder<ReservationSeries> builder)
        {
            builder.ToTable("reservation_series");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasColumnName("id")
                .IsRequired()
                .ValueGeneratedOnAdd();

            // Dono (Account)
            builder.Property(x => x.AccountId)
                .HasColumnName("account_id")
                .IsRequired();

            builder.HasOne(x => x.Account)
                .WithMany()
                .HasForeignKey(x => x.AccountId)
                .HasConstraintName("fk_reservation_series_account_id")
                .OnDelete(DeleteBehavior.Restrict);

            // Sala padrão: tua entidade usa RoomId, mas a coluna será 'default_room_id'
            builder.Property(x => x.RoomId)
                .HasColumnName("default_room_id");

            // Título / Descrição padrão
            builder.Property(x => x.Title)
                .HasColumnName("title")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasColumnName("description")
                .HasMaxLength(1000);

            // Janela (datas locais)
            builder.Property(x => x.WindowStart)
                .HasColumnName("window_start")
                .HasColumnType("date")
                .IsRequired();

            builder.Property(x => x.WindowEnd)
                .HasColumnName("window_end")
                .HasColumnType("date")
                .IsRequired();

            // RRULE e normalizados
            builder.Property(x => x.RecurrenceRule)
                .HasColumnName("recurrence_rule")
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(x => x.DaysOfWeek)
                .HasColumnName("days_of_week")
                .HasMaxLength(32);

            builder.Property(x => x.TimeStart)
                .HasColumnName("time_start")
                .HasColumnType("time without time zone")
                .IsRequired();

            builder.Property(x => x.TimeEnd)
                .HasColumnName("time_end")
                .HasColumnType("time without time zone")
                .IsRequired();

            builder.Property(x => x.SeriesStatus)
                .HasColumnName("series_status")
                .IsRequired();

            // Índices
            builder.HasIndex(x => x.AccountId)
                .HasDatabaseName("ix_reservation_series_account_id");

            builder.HasIndex(x => x.RoomId)
                .HasDatabaseName("ix_reservation_series_default_room_id");

            builder.HasIndex(x => x.WindowStart)
                .HasDatabaseName("ix_reservation_series_window_start");

            // Constraint: window_end >= window_start
            builder.ToTable(t => t.HasCheckConstraint(
                "ck_reservation_series_window_end_after_start",
                "window_end >= window_start"));
        }
    }

    // ------------------------------
    // ReservationException (LOCAL)
    // ------------------------------
    public class ReservationExceptionConfiguration : IEntityTypeConfiguration<ReservationException>
    {
        public void Configure(EntityTypeBuilder<ReservationException> builder)
        {
            builder.ToTable("reservation_exception");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasColumnName("id")
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(x => x.ReservationId)
                .HasColumnName("reservation_id")
                .IsRequired();

            builder.Property(x => x.ExceptionDate)
                .HasColumnName("exception_date")
                .HasColumnType("timestamp without time zone")
                .IsRequired();

            builder.Property(x => x.Reason)
                .HasColumnName("reason")
                .HasMaxLength(1000);

            builder.HasOne(x => x.Reservation)
               .WithMany(r => r.Exceptions)
               .HasForeignKey(x => x.ReservationId)
               .HasConstraintName("fk_reservation_exception_reservation_id")
               .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.ReservationId, x.ExceptionDate })
                .HasDatabaseName("ix_reservation_exception_reservation_date");
        }
    }
}
