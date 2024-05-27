using System.Runtime.CompilerServices;
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
    public IEnumerable<UserCompleteData> GetUsers(int userId, bool isActive)
    {
        string sql = @"EXEC TutorialAppSchema.spUsers_Get";
        string parameters = "";

        if (userId != 0)
        {
            parameters += ", @UserId = " + userId.ToString();
        }
        if (isActive)
        {
            parameters += ", @Active = " + isActive.ToString();
        }
        else
        {
            parameters += ", @Active = " + isActive.ToString();
        }

        sql += parameters.Substring(1);

        IEnumerable<UserCompleteData> users = _dapper.LoadData<UserCompleteData>(sql);

        return users;
    }

    //Update Or Insert User Details
    [HttpPut("UpsertUser")]
    public IActionResult UpsertUser(UserCompleteData user)
    {
        string sql = @"EXEC TutorialAppSchema.Users.spUser_Upsert
                @FirstName = '" + user.FirstName +
            "', @LastName = '" + user.LastName.Trim().Replace("'","''") +
            "', @Email = '" + user.Email +
            "', @Gender = '" + user.Gender +
            "', @Active = '" + user.Active + 
            "', @JobTitle = '" + user.JobTitle + 
            "', @Department = '" + user.Department + 
            "', @Salary = '" + user.Salary + 
            "',  @UserId = " + user.UserId.ToString();

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }
        throw new Exception("Failed To Update User");
    }

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUsers(int userId)
    {
        string sql = @"
            DELETE FROM 
                TutorialAppSchema.Users
            WHERE UserId = " + userId.ToString();

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }
        throw new Exception("Failed To Delete User");
    }

    [HttpGet("GetUsersSalary")]
    // public IActionResult Test()
    public IEnumerable<UserSalary> GetUsersSalary()
    {
        string sql = @"
            SELECT  [UserId]
                , [Salary]
            FROM  TutorialAppSchema.UserSalary;";

        IEnumerable<UserSalary> usersSalary = _dapper.LoadData<UserSalary>(sql);

        return usersSalary;
    }

    [HttpGet("GetUserSalary/{userId}")]
    public UserSalary GetUserSalary(int userId)
    {
        string sql = @"
            SELECT
            [Userid],
            [Salary] 
            FROM 
            TutorialAppSchema.UserSalary 
            WHERE 
            UserId = " + userId.ToString();

        UserSalary? userSalary = _dapper.LoadDataSingle<UserSalary>(sql);

        if(userSalary != default)
        {
            return userSalary;
        }
        throw new Exception("User Salary Not Found");
    }

    [HttpDelete("DeleteUserSalary/{userId}")]
    public IActionResult DeleteUsersSalary(int userId)
    {
        string sql = @"
            DELETE FROM 
                TutorialAppSchema.UserSalary
            WHERE UserId = " + userId.ToString();

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }
        throw new Exception("Failed To Delete User Salary");
    }

    [HttpGet("GetUsersJobInfo")]
    public IEnumerable<UserJobInfo> GetUsersJobInfo()
    {
        string sql = @"
            SELECT 
            [UserId],
            [JobTitle],
            [Department] 
            FROM TutorialAppSchema.UserJobInfo";

        IEnumerable<UserJobInfo> usersJobInfo = _dapper.LoadData<UserJobInfo>(sql);

        return usersJobInfo;
    }

    [HttpGet("GetUserJobInfo/{userId}")]
    public UserJobInfo GetUserJobInfo(int userId)
    {
        string sql = @"
            SELECT 
            [UserId],
            [JobTitle],
            [Department] 
            FROM TutorialAppSchema.UserJobInfo
            WHERE 
            UserId = " + userId.ToString();

        UserJobInfo? userJobInfo = _dapper.LoadDataSingle<UserJobInfo>(sql);

        if(userJobInfo != default)
        {
            return userJobInfo;
        }
        throw new Exception("User Job Info Not Found");
    }

    [HttpDelete("DeleteUserJobInfo/{userId}")]
    public IActionResult DeleteUserJobInfo(int userId)
    {
        string sql = @"
            DELETE FROM 
                TutorialAppSchema.UserJobInfo
            WHERE UserId = " + userId.ToString();

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }
        throw new Exception("Failed To Delete User Job Info");
    }

}