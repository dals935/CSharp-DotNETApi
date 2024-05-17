namespace DotnetAPI.Dtos
{
    public partial class UserForLogInDto
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public UserForLogInDto()
        {
            if (Email == null){Email = "";}
            if (Password == null){Password = "";}
        }
    }
}