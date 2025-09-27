
using RR.Core.Enums;

namespace RR.Util.ReservationRules
{
    public static class ReservationSeriesParser
    {
        public static EReservationDay ParseMask(string reserva)
        {
            if (string.IsNullOrWhiteSpace(reserva)) return EReservationDay.None;

            var parts = reserva.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            EReservationDay mask = EReservationDay.None;

            foreach (var p in parts)
            {
                mask |= p switch
                {
                    "MO" => EReservationDay.MO,
                    "TU" => EReservationDay.TU,
                    "WE" => EReservationDay.WE,
                    "TH" => EReservationDay.TH,
                    "FR" => EReservationDay.FR,
                    "SA" => EReservationDay.SA,
                    "SU" => EReservationDay.SU,
                    _ => throw new ArgumentException($"Dia inválido: {p}")
                };
            }
            return mask;
        }

        public static EReservationFrequency ParseMaskFrequency(string reserva)
        {
            if (string.IsNullOrWhiteSpace(reserva))
                return EReservationFrequency.WEEKLY;

            return reserva.ToUpper() switch
            {
                "MONTHLY" => EReservationFrequency.MONTHLY,
                "WEEKLY" => EReservationFrequency.WEEKLY,
                "YEARLY" => EReservationFrequency.YEARLY,
                _ => throw new ArgumentException($"Frequência inválida: {reserva}")
            };
        }
    }
}
