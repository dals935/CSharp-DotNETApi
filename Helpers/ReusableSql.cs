using System.Data;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Models;

namespace DotnetAPI.Helpers
{
    public class ReusableSql
    {
        private readonly DataContextDapper _dapper;
        public ReusableSql(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        public bool UpsertUser(UserCompleteData user)
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

            return _dapper.ExecuteSqlWithParameters(sql, sqlParameters);
        }
    }
}