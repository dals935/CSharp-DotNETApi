using System.Runtime.CompilerServices;
using AutoMapper;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserEFController : ControllerBase
{

    IUserRepository _userRepository;
    IMapper _mapper;
    public UserEFController(IConfiguration config, IUserRepository userRepository)
    {

        _userRepository = userRepository;

        _mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<UserToAddDto, User>();
            cfg.CreateMap<UserSalary, UserSalary>();
            cfg.CreateMap<UserJobInfo, UserJobInfo>();
        }));
        
    }


    [HttpGet("GetUsers")]
    public IEnumerable<User> GetUsersEF()
    {

        IEnumerable<User> users = _userRepository.GetUsersEF();

        return users;
    }

    [HttpGet("GetSingleUser/{userId}")]
    public User GetSingleUsersEF(int userId)
    {

        return _userRepository.GetSingleUsersEF(userId);

    }

    [HttpPut("EditUser")]
    public IActionResult EditUserEF(User user)
    {

        User? userDb = _userRepository.GetSingleUsersEF(user.UserId);

        if (userDb != null)
        {
            userDb.Active = user.Active;
            userDb.FirstName = user.FirstName;
            userDb.LastName = user.LastName;
            userDb.Email = user.Email;
            userDb.Gender = user.Gender;
            if (_userRepository.SaveChanges())
            {
                return Ok();
            }
            throw new Exception("Failed To Update User");
        }
        throw new Exception("User Not Found");

    }

    [HttpPost("AddUser")]
    public IActionResult AddUserEF(UserToAddDto user)
    {

        User userDb = _mapper.Map<User>(user);

        userDb.Active = user.Active;
        userDb.FirstName = user.FirstName;
        userDb.LastName = user.LastName;
        userDb.Email = user.Email;
        userDb.Gender = user.Gender;

        _userRepository.AddEntity<User>(userDb);
        if (_userRepository.SaveChanges())
        {
            return Ok();
        }

        throw new Exception("User Not Added");
    }

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUserEF(int userId)
    {
        User? userDb = _userRepository.GetSingleUsersEF(userId);
        if (userDb != null)
        {
            _userRepository.RemoveEntity<User>(userDb);
            if (_userRepository.SaveChanges())
            {
                return Ok();
            }
            throw new Exception("Failed To Delete User");
        }
        throw new Exception("User Not Found");
    }

    [HttpGet("GetUsersSalary")]
    public IEnumerable<UserSalary> GetUsersSalaryEF()
    {

        return _userRepository.GetUsersSalaryEF();

    }

    [HttpGet("UserSalary/{userId}")]
    public UserSalary GetUserSalaryEF(int userId)
    {
        return _userRepository.GetSingleSalaryEF(userId);
    }

    [HttpPost("UserSalary")]
    public IActionResult PostUserSalaryEF(UserSalary userForInsert)
    {
        _userRepository.AddEntity<UserSalary>(userForInsert);
        if (_userRepository.SaveChanges())
        {
            return Ok();
        }
        throw new Exception("Failed to Add User");
    }

    [HttpPut("EditUserSalary")]
    public IActionResult EditUserSalaryEF(UserSalary userForUpdate)
    {

        UserSalary? userToUpdate = _userRepository.GetSingleSalaryEF(userForUpdate.UserId);

        if (userForUpdate != null)
        {
            _mapper.Map(userForUpdate, userToUpdate);
            if (_userRepository.SaveChanges())
            {
                return Ok();
            }
            throw new Exception("Failed To Update User Salary");
        }
        throw new Exception("User Not Found");

    }

    [HttpDelete("DeleteUserSalary/{userId}")]
    public IActionResult DeleteUserSalaryEF(int userId)
    {
        UserSalary? userDb = _userRepository.GetSingleSalaryEF(userId);

        if (userDb != null)
        {
            _userRepository.RemoveEntity<UserSalary>(userDb);
            if (_userRepository.SaveChanges())
            {
                return Ok();
            }
            throw new Exception("Failed To Delete User");
        }
        throw new Exception("User Not Found");
    }

    [HttpGet("GetUsersJobInfo")]
    public IEnumerable<UserJobInfo> GetUsersJobInfoEF()
    {

        return _userRepository.GetUsersJobInfoEF();

    }

    [HttpGet("GetUserJobInfo/{userId}")]
    public UserJobInfo GetUserJobInfoEF(int userId)
    {
        return _userRepository.GetSingleJobInfoEF(userId);
    }

    [HttpPost("UserJobInfo")]
    public IActionResult UserJobInfoEF(UserJobInfo userForInsert)
    {
        _userRepository.AddEntity<UserJobInfo>(userForInsert);
        if (_userRepository.SaveChanges())
        {
            return Ok();
        }
        throw new Exception("Failed to Add User Job Info");
    }

    [HttpPut("EditUserJobInfo")]
    public IActionResult EditUserJobInfoEF(UserJobInfo userForUpdate)
    {

        UserJobInfo? userToUpdate = _userRepository.GetSingleJobInfoEF(userForUpdate.UserId);

        if (userToUpdate != null)
        {
            _mapper.Map(userForUpdate, userToUpdate);
            if (_userRepository.SaveChanges())
            {
                return Ok();
            }
            throw new Exception("Failed To Update User Job Info");
        }
        throw new Exception("User Not Found");

    }

    [HttpDelete("DeleteUserJobInfo/{userId}")]
    public IActionResult DeleteUserJobInfoEF(int userId)
    {
        UserJobInfo? userDb = _userRepository.GetSingleJobInfoEF(userId);

        if (userDb != null)
        {
            _userRepository.RemoveEntity<UserJobInfo>(userDb);
            if (_userRepository.SaveChanges())
            {
                return Ok();
            }
            throw new Exception("Failed To Delete User");
        }
        throw new Exception("User Not Found");
    }

}