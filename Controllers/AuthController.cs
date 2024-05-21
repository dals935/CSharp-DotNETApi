using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAPI.Controllers
{
    public class AuthController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        private readonly IConfiguration _config;
        public AuthController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            _config = config;
        }

        [HttpPost("Register")]
        public IActionResult Register(UserForRegistrationDto userForRegistration)
        {
            if (userForRegistration.Password == userForRegistration.PasswordConfirm)
            {
                string sqlCheckUserEmailExists = "SELECT Email FROM TutorialAppSchema.Auth WHERE Email = '" +
                    userForRegistration + "'";

                IEnumerable<string> existingUsers = _dapper.LoadData<string>(sqlCheckUserEmailExists);
                if (existingUsers.Count() == 0)
                {
                    byte[] passwordSalt = new byte[128 / 8];
                    using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                    {
                        rng.GetNonZeroBytes(passwordSalt);
                    }

                    byte[] passwordHash = GetPasswordHash(userForRegistration.Password, passwordSalt);

                    string sqlAddAuth = @"
                    INSERT INTO TutorialAppSchema.Auth ([Email], 
                    [PasswordHash], 
                    [PasswordSalt]) VALUES('" + userForRegistration.Email +
                    "', @PasswordHash, @PasswordSalt)";

                    List<SqlParameter> sqlParameters = new List<SqlParameter>();

                    SqlParameter passwordSaltParameter = new SqlParameter("@PasswordSalt", SqlDbType.VarBinary);
                    passwordSaltParameter.Value = passwordSalt;
                    SqlParameter passwordHashParameter = new SqlParameter("@PasswordHash", SqlDbType.VarBinary);
                    passwordHashParameter.Value = passwordHash;

                    sqlParameters.Add(passwordSaltParameter);
                    sqlParameters.Add(passwordHashParameter);

                    if (_dapper.ExecuteSqlWithParameters(sqlAddAuth, sqlParameters))
                    {

                        string sqlAddUser = @"
                        INSERT INTO TutorialAppSchema.Users(
                            [FirstName],
                            [LastName],
                            [Email],
                            [Gender],
                            [Active]
                        ) VALUES ( '" + userForRegistration.FirstName +
                            "', '" + userForRegistration.LastName.Trim().Replace("'", "''") +
                            "', '" + userForRegistration.Email +
                            "', '" + userForRegistration.Gender +
                            "', 1)";

                        if(_dapper.ExecuteSql(sqlAddUser))
                        {

                            return Ok();
                            
                        }
                        throw new Exception("Failed To Add User");
                    }
                    throw new Exception("Failed To Register User");
                }
                throw new Exception("User With Email: " + userForRegistration.Email + " Already Exists.");
            }
            throw new Exception("Passwords Do Not Match");
        }

        [HttpPost("Login")]
        public IActionResult Login(UserForLogInDto userForLogIn)
        {
            string sqlForHashAndSalt = @"SELECT [Email],
                [PasswordHash],
                [PasswordSalt] FROM TutorialAppSchema.Auth WHERE Email = '" +
                userForLogIn.Email + "'";

            UserForLogInConfirmationDto? userForConformation = _dapper
                .LoadDataSingle<UserForLogInConfirmationDto?>(sqlForHashAndSalt);

            if(userForConformation == null)
            {
                return StatusCode(401, "Email Incorrect");
            }

            byte[] passwordHash = GetPasswordHash(userForLogIn.Password, userForConformation.PasswordSalt);

            //If Statement Won't work for password here
            for (int index = 0; index < passwordHash.Length; index++)
            {
                if (passwordHash[index] != userForConformation.PasswordHash[index])
                {
                    return StatusCode(401, "Password Incorrect");
                }
            }

            string userIdSql = @"SELECT [UserId] FROM TutorialAppSchema.Users WHERE Email = '"+ userForLogIn.Email +"'";

            int userId = _dapper.LoadDataSingle<int>(userIdSql);

            return Ok(new Dictionary<string, string>{
                {"token", CreateToken(userId)}
            });
        }

        private byte[] GetPasswordHash(string password, byte[] passwordSalt)
        {
            string passwordSaltPlusString = _config.GetSection("AppSettings:PasswordKey").Value
                     + Convert.ToBase64String(passwordSalt);

            return KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8
            );
        }

        private string CreateToken(int userId)
        {
            Claim[] claims= new Claim[] {
                new Claim("userId", userId.ToString())
            };

            string? tokenKeyString = _config.GetSection("AppSettings:TokenKey").Value;
 
            SymmetricSecurityKey tokenKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                tokenKeyString != null ? tokenKeyString : ""
                )
            );

            SigningCredentials credentials = new SigningCredentials(
                tokenKey, 
                SecurityAlgorithms.HmacSha512Signature );

            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor()
                {
                    Subject = new ClaimsIdentity(claims),
                    SigningCredentials = credentials,
                    Expires = DateTime.Now.AddDays(1)
                };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            SecurityToken token = tokenHandler.CreateToken(descriptor);

            return tokenHandler.WriteToken(token);

        }

    }
}