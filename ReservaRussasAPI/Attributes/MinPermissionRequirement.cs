using RR.Core.Enums;
using Microsoft.AspNetCore.Authorization;
namespace ReservaRussasAPI.Attributes
{
    public class MinPermissionRequirement : IAuthorizationRequirement
    {
      public EAccountPermission MinPermission { get; }

      public MinPermissionRequirement(EAccountPermission minPermission)
      {
        MinPermission = minPermission;
      }
    }
}
