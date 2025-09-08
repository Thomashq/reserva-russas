/* ao criar uma reserva recorrente, um evento pode acontecer e de comum acordo as partes reservantes podem querer alterar uma ou mais ocorrências específicas sem afetar toda a série. 
   Para isso, podemos criar uma entidade separada chamada "ReservationException" que armazenará essas exceções. 
   Essa entidade pode conter informações como a data da ocorrência específica, o motivo da alteração e quaisquer detalhes adicionais relevantes. */
using RR.Core.Entities.Base;
using RR.Core.Enums;

namespace RR.Core.Entities
{
    public class ReservationException : BaseEntity
    {
        public int Id { get; set; }

        public int SeriesId { get; set; }
        public int? ReservationId { get; set; } // ocorrência alvo (Skip) ou original (Move)

        public EReservationExceptionAction Action { get; set; } // Skip | Move

        // Data local da série que identifica a ocorrência original
        public DateTime OriginalDate { get; set; }

        public DateTime ExceptionDate { get; set; }

        // Preenchidos no caso de Move (LOCAL)
        public DateTime? NewStartTime { get; set; }
        public DateTime? NewEndTime { get; set; }
        public int? NewRoomId { get; set; }

        public string Reason { get; set; }

        // Navegações
        public virtual ReservationSeries Series { get; set; }
        public virtual Reservation Reservation { get; set; }
        public virtual Rooms NewRoom { get; set; }
    }
}
