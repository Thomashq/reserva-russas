using System.ComponentModel.DataAnnotations;

namespace RR.Core.DTOs.Requests
{
    public class CreateSeriesRequest
    {
        [Required] public int AccountId { get; set; }
        public int DefaultRoomId { get; set; }

        [Required, MaxLength(200)] public string Title { get; set; } = default!;
        [MaxLength(1000)] public string? Description { get; set; }

        [Required] public DateTime WindowStart { get; set; }
        [Required] public DateTime WindowEnd { get; set; }

        [MaxLength(500)] public string? RecurrenceRule { get; set; }

        //Permite 1..7 dias (antes exigia exatamente 2)
        [Required, RegularExpression(@"^(MO|TU|WE|TH|FR|SA|SU)(,(MO|TU|WE|TH|FR|SA|SU))*$",
            ErrorMessage = "Informe 1 ou mais dias (ex.: MO,WE ou MO,TU,WE).")]
        public string DaysOfWeek { get; set; } = default!;

        [Required] public TimeSpan TimeStart { get; set; }
        [Required] public TimeSpan TimeEnd { get; set; }

        public List<DateTime> Blackouts { get; set; } = new();

        public int Interval { get; set; } = 1;
    }

    public class PreviewSeriesRequest : CreateSeriesRequest { }

    public class EditSeriesRequest
    {
        [Required] public int SeriesId { get; set; }
        public int? RoomId { get; set; }
        [MaxLength(200)] public string? Title { get; set; }
        [MaxLength(1000)] public string? Description { get; set; }

        public DateTime? WindowStart { get; set; }
        public DateTime? WindowEnd { get; set; }

        [MaxLength(500)] public string? RecurrenceRule { get; set; }// MONTHLY, YEARLY
        public string? DaysOfWeek { get; set; }  // "MO,WE"
        public TimeSpan? TimeStart { get; set; }
        public TimeSpan? TimeEnd { get; set; }
        public int Interval { get; set; } = 1;
        public DateTime? ApplyFrom { get; set; }
    }
}
