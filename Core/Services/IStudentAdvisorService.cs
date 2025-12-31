using RR.Core.Entities;

namespace RR.Core.Services
{
    public interface IStudentAdvisorService
    {
      Task<IEnumerable<StudentAdvisor>> GetAll();
      Task<bool> AddAsync(StudentAdvisor studentAdvisor);
      Task<StudentAdvisor> GetById(int id);
      Task<StudentAdvisor> UpdateAsync(StudentAdvisor studentAdvisor);
      Task<bool>DeleteAsync(int id);
      Task<IEnumerable<StudentAdvisor>> GetByStudentId(int id);
      Task<IEnumerable<StudentAdvisor>> GetByServantId(int id);
    }
}
