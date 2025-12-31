namespace RR.Core.DTOs.Requests
{
  ///<summary>
  /// DTO para criar relação de orientação
  /// <summary>
  public class CreateStudentAdvisorRequest
  {
     public int StudentId {get; set;}
     public int ServantId {get; set;}
  }
}
