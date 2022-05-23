namespace SeanProfile.Api.Model
{
    public class UserModel
    {
        public string Username { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public int Id { get; set; }

    }
}
