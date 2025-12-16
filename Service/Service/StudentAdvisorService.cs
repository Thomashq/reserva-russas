using RR.Core.Entities;
using RR.Core.Services;
using RR.Infraestructure.DataContext;
using Microsoft.EntityFrameworkCore;

namespace RR.Service.Service
{
    public class StudentAdvisorService : IStudentAdvisorService
    {
      private readonly ApplicationDbContext _dbContext;

      public StudentAdvisorService(ApplicationDbContext dbContext)
      {
        _dbContext = dbContext;
      }
      
      public async Task<IEnumerable<StudentAdvisor>> GetallAsync()
      {
        return await _dbContext.StudentAdvisor.Where(x => x.IsActive).ToListAsync();
      }

    }
}
