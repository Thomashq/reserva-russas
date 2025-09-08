using System.ComponentModel.DataAnnotations;

namespace RR.Core.DTOs.Requests
{
    public class PeriodRequest
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }

    public class CreateReservationRequest
    {
        [Required]
        public int RoomId { get; set; }
        [Required]
        public int AccountId { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
    }

    public class ReservationUpdateRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "Id inválido.")]
        public int Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "RoomId inválido.")]
        public int RoomId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "AccountId inválido.")]
        public int AccountId { get; set; }

        [Required(ErrorMessage = "Informe a data/hora inicial.")]
        public DateTimeOffset StartTime { get; set; }

        [Required(ErrorMessage = "Informe a data/hora final.")]
        public DateTimeOffset EndTime { get; set; }

        public string? Note { get; set; }
    }
}
