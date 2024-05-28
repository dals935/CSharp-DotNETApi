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
    public IEnumerable<UserCompleteData> GetUsers(int userId = 0, bool isActive = true)
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

    //Delete All User Data
    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUsers(int userId)
    {
        string sql = @"EXEC TutorialAppSchema.spUser_Delete
            @UserId = " + userId.ToString();

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }
        throw new Exception("Failed To Delete User");
    }

}