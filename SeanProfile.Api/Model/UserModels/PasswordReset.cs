namespace SeanProfile.Api.Model;

public class PasswordReset
{
    public int UserId { get; set; }
    public string PasswordResetKey { get; set; } = string.Empty;
    public long PasswordResetKeyTimeOut { get; set; }
    public bool PasswordResetUsed { get; set; }
    public DateTime InsertedDateTime { get; set; }
    public DateTime UsedDateTime { get; set; }

}

