namespace RR.Core.DTOs.Requests
{
    public class CreateEquipmentReservationRequest
    {
        public int EquipmentId { get; set; }
        public int AccountId { get; set; }
        public int? RoomReservationId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }

    public class EquipmentReservationUpdateRequest
    {
        public int Id { get; set; }
        public int EquipmentId { get; set; }
        public int AccountId { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
    }

    public class EquipmentAvailabilityRequest
    {
        public int EquipmentId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
