using Core.Repositories;
using Domain.Models;
using RR.Core.DTOs;
using RR.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RR.Service.Service
{
    public class StudentService : IStudentService
    {
        private readonly IRepository<Student> _repository;

        public StudentService(IRepository<Student> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<StudentDTO>> GetAllAsync()
        {
            var students = await _repository.GetAllAsync();
            return students.Select(s => new StudentDTO { Id = s.Id, AccountId = s.AccountId, Reservations = s.Reservations, Advisor = s.Advisor, Permissions = s.Permissions });
        }

        public async Task<StudentDTO?> GetByIdAsync(Guid id)
        {
            var student = await _repository.GetByIdAsync(id);
            return student != null ? new StudentDTO { Id = student.Id, AccountId = student.AccountId, Reservations = student.Reservations, Advisor = student.Advisor, Permissions = student.Permissions } : null;
        }

        public async Task<StudentDTO> CreateAsync(StudentDTO dto)
        {
            var student = new Student { Id = dto.Id, AccountId = dto.AccountId, Reservations = dto.Reservations, Advisor = dto.Advisor, Permissions = dto.Permissions };
            await _repository.AddAsync(student);
            return dto;
        }

        public async Task<StudentDTO?> UpdateAsync(Guid id, StudentDTO dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return null;

            existing.Reservations = dto.Reservations;
            existing.Advisor = dto.Advisor;
            existing.Permissions = dto.Permissions;
            await _repository.UpdateAsync(existing);
            return dto;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return false;
            await _repository.DeleteAsync(id);
            return true;
        }
    }
}
