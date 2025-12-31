using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using RR.Core.Enums;
using RR.Core.Common;
using RR.Infraestructure.DataContext;
using System.Security.Claims;
using ReservaRussasAPI.Attributes;

namespace ReservaRussasAPI.Handler
{
    public class MinPermissionHandler : AuthorizationHandler<MinPermissionRequirement>
    {
      private readonly ApplicationDbContext _db;

      public MinPermissionHandler(ApplicationDbContext db)
      {
        _db = db;
      }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, MinPermissionRequirement requirement)
        {
          var permStr = context.User.FindFirstValue("permission");
          if (!Enum.TryParse<EAccountPermission>(permStr, out var userPerm))
            userPerm = EAccountPermission.Student;

          var userLevel = AccountPermissionLevel.GetLevel(userPerm);
          var requiredLevel = AccountPermissionLevel.GetLevel(requirement.MinPermission);

          // 2) hierarquia normal
          if (userLevel >= requiredLevel)
          {
            context.Succeed(requirement);
            return;
          }

          // 3) regra especial:
          // Student + StudentAdvisor ativo = Servant 
        if (userPerm == EAccountPermission.Student &&
            requiredLevel <= AccountPermissionLevel.GetLevel(EAccountPermission.Servant))
        {
          var studentIdStr = context.User.FindFirstValue("studentId");
          if (int.TryParse(studentIdStr, out var studentId))
          {
            var hasAdvisor = await _db.StudentAdvisor
              .AnyAsync(x => x.StudentId == studentId && x.IsActive);

            if (hasAdvisor)
            {
              context.Succeed(requirement);
              return;
            }
          }
        }
      }
    }
}
