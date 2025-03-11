namespace RR.Core.DTOs
{
    public class ServantDTO:AccountDTO
    {
        public Guid Id { get; set; }

        public Guid AccountId { get; set; }

        public AccountDTO? Account { get; set; }

        public List<Guid>? Advisee { get; set; }

        public List<StudentDTO>? AdviseeList {get; set;}

        public List<Guid>? Reservation { get; set; }

        public List<ReservationDTO>? ReservationList {get; set;} = new();
    }
}
