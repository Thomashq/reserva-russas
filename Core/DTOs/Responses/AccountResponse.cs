namespace RR.Core.Responses.Account
{
    /// <summary>
    /// Response básico de conta
    /// </summary>
    public class AccountResponse
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Mail { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public int AccountPermission { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Response resumido de conta (para listagens)
    /// </summary>
    public class AccountSummaryResponse
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Mail { get; set; } = string.Empty;
        public int AccountPermission { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Response de conta criada
    /// </summary>
    public class AccountCreatedResponse
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Mail { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public int AccountPermission { get; set; }
        public DateTime CreatedAt { get; set; }
    }


}
