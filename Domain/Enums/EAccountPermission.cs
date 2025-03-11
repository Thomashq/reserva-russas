using System.ComponentModel;

namespace Domain.Enums;

public enum EAccountPermission
{
    [Description("Manager")]
    Manager,
    [Description("Servant")]
    Servant,
    [Description("Student")]
    Student
}
