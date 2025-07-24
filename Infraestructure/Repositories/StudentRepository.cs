using Microsoft.EntityFrameworkCore;
using RR.Core.Entities;
using RR.Core.Repositories;
using RR.Infraestructure.DataContext;

namespace RR.Infraestructure.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly ApplicationDbContext _context;

        public StudentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAsync(Student student)
        {
            student.IsActive = true;
            await _context.Student.AddAsync(student);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var student = await _context.Student.FindAsync(id);
            if (student == null) return false;

            student.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Student>> GetAllAsync()
        {
            return await _context.Student.Where(x => x.IsActive).ToListAsync();
        }

        public async Task<Student> GetStudentById(int id)
        {
            var student = await _context.Student.FirstOrDefaultAsync(x => x.Id == id && x.IsActive);
            return student;
        }

        public async Task<Student> GetStudentByAccountId(int id)
        {
            var entity = await _context.Student.FirstOrDefaultAsync(x => x.AccountId == id && x.IsActive);
            return entity;
        }

        public async Task<Student> UpdateAsync(Student student)
        {
            var existingStudent = await _context.Student.FindAsync(student.Id);
            if (existingStudent == null) return null;

            existingStudent.AccountId = student.AccountId;
            existingStudent.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingStudent;
        }
    }
}