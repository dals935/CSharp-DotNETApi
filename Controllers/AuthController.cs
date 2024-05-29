using System.Data;
using System.Security.Cryptography;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        private readonly AuthHelper _authHelper;
        public AuthController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            _authHelper = new AuthHelper(config);
        }

        [AllowAnonymous]
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

                    byte[] passwordHash = _authHelper.GetPasswordHash(userForRegistration.Password, passwordSalt);

                    string sqlAddAuth = @"EXEC TutorialAppSchema.spRegistration_Upsert 
                        @Email = @EmailParameter, 
                        @PasswordHash = @PasswordHashParameter, 
                        @PasswordSalt = @PasswordSaltParameter";

                    List<SqlParameter> sqlParameters = new List<SqlParameter>();

                    SqlParameter emailParameter = new SqlParameter("@EmailParameter", SqlDbType.VarChar);
                    emailParameter.Value = userForRegistration.Email;
                    sqlParameters.Add(emailParameter);

                    SqlParameter passwordSaltParameter = new SqlParameter("@PasswordSaltParameter", SqlDbType.VarBinary);
                    passwordSaltParameter.Value = passwordSalt;
                    sqlParameters.Add(passwordSaltParameter);

                    SqlParameter passwordHashParameter = new SqlParameter("@PasswordHashParameter", SqlDbType.VarBinary);
                    passwordHashParameter.Value = passwordHash;
                    sqlParameters.Add(passwordHashParameter);

                    if (_dapper.ExecuteSqlWithParameters(sqlAddAuth, sqlParameters))
                    {
                        
                        string sqlAddUser = @"EXEC TutorialAppSchema.spUser_Upsert
                                        @FirstName = '" + userForRegistration.FirstName +
                                    "', @LastName = '" + userForRegistration.LastName.Trim().Replace("'","''") +
                                    "', @Email = '" + userForRegistration.Email +
                                    "', @Gender = '" + userForRegistration.Gender +
                                    "', @Active = 1" + 
                                    ", @JobTitle = '" + userForRegistration.JobTitle + 
                                    "', @Department = '" + userForRegistration.Department + 
                                    "', @Salary = '" + userForRegistration.Salary + "'";

                        // string sqlAddUser = @"
                        // INSERT INTO TutorialAppSchema.Users(
                        //     [FirstName],
                        //     [LastName],
                        //     [Email],
                        //     [Gender],
                        //     [Active]
                        // ) VALUES ( '" + userForRegistration.FirstName +
                        //     "', '" + userForRegistration.LastName.Trim().Replace("'", "''") +
                        //     "', '" + userForRegistration.Email +
                        //     "', '" + userForRegistration.Gender +
                        //     "', 1)";

                        if (_dapper.ExecuteSql(sqlAddUser))
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

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login(UserForLogInDto userForLogIn)
        {
            string sqlForHashAndSalt = @"SELECT [Email],
                [PasswordHash],
                [PasswordSalt] FROM TutorialAppSchema.Auth WHERE Email = '" +
                userForLogIn.Email + "'";

            UserForLogInConfirmationDto? userForConformation = _dapper
                .LoadDataSingle<UserForLogInConfirmationDto?>(sqlForHashAndSalt);

            if (userForConformation == null)
            {
                return StatusCode(401, "Email Incorrect");
            }

            byte[] passwordHash = _authHelper.GetPasswordHash(userForLogIn.Password, userForConformation.PasswordSalt);

            //If Statement Won't work for password here
            for (int index = 0; index < passwordHash.Length; index++)
            {
                if (passwordHash[index] != userForConformation.PasswordHash[index])
                {
                    return StatusCode(401, "Password Incorrect");
                }
            }

            string userIdSql = @"SELECT [UserId] FROM TutorialAppSchema.Users WHERE Email = '" + userForLogIn.Email + "'";

            int userId = _dapper.LoadDataSingle<int>(userIdSql);

            return Ok(new Dictionary<string, string>{
                {"token", _authHelper.CreateToken(userId)}
            });
        }

        [HttpGet("RefreshToken")]
        public IActionResult RefreshToken()
        {

            string userId = User.FindFirst("userId")?.Value + "";
            string userIdSql = "SELECT UserId FROM TutorialAppSchema.Users WHERE UserId = " + userId;
            
            int userIdFromDB = _dapper.LoadDataSingle<int>(userIdSql);

            return Ok(new Dictionary<string, string>{
                {"token", _authHelper.CreateToken(userIdFromDB)}
            });

        }
    }
}