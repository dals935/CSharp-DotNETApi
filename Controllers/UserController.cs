using System.Runtime.CompilerServices;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{

    DataContextDapper _dapper;
    public UserController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
    }

    [HttpGet("TestConnection")]
    public DateTime TestConnection()
    {
        return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
    }
        
    [HttpGet("GetUsers")]
    // public IActionResult Test()
    public IEnumerable<User> GetUsers()
    {
        string sql = @"
            SELECT  [UserId]
                , [FirstName]
                , [LastName]
                , [Email]
                , [Gender]
                , [Active]
            FROM  TutorialAppSchema.Users;";

        IEnumerable<User> users = _dapper.LoadData<User>(sql);

        return users;
    }

    [HttpGet("GetSingleUser/{userId}")]
    public User GetSingleUsers(int userId)
    {
        string sql = @"
            SELECT  [UserId]
                , [FirstName]
                , [LastName]
                , [Email]
                , [Gender]
                , [Active]
            FROM  TutorialAppSchema.Users
                WHERE UserId = " + userId.ToString();

        User? user = _dapper.LoadDataSingle<User>(sql);

        if(user != default)
        {
            return user;
        }
        throw new Exception("User Not Found");
    
    }

    [HttpPut("EditUser")]
    public IActionResult EditUser(User user)
    {
        string sql = @"
            UPDATE TutorialAppSchema.Users
                SET [FirstName] = '" + user.FirstName +
                "', [LastName] = '" + user.LastName.Trim().Replace("'","''") +
                "', [Email] = '" + user.Email +
                "', [Gender] = '" + user.Gender +
                "', [Active] = '" + user.Active + 
                "' WHERE UserId = " + user.UserId.ToString();

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }
        throw new Exception("Failed To Update User");
    }

    [HttpPost("AddUser")]
    public IActionResult AddUser(UserToAddDto user)
    {
        string sql = @"
            INSERT INTO TutorialAppSchema.Users(
                [FirstName],
                [LastName],
                [Email],
                [Gender],
                [Active]
            ) VALUES ( '"+ user.FirstName +
                "', '" + user.LastName.Trim().Replace("'","''") +
                "', '" + user.Email +
                "', '" + user.Gender +
                "', '" + user.Active + 
            "')";
        
        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }
        throw new Exception("Failed To Add User");
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
    public UserSalary GetUsers(int userId)
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

    [HttpPost("AddUserSalary")]
    public IActionResult AddUserSalary(UserSalary userSalary)
    {
        string sql = @"
            INSERT INTO TutorialAppSchema.UserSalary(
                [UserId],
                [Salary]
            ) VALUES ( '"+ userSalary.UserId.ToString() +"', '"+ userSalary.Salary + "')";
        
        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }
        throw new Exception("Failed To Add User Salary");
    }

    [HttpPut("EditUserSalary")]
    public IActionResult EditUserSalary(UserSalary userSalary)
    {
        string sql = @"
            UPDATE TutorialAppSchema.UserSalary
                SET [Salary] = '"+ userSalary.Salary +
                "' WHERE UserId = " + userSalary.UserId.ToString();

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }
        throw new Exception("Failed To Update User Salary");
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

    [HttpPost("AddUserJobInfo")]
    public IActionResult AddUserJobInfo(UserJobInfo userJobInfo)
    {
        string sql = @"
            INSERT INTO TutorialAppSchema.UserJobInfo(
                [UserId],
                [JobTitle],
                [Department]
            ) VALUES ( '"+ userJobInfo.UserId.ToString() +
                   "', '"+ userJobInfo.JobTitle +
                   "', '"+ userJobInfo.Department +"')";
        
        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }
        throw new Exception("Failed To Add User Job Info");
    }

    [HttpPut("EditUserJobInfo")]
    public IActionResult EditUserJobInfo(UserJobInfo userJobInfo)
    {
        string sql = @"
            UPDATE TutorialAppSchema.UserJobInfo
                SET [JobTitle] = '"+ userJobInfo.JobTitle +
                "', [Department] = '"+ userJobInfo.Department + 
                "' WHERE UserId = " +userJobInfo.UserId.ToString();

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }
        throw new Exception("Failed To Update User Job Info");
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