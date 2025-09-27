//caso ocorra conflito esse escopo deve ser gerado e exibido no front end para o usuário ou manager
namespace RR.Core.DTOs
{
    public class ConflictDto
    {
        public string Scope { get; set; }              // "ROOM" | "OWNER" | "CLASS"
        public int? RoomId { get; set; }
        public int? AccountId { get; set; }            // professor/owner
        public string ClassCode { get; set; }          // se você usar turma/curso
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Message { get; set; }
    }
}
