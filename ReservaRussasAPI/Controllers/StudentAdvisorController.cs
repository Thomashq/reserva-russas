using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ReservaRussasAPI.Controllers.Base;
using RR.Core.Services;
using RR.Core.Entities;
using RR.Core.DTOs.Requests;

namespace ReservaRussasAPI.Controllers
{
    public class StudentAdvisorController : BaseControllerFYP
    {
      private readonly IStudentAdvisorService _studentAdvisorService;

      public StudentAdvisorController(IStudentAdvisorService studentAdvisorService)
      {
       _studentAdvisorService = studentAdvisorService;
      }

      [HttpGet]
      [Authorize(Policy = "ServantOrAbove")]
      public async Task<IActionResult> GetAll()
      {
        try
        {
          var list = await _studentAdvisorService.GetAll();

          return ResponseOk(list);
        }
        catch(Exception ex)
        {
          return ResponseBadRequest(ex.ToString());
        }
      }

      [HttpGet("student/{studentId:int}")]
      [Authorize(Policy = "ServantOrAbove")]
      public async Task<IActionResult> GetByStudentId(int studentId)
      {
        try
        {
          var list = await _studentAdvisorService.GetByStudentId(studentId);

          return ResponseOk(list);
        }
        catch(Exception ex)
        {
          return ResponseBadRequest(ex.ToString());
        }
      }

      [HttpGet("servant/{servantId:int}")]
      [Authorize(Policy = "ServantOrAbove")]
      public async Task<IActionResult> GetByServanttId(int servantId)
      {
        try
        {
          var list = await _studentAdvisorService.GetByServantId(servantId);

          return ResponseOk(list);
        }
        catch(Exception ex)
        {
          return ResponseBadRequest(ex.ToString());
        }
      }
      
      [HttpGet("{id:int}")]
      [Authorize(Policy = "ServantOrAbove")]
      public async Task<IActionResult> GetById(int id)
      {
        try
        {
          var item = await _studentAdvisorService.GetById(id);

          return ResponseOk(item);
        }
        catch(Exception ex)
        {
          return ResponseBadRequest(ex.ToString());
        }
      }

      [HttpPost]
      [Authorize(Policy = "ServantOrAbove")]
      public async Task<IActionResult> AddAsync([FromBody] CreateStudentAdvisorRequest studentAdvisor)
      {
        try
        {
          var created = await _studentAdvisorService.AddAsync( new StudentAdvisor{
            StudentId = studentAdvisor.StudentId,
            ServantId = studentAdvisor.ServantId
          }); 
          return ResponseOk(created);
        }
        catch(Exception ex)
        {
          return ResponseBadRequest(ex.ToString());
        }
      }

      [HttpPut]
      [Authorize(Policy = "ServantOrAbove")]
      public async Task<IActionResult> UpdateAsync([FromBody] StudentAdvisor studentAdvisor)
      {
        try
        {
          var updated = await _studentAdvisorService.UpdateAsync(studentAdvisor);

          return ResponseOk(updated);
        }
        catch(Exception ex)
        {
          return ResponseBadRequest(ex.ToString());
        }
      }

      [HttpPut("delete/{id:int}")]
      [Authorize(Policy = "ServantOrAbove")]
      public async Task<IActionResult> DeleteAsync (int id)
      {
        try
        {
          var item = await _studentAdvisorService.DeleteAsync(id);

          return ResponseOk(item);
        }
        catch(Exception ex)
        {
          return ResponseBadRequest(ex.ToString());
        }
      }
    }
}
