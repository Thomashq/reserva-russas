using RR.Core.Enums;

namespace RR.Core.Common
{
    public class AccountPermissionLevel
    {
      public static int GetLevel(EAccountPermission p)
      {
      // Student < Servant < Manager < Admin
        return p switch
        {
          EAccountPermission.Student => 0,
          EAccountPermission.Servant => 1,
          EAccountPermission.Manager => 2,
          EAccountPermission.Admin => 3,
          _ => 0
        };
      }
    }
}
