using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RR.Core.DTOs.Requests
{
    public class PeriodRequest
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            if (Start >= End)
            {
                yield return new ValidationResult("Ínicio deve ser anterior ao fim", new[] { nameof(Start), nameof(End) });
            }
        }
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
        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            if (RoomId <= 0)
            {
                yield return new ValidationResult("RoomId must be a positive number", new[] { nameof(RoomId) });
            }
            if (AccountId <= 0)
            {
                yield return new ValidationResult("AccountId must be a positive number", new[] { nameof(AccountId) });
            }
            if (StartTime >= EndTime)
            {
                yield return new ValidationResult("StartTime must be before EndTime", new[] { nameof(StartTime), nameof(EndTime) });
            }
        }
    }

    public class ReservationUpdateRequest : IValidatableObject
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

        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
                if (StartTime == default)
                    yield return new ValidationResult(
                        "Data/hora inicial inválida.",
                        new[] { nameof(StartTime) });

                if (EndTime == default)
                    yield return new ValidationResult(
                        "Data/hora final inválida.",
                        new[] { nameof(EndTime) });

                if (StartTime >= EndTime)
                    yield return new ValidationResult(
                        "A data/hora inicial deve ser anterior à final.",
                        new[] { nameof(StartTime), nameof(EndTime) });

                var duration = EndTime - StartTime;

                if (duration < TimeSpan.FromMinutes(30))
                    yield return new ValidationResult(
                        "A duração mínima da reserva é de 30 minutos.",
                        new[] { nameof(StartTime), nameof(EndTime) });

                if (duration > TimeSpan.FromHours(4))
                    yield return new ValidationResult(
                        "A duração máxima da reserva é de 4 horas.",
                        new[] { nameof(StartTime), nameof(EndTime) });

                // Ajuste esta regra conforme sua política para updates:
                if (StartTime < DateTimeOffset.UtcNow.AddMinutes(-5))
                    yield return new ValidationResult(
                        "Não é permitido atualizar para uma data no passado.",
                        new[] { nameof(StartTime) });
            }
        }
}
