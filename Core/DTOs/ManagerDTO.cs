namespace RR.Core.DTOs
{
    public class ManagerDTO : AccountDTO
    {
        public Guid Id { get; set; }

        public AccountDTO Account { get; set; }

        public List<RoomDTO> ManagedRooms { get; set; }
    }
}
