using DotnetAPI.Models;

namespace DotnetAPI.Data
{
    public interface IUserRepository
    {
        public bool SaveChanges();

        public void AddEntity<T>(T entityToAdd);

        public void RemoveEntity<T>(T entityToAdd);

        public IEnumerable<User> GetUsersEF();

        public IEnumerable<UserSalary> GetUsersSalaryEF();

        public IEnumerable<UserJobInfo> GetUsersJobInfoEF();

        public User GetSingleUsersEF(int userId);

        public UserSalary GetSingleSalaryEF(int userId);

        public UserJobInfo GetSingleJobInfoEF(int userId);
    }
}