namespace SeanProfile.Api.Model
{
    public class UserChangePassword
    {
        public int Id { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
