namespace DotnetAPI.Dtos
{
    public partial class UserForLogInConfirmationDto
    {
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        UserForLogInConfirmationDto()
        {
            if(Email == null){Email = "";}
            if(PasswordHash == null){PasswordHash = new byte[0];}
            if(PasswordSalt == null){PasswordSalt = new byte[0];}
        }
    }
}