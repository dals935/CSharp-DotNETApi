using System.Data;
using System.Runtime.CompilerServices;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserCompleteController : ControllerBase
{

    DataContextDapper _dapper;
    public UserCompleteController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
    }

    [HttpGet("TestConnection")]
    public DateTime TestConnection()
    {
        return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
    }
    
    //Return Data Of All Users or Single User
    [HttpGet("GetUsers/{userId}/{isActive}")]
    public IEnumerable<UserCompleteData> GetUsers(int userId = 0, bool isActive = true)
    {
        string sql = @"EXEC TutorialAppSchema.spUsers_Get";
        string stringParameters = "";
        DynamicParameters sqlParameters = new DynamicParameters();

        if (userId != 0)
        {
            stringParameters += ", @UserId = @UserIdParameter";
            sqlParameters.Add("@UserIdParameter", userId, DbType.Int32);
        }
        if (isActive)
        {
            stringParameters += ", @Active = @ActiveParameter";
            sqlParameters.Add("@ActiveParameter", isActive, DbType.Boolean);
        }
        else
        {
            stringParameters += ", @Active = @ActiveParameter";
            sqlParameters.Add("@ActiveParameter", isActive, DbType.Boolean);
        }

        sql += stringParameters.Substring(1);

        IEnumerable<UserCompleteData> users = _dapper.LoadDataWithParameters<UserCompleteData>(sql, sqlParameters);

        return users;
    }

    //Update Or Insert User Details
    [HttpPut("UpsertUser")]
    public IActionResult UpsertUser(UserCompleteData user)
    {
        string sql = @"EXEC TutorialAppSchema.spUser_Upsert
                @FirstName = @FirtNameParameter,
                @LastName = @LastNameParameter,
                @Email = @EmailParameter,
                @Gender = @GenderParameter,
                @Active = @ActiveParameter,
                @JobTitle = @JobTitileParameter,
                @Department = @DepartmetParameter,
                @Salary = @SalaryParameter,
                @UserId = @UserIdParameter";

        DynamicParameters sqlParameters = new DynamicParameters();

        sqlParameters.Add("@FirtNameParameter", user.FirstName, DbType.String);
        sqlParameters.Add("@LastNameParameter", user.LastName.Trim().Replace("'","''"), DbType.String);
        sqlParameters.Add("@EmailParameter", user.Email, DbType.String);
        sqlParameters.Add("@GenderParameter", user.Gender, DbType.String);
        sqlParameters.Add("@ActiveParameter", user.Active, DbType.Boolean);
        sqlParameters.Add("@JobTitileParameter", user.JobTitle, DbType.String);
        sqlParameters.Add("@DepartmetParameter", user.Department, DbType.String);
        sqlParameters.Add("@SalaryParameter", user.Salary, DbType.Decimal);
        sqlParameters.Add("@UserIdParameter", user.UserId, DbType.Int32);

        if (_dapper.ExecuteSqlWithParameters(sql, sqlParameters))
        {
            return Ok();
        }
        throw new Exception("Failed To Update User");
    }

    //Delete All User Data
    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUsers(int userId)
    {
        string sql = @"EXEC TutorialAppSchema.spUser_Delete
            @UserId = @UserIdParameter";

        DynamicParameters sqlParameters = new DynamicParameters();

        sqlParameters.Add("@UserIdParameter", userId, DbType.Int32);

        if (_dapper.ExecuteSqlWithParameters(sql, sqlParameters))
        {
            return Ok();
        }
        throw new Exception("Failed To Delete User");
    }

}