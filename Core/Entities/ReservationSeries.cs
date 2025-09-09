using RR.Core.Entities.Base;

namespace RR.Core.Entities
{
    public class ReservationSeries : BaseEntity
    {
        public int Id { get; set; }

        // Dono / responsável
        public int AccountId { get; set; }

        public int RoomId { get; set; }

        // Rótulos padrão para ocorrências
        public string Title { get; set; }
        public string Description { get; set; }

        // Janela do semestre (usar apenas a parte da data; estará definido como 00:00:00 por padrao no front, nao trocar)
        public DateTime WindowStart { get; set; }
        public DateTime WindowEnd { get; set; }

        // RRULE (ex.: "FREQ=WEEKLY;WE;INTERVAL=1;UNTIL=20251212T235959")
        public string RecurrenceRule { get; set; }

        // Campos normalizados (evita parsear RRULE toda hora)
        public string DaysOfWeek { get; set; }  // ex.: "MO,WE"
        public TimeSpan TimeStart { get; set; } // ex.: 10:00
        public TimeSpan TimeEnd { get; set; } // ex.: 11:40

        // Status da série (separe do status das ocorrências)
        public int SeriesStatus { get; set; } // 0=Criado 1=Aceito 2=rejeitado/cancelado
    }
}
