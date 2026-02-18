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
      
      public async Task<IEnumerable<StudentAdvisor>> GetAll()
      {
        return await _dbContext.StudentAdvisor.Where(x => x.IsActive).ToListAsync();
      }

      public async Task<bool> AddAsync (StudentAdvisor studentAdvisor)
      {
        //verificar se ja existe uma relação ativa com os dois
        var activerel = await _dbContext.StudentAdvisor.FirstOrDefaultAsync(
              a => a.StudentId  == studentAdvisor.StudentId && a.ServantId == studentAdvisor.ServantId
              && a.IsActive == true);

        if(activerel != null)
          return false;
        
        studentAdvisor.IsActive = true;
        await _dbContext.StudentAdvisor.AddAsync(studentAdvisor);
        await _dbContext.SaveChangesAsync();

        return true;
      }
      
      public async Task<StudentAdvisor> GetById(int id)
      {
        return await _dbContext.StudentAdvisor.FirstOrDefaultAsync(a=>a.Id == id);
      }

      public async Task<StudentAdvisor> UpdateAsync(StudentAdvisor studentAdvisor)
      {
        var relationToUpdate = await _dbContext.StudentAdvisor.FirstOrDefaultAsync(a=>a.Id == studentAdvisor.Id && a.IsActive == true);

        if(relationToUpdate == null)
          return null;

        relationToUpdate.ServantId = studentAdvisor.ServantId;
        relationToUpdate.StudentId = studentAdvisor.StudentId;
        relationToUpdate.UpdatedAt = DateTime.UtcNow;
        relationToUpdate.IsActive = studentAdvisor.IsActive;
        
        await _dbContext.SaveChangesAsync();
        return relationToUpdate;
      }

      public async Task<bool> DeleteAsync(int id)
      {
        var deleted = await _dbContext.StudentAdvisor.FindAsync(id);

        if(deleted == null) return false;

        deleted.IsActive = false;

        await _dbContext.SaveChangesAsync();

        return true;
      }
      
      public async Task<IEnumerable<StudentAdvisor>> GetByStudentId(int id)
      {
        var relationList = await _dbContext.StudentAdvisor.Where(a=>a.StudentId == id && a.IsActive == true).ToListAsync();

        return relationList;
      }

      public async Task<IEnumerable<StudentAdvisor>> GetByServantId(int id)
      {
        var relationList = await _dbContext.StudentAdvisor.Where(a=>a.ServantId== id && a.IsActive == true).ToListAsync();

        return relationList;
      }
    }
}
