namespace DotnetAPI.Models
{
    public partial class UserResetPassword
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }

        public UserResetPassword()
        {
            if (Email == null){Email = "";}
            if (Password == null){Password = "";}
            if (PasswordConfirm == null){PasswordConfirm = "";}
        }
    }
}