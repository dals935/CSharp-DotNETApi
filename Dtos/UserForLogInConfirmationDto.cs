namespace DotnetAPI.Dtos
{
    partial class UserForLogInConfirmationDto
    {
        byte[] PasswordHash { get; set; }
        byte[] PasswordSalt { get; set; }

        UserForLogInConfirmationDto()
        {
            if(PasswordHash == null){PasswordHash = new byte[0];}
            if(PasswordSalt == null){PasswordSalt = new byte[0];}
        }
    }
}