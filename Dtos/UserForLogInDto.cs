namespace DotnetAPI.Dtos
{
    partial class UserForLogInDto
    {
        string Email { get; set; }
        string Password { get; set; }

        public UserForLogInDto()
        {
            if (Email == null){Email = "";}
            if (Password == null){Password = "";}
        }
    }
}