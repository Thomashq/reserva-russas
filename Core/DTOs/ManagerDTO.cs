namespace RR.Core.DTOs
{
    public class ManagerDTO : AccountDTO
    {
        public Guid Id { get; set; }

        public Guid AccountId { get; set; }

        public AccountDTO Account { get; set; }

        public List<Guid> ManagedRooms { get; set; }

        public List<RoomDTO> ManagedRoomsToList { get; set; }
    }
}
