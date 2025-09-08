using RR.Core.Enums;

namespace RR.Util.ReservationRules
{
    public static class WeekdayHelper
    {
        /// <summary>
        /// Gera as datas específicas baseado nos parâmetros
        /// </summary>
        public static List<DateTime> GenerateDates(
            DateTime windowStart,
            DateTime windowEnd,
            EReservationDay daysMask,
            EReservationFrequency frequency,
            int interval)
        {
            var dates = new List<DateTime>();

            switch (frequency)
            {
                case EReservationFrequency.WEEKLY:
                    dates = GenerateWeeklyDates(windowStart, windowEnd, daysMask, interval);
                    break;

                case EReservationFrequency.MONTHLY:
                    dates = GenerateMonthlyDates(windowStart, windowEnd, daysMask, interval);
                    break;

                case EReservationFrequency.YEARLY:
                    dates = GenerateYearlyDates(windowStart, windowEnd, daysMask, interval);
                    break;
            }

            return dates.OrderBy(d => d).ToList();
        }

        /// <summary>
        /// Gera datas semanais
        /// </summary>
        private static List<DateTime> GenerateWeeklyDates(
            DateTime windowStart,
            DateTime windowEnd,
            EReservationDay daysMask,
            int interval)
        {
            var dates = new List<DateTime>();
            var startOfWeek = GetStartOfWeek(windowStart);
            var currentWeek = startOfWeek;

            while (currentWeek <= windowEnd)
            {
                for (int dayOffset = 0; dayOffset < 7; dayOffset++)
                {
                    var currentDate = currentWeek.AddDays(dayOffset);

                    if (currentDate < windowStart.Date || currentDate > windowEnd.Date)
                        continue;

                    if (IsDayIncluded(currentDate.DayOfWeek, daysMask))
                    {
                        dates.Add(currentDate);
                    }
                }

                currentWeek = currentWeek.AddDays(7 * interval);
            }

            return dates;
        }

        /// <summary>
        /// Gera datas mensais
        /// </summary>
        private static List<DateTime> GenerateMonthlyDates(
            DateTime windowStart,
            DateTime windowEnd,
            EReservationDay daysMask,
            int interval)
        {
            var dates = new List<DateTime>();
            var currentMonth = new DateTime(windowStart.Year, windowStart.Month, 1);

            while (currentMonth <= windowEnd)
            {
                var daysInMask = GetDaysFromMask(daysMask);

                foreach (var targetDay in daysInMask)
                {
                    var occurrences = GetDayOccurrencesInMonth(currentMonth, targetDay);

                    foreach (var occurrence in occurrences)
                    {
                        if (occurrence >= windowStart.Date && occurrence <= windowEnd.Date)
                        {
                            dates.Add(occurrence);
                        }
                    }
                }

                currentMonth = currentMonth.AddMonths(interval);
            }

            return dates;
        }

        /// <summary>
        /// Gera datas anuais
        /// </summary>
        private static List<DateTime> GenerateYearlyDates(
            DateTime windowStart,
            DateTime windowEnd,
            EReservationDay daysMask,
            int interval)
        {
            var dates = new List<DateTime>();
            var currentYear = windowStart.Year;

            while (new DateTime(currentYear, 1, 1) <= windowEnd)
            {
                var yearStart = new DateTime(currentYear, windowStart.Month, windowStart.Day);

                if (yearStart >= windowStart.Date && yearStart <= windowEnd.Date)
                {
                    if (IsDayIncluded(yearStart.DayOfWeek, daysMask))
                    {
                        dates.Add(yearStart);
                    }
                }

                currentYear += interval;
            }

            return dates;
        }

        #region Helper Methods

        private static DateTime GetStartOfWeek(DateTime date)
        {
            var diff = (7 + (date.DayOfWeek - DayOfWeek.Sunday)) % 7;
            return date.AddDays(-diff).Date;
        }

        private static bool IsDayIncluded(DayOfWeek dayOfWeek, EReservationDay daysMask)
        {
            var dayFlag = dayOfWeek switch
            {
                DayOfWeek.Monday => EReservationDay.MO,
                DayOfWeek.Tuesday => EReservationDay.TU,
                DayOfWeek.Wednesday => EReservationDay.WE,
                DayOfWeek.Thursday => EReservationDay.TH,
                DayOfWeek.Friday => EReservationDay.FR,
                DayOfWeek.Saturday => EReservationDay.SA,
                DayOfWeek.Sunday => EReservationDay.SU,
                _ => EReservationDay.None
            };

            return daysMask.HasFlag(dayFlag);
        }

        private static List<DayOfWeek> GetDaysFromMask(EReservationDay daysMask)
        {
            var days = new List<DayOfWeek>();

            if (daysMask.HasFlag(EReservationDay.SU)) days.Add(DayOfWeek.Sunday);
            if (daysMask.HasFlag(EReservationDay.MO)) days.Add(DayOfWeek.Monday);
            if (daysMask.HasFlag(EReservationDay.TU)) days.Add(DayOfWeek.Tuesday);
            if (daysMask.HasFlag(EReservationDay.WE)) days.Add(DayOfWeek.Wednesday);
            if (daysMask.HasFlag(EReservationDay.TH)) days.Add(DayOfWeek.Thursday);
            if (daysMask.HasFlag(EReservationDay.FR)) days.Add(DayOfWeek.Friday);
            if (daysMask.HasFlag(EReservationDay.SA)) days.Add(DayOfWeek.Saturday);

            return days;
        }

        private static List<DateTime> GetDayOccurrencesInMonth(DateTime month, DayOfWeek targetDay)
        {
            var occurrences = new List<DateTime>();
            var firstDay = new DateTime(month.Year, month.Month, 1);
            var lastDay = firstDay.AddMonths(1).AddDays(-1);

            var current = firstDay;
            while (current.DayOfWeek != targetDay)
            {
                current = current.AddDays(1);
            }

            while (current <= lastDay)
            {
                occurrences.Add(current);
                current = current.AddDays(7);
            }

            return occurrences;
        }

        #endregion
    }
}