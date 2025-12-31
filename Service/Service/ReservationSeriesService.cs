using Microsoft.EntityFrameworkCore;
using RR.Core.DTOs.Requests;
using RR.Core.Entities;
using RR.Core.Enums;
using RR.Core.Services;
using RR.Infraestructure.DataContext;

namespace RR.Service.Service
{
    public class ReservationSeriesService : IReservationSeriesService
    {
        private readonly ApplicationDbContext _context;
        private readonly IReservationService _reservationService;

        public ReservationSeriesService(ApplicationDbContext context, IReservationService reservationService) { _context = context; _reservationService = reservationService; }

        public async Task<int> CreateSeries(CreateSeriesRequest series)
        {
            var acc = await _context.Account.FirstOrDefaultAsync(a => a.Id == series.AccountId);
            if (acc is not null && acc.AccountPermission == (int)EAccountPermission.Student)
                throw new UnauthorizedAccessException("Alunos não podem reservar salas.");

            var reservationSeries = new ReservationSeries
            {
                AccountId = series.AccountId,
                RoomId = series.DefaultRoomId,
                Title = series.Title,
                Description = series.Description ?? "",
                WindowStart = series.WindowStart,
                WindowEnd = series.WindowEnd,
                RecurrenceRule = series.RecurrenceRule ?? "",
                DaysOfWeek = series.DaysOfWeek ?? "",
                TimeStart = series.TimeStart,
                TimeEnd = series.TimeEnd,
                SeriesStatus = 1,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.ReservationSeries.AddAsync(reservationSeries);
            await _context.SaveChangesAsync();

            var preview = await PreviewSeries(new PreviewSeriesRequest
            {
                AccountId = series.AccountId,
                DefaultRoomId = series.DefaultRoomId,
                Title = series.Title,
                Description = series.Description,
                WindowStart = series.WindowStart,
                WindowEnd = series.WindowEnd,
                DaysOfWeek = series.DaysOfWeek,
                TimeStart = series.TimeStart,
                TimeEnd = series.TimeEnd,
                Interval = series.Interval
            });

            foreach (var r in preview)
            {
                r.SeriesId = reservationSeries.Id;
                await _reservationService.AddAsync(r);
            }

            return reservationSeries.Id;
        }

        public Task<List<Reservation>> PreviewSeries(PreviewSeriesRequest series)
        {
            if (series.WindowStart > series.WindowEnd) throw new ArgumentException("WindowStart must be before WindowEnd.");

            var days = ParseDaysOfWeek(series.DaysOfWeek);
            var results = new List<Reservation>();

            var startDate = series.WindowStart.Date;
            var endDate = series.WindowEnd.Date;

            for (var d = startDate; d <= endDate; d = d.AddDays(1))
            {
                if (!days.Contains(d.DayOfWeek)) continue;

                // Interval semanal simples (1 = toda semana)
                if (series.Interval > 1)
                {
                    var weeksFromStart = (int)((d - startDate).TotalDays / 7);
                    if (weeksFromStart % series.Interval != 0) continue;
                }

                var start = d.Add(series.TimeStart);
                var end = d.Add(series.TimeEnd);
                if (start >= end) continue;

                results.Add(new Reservation
                {
                    AccountId = series.AccountId,
                    RoomId = series.DefaultRoomId,
                    Title = series.Title,
                    Description = series.Description ?? "",
                    StartTime = start,
                    EndTime = end,
                    Status = 0,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            return Task.FromResult(results);
        }

        public async Task<int> EditSeries(EditSeriesRequest series)
        {
            var entity = await _context.ReservationSeries.FirstOrDefaultAsync(x => x.Id == series.SeriesId && x.IsActive);
            if (entity == null) throw new Exception("Série não encontrada");

            if (series.Title is not null) entity.Title = series.Title;
            if (series.Description is not null) entity.Description = series.Description;
            if (series.WindowStart.HasValue) entity.WindowStart = series.WindowStart.Value;
            if (series.WindowEnd.HasValue) entity.WindowEnd = series.WindowEnd.Value;
            if (series.RecurrenceRule is not null) entity.RecurrenceRule = series.RecurrenceRule;
            if (series.DaysOfWeek is not null) entity.DaysOfWeek = series.DaysOfWeek;
            if (series.TimeStart.HasValue) entity.TimeStart = series.TimeStart.Value;
            if (series.TimeEnd.HasValue) entity.TimeEnd = series.TimeEnd.Value;

            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<int> CancelSeries(int seriesId, DateTime? from = null)
        {
            var entity = await _context.ReservationSeries.FirstOrDefaultAsync(x => x.Id == seriesId && x.IsActive);
            if (entity == null) return 0;

            entity.IsActive = false;
            entity.UpdatedAt = DateTime.UtcNow;

            var q = _context.Reservation.Where(r => r.SeriesId == seriesId && r.IsActive);
            if (from.HasValue) q = q.Where(r => r.StartTime >= from.Value);

            var reservations = await q.ToListAsync();
            foreach (var r in reservations)
            {
                r.IsActive = false;
                r.Status = 2;
                r.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return entity.Id;
        }

        private static HashSet<DayOfWeek> ParseDaysOfWeek(string? daysOfWeek)
        {
            var set = new HashSet<DayOfWeek>();
            if (string.IsNullOrWhiteSpace(daysOfWeek)) return set;

            var parts = daysOfWeek.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var p in parts)
            {
                switch (p.ToUpper())
                {
                    case "MO": set.Add(DayOfWeek.Monday); break;
                    case "TU": set.Add(DayOfWeek.Tuesday); break;
                    case "WE": set.Add(DayOfWeek.Wednesday); break;
                    case "TH": set.Add(DayOfWeek.Thursday); break;
                    case "FR": set.Add(DayOfWeek.Friday); break;
                    case "SA": set.Add(DayOfWeek.Saturday); break;
                    case "SU": set.Add(DayOfWeek.Sunday); break;
                }
            }

            return set;
        }
    }
}
