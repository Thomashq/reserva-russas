using System.ComponentModel;

namespace RR.Core.Enums
{
    public enum EAccountPermission
    {
        [Description("Manager")]
        Manager,
        [Description("Servant")]
        Servant,
        [Description("Student")]
        Student
    }
}
