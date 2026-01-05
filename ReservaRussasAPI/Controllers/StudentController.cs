using Microsoft.AspNetCore.Mvc;
using ReservaRussasAPI.Controllers.Base;
using RR.Core.Entities;
using RR.Core.Services;

namespace ReservaRussasAPI.Controllers
{
    public class StudentController : BaseControllerFYP
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
              var student = await _studentService.GetAllAsync();
              return ResponseOk(student);
            }
            catch (Exception ex)
            {
                return ResponseInternalServerError(ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] Student student)
        {
            try
            {
                if (student == null)
                    return ResponseBadRequest("Student data is required");

                var ok = await _studentService.AddAsync(student);
                return ResponseOk(ok);
            }
            catch (Exception ex)
            {
                return ResponseInternalServerError(ex);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudentById(int id)
        {
            try
            {
                if (id <= 0)
                    return ResponseBadRequest("ID must be a positive number");

                var student = await _studentService.GetStudentById(id);
                if (student == null)
                    return ResponseNotFound("Student not found");

                return ResponseOk(student);
            }
            catch (Exception ex)
            {
                return ResponseInternalServerError(ex);
            }
        }

        [HttpGet("account/{id}")]
        public async Task<IActionResult> GetStudentByAccountId(int id)
        {
            try
            {
                if (id <= 0)
                    return ResponseBadRequest("ID must be a positive number");

                var student = await _studentService.GetStudentByAccountId(id);
                if (student == null)
                    return ResponseNotFound("Student not found");

                return ResponseOk(student);
            }
            catch (Exception ex)
            {
                return ResponseInternalServerError(ex);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] Student student)
        {
            try
            {
                if (id <= 0)
                    return ResponseBadRequest("ID must be a positive number");

                if (student == null)
                    return ResponseBadRequest("Student data is required");

                student.Id = id;

                var updated = await _studentService.UpdateAsync(student);
                if (updated == null)
                    return ResponseNotFound("Student not found");

                return ResponseOk(updated);
            }
            catch (Exception ex)
            {
                return ResponseInternalServerError(ex);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return ResponseBadRequest("ID must be a positive number");

                var deleted = await _studentService.DeleteAsync(id);
                if (!deleted)
                    return ResponseNotFound("Student not found or already inactive");

                return ResponseOk(deleted);
            }
            catch (Exception ex)
            {
                return ResponseInternalServerError(ex);
            }
        }
    }
}

