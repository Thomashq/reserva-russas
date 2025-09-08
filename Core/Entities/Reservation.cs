using RR.Core.Entities.Base;
using RR.Core.Enums;

namespace RR.Core.Entities;

public class Reservation: BaseEntity 
{
    public int Id { get; set; }

    public int RoomId { get; set; }
    public int AccountId { get; set; }

    public string Title { get; set; }
    public string Description { get; set; }

    // Liga à série (null = reserva avulsa)
    public int? SeriesId { get; set; }
    public EReservationOrigin Origin { get; set; } = EReservationOrigin.Manual;

    // Se foi movida, referencia a original cancelada
    public int? MovedFromReservationId { get; set; }

    // Seu fluxo de aprovação atual (mantenha como está)
    // 0-criado 1-aprovado 2-rejeitado/cancelado
    public int Status { get; set; }

    // Intervalo da ocorrência (LOCAL)
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    // Navegações
    public virtual Rooms Room { get; set; }
    public virtual Account Account { get; set; }
    public virtual ReservationSeries Series { get; set; }
    public virtual Reservation MovedFromReservation { get; set; }
    public virtual ICollection<ReservationException> Exceptions { get; set; } = new List<ReservationException>();
}

