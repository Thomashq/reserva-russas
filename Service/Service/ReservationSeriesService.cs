using RR.Core.DTOs.Requests;
using RR.Core.Entities;
using RR.Core.Enums;
using RR.Core.Repositories;
using RR.Core.Enums;
using RR.Core.Services;
using RR.Util.ReservationRules;

namespace RR.Service.Service
{
    public class ReservationSeriesService : IReservationSeriesService
    {
        private readonly IReservationSeriesRepository _reservationSeriesRepository;
        private readonly IReservationService _reservationService;
        private readonly IAccountRepository _accounts;

        public ReservationSeriesService(IReservationSeriesRepository reservationSeriesRepository, IReservationService reservationService, IAccountRepository accounts)
        {
            _reservationSeriesRepository = reservationSeriesRepository;
            _accounts = accounts;
            _reservationService = reservationService;
        }

        public async Task<int> CancelSeries(int seriesId, DateTime? from = null)
        {
            var series = await _reservationSeriesRepository.GetSeriesById(seriesId);
            if (series == null) return 0;

            var cutoff = (from?.ToUniversalTime() ?? DateTime.UtcNow);

            var all = await _reservationService.GetReservationsBySeriesId(seriesId);
            var target = all
                .Where(r => r.IsActive && r.EndTime.ToUniversalTime() >= cutoff)
                .ToList();

            foreach (var r in target)
            {
                r.IsActive = false; // soft delete
                await _reservationService.DeleteAsync(r.Id);
            }

            // (Opcional simples) marcar a série como cancelada
            series.IsActive = true; // Cancelled
            await _reservationSeriesRepository.UpdateSeries(series);

            return target.Count;
        }

        public async Task<int> CreateSeries(CreateSeriesRequest series)
        {
            ReservationSeries reservationSeries = new ReservationSeries
            {
                AccountId = series.AccountId,
                RoomId = series.DefaultRoomId,
                Title = series.Title,
                Description = series.Description,
                WindowStart = series.WindowStart,
                WindowEnd = series.WindowEnd,
                RecurrenceRule = series.RecurrenceRule,
                DaysOfWeek = series.DaysOfWeek,
                TimeStart = series.TimeStart,
                TimeEnd = series.TimeEnd,
                SeriesStatus = 1 // Active
            };
            await _reservationSeriesRepository.AddSeries(reservationSeries);

            return await MakeReservationsAsync(reservationSeries, series.Interval);
        }

        public async Task<int> EditSeries(EditSeriesRequest req)
        {
            var series = await _reservationSeriesRepository.GetSeriesById(req.SeriesId);
            if (series == null) return 0;

            // Atualiza somente o que veio
            series.Title = req.Title ?? series.Title;
            series.Description = req.Description ?? series.Description;
            series.WindowStart = req.WindowStart ?? series.WindowStart;
            series.WindowEnd = req.WindowEnd ?? series.WindowEnd;
            series.RecurrenceRule = req.RecurrenceRule ?? series.RecurrenceRule;
            series.DaysOfWeek = req.DaysOfWeek ?? series.DaysOfWeek;
            series.TimeStart = req.TimeStart ?? series.TimeStart;
            series.TimeEnd = req.TimeEnd ?? series.TimeEnd;

            await _reservationSeriesRepository.UpdateSeries(series);

            // Abordagem simples: desativar todas as reservas atuais e recriar
            var existing = await _reservationService.GetReservationsBySeriesId(req.SeriesId);
            foreach (var r in existing.Where(x => x.IsActive))
            {
                r.IsActive = false;
                await _reservationService.UpdateAsync(r);
            }

            var interval = req.Interval; // assuma que vem no request
            var created = await MakeReservationsAsync(series, interval);
            return created;
        }

        public Task<List<Reservation>> PreviewSeries(PreviewSeriesRequest series)
        {
            // Somente expandir, SEM persistir
            EReservationDay daysMask = ReservationSeriesParser.ParseMask(series.DaysOfWeek);
            EReservationFrequency frequency = ReservationSeriesParser.ParseMaskFrequency(series.RecurrenceRule);

            var dates = WeekdayHelper.GenerateDates(
                series.WindowStart,
                series.WindowEnd,
                daysMask,
                frequency,
                series.Interval
            );

            var preview = new List<Reservation>(capacity: dates.Count);
            foreach (var date in dates)
            {
                var startTime = date.Date + series.TimeStart;
                var endTime = date.Date + series.TimeEnd;

                preview.Add(new Reservation
                {
                    AccountId = series.AccountId,
                    RoomId = series.DefaultRoomId,
                    Title = series.Title,
                    Description = series.Description,
                    StartTime = startTime,
                    EndTime = endTime,
                    Origin = EReservationOrigin.SeriesGenerated,
                    IsActive = true
                });
            }

            return Task.FromResult(preview);
        }

        private async Task<int> MakeReservationsAsync(ReservationSeries reservationSeries, int interval)
        {
            EReservationDay daysMask = ReservationSeriesParser.ParseMask(reservationSeries.DaysOfWeek);
            EReservationFrequency frequency = ReservationSeriesParser.ParseMaskFrequency(reservationSeries.RecurrenceRule);

            var dates = WeekdayHelper.GenerateDates(
                reservationSeries.WindowStart,
                reservationSeries.WindowEnd,
                daysMask,
                frequency,
                interval);

            int count = 0;
            foreach (var date in dates)
            {
                var startTime = date.Date + reservationSeries.TimeStart;
                var endTime = date.Date + reservationSeries.TimeEnd;
                var reservation = new Reservation
                {
                    AccountId = reservationSeries.AccountId,
                    RoomId = reservationSeries.RoomId,
                    Title = reservationSeries.Title,
                    Description = reservationSeries.Description,
                    SeriesId = reservationSeries.Id,
                    Origin = EReservationOrigin.SeriesGenerated,
                    StartTime = startTime,
                    EndTime = endTime,
                    IsActive = true
                };
                await _reservationService.AddAsync(reservation);
                count++;
            }

            return count;
        }
    }
}
