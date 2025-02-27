namespace RR.Core.DTOs
{
    public class ServantDTO:AccountDTO
    {
        public Guid Id { get; set; }

        public AccountDTO Account { get; set; }

        public List<StudentDTO> Advisee {get; set;}  

        public List<ReservationDTO> Reservation {get; set;} = new();
    }
}
