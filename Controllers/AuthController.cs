using System.Data;
using System.Security.Cryptography;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Helpers;
using DotnetAPI.Models;
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
                    UserForLogInDto userForSetPassword = new UserForLogInDto(){
                        Email = userForRegistration.Email,
                        Password = userForRegistration.Password,
                    };

                    if (_authHelper.SetPassword(userForSetPassword))
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

        [HttpPut("ResetPassword")]
        public IActionResult ResetPassword(UserResetPassword userResetPassword)
        {
            UserForLogInDto userForSetPassword = new UserForLogInDto(){
                        Email = userResetPassword.Email,
                        Password = userResetPassword.Password,
                    };
            if (userForSetPassword.Password != userResetPassword.PasswordConfirm)
            {
                throw new Exception("Passwords Do Not Match");
            }
            if (_authHelper.SetPassword(userForSetPassword))
            {
                return Ok();
            }
            throw new Exception("Password Failed to Update");
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