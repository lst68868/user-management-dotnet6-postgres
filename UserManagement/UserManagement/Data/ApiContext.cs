using System.Collections.Generic;
using System.Linq;
using System.Data;
using Dapper;
using Npgsql;
using UserManagement.Models;

namespace UserManagement.Data
{
    public class ApiContext
    {
        private readonly string _connectionString;

        public ApiContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        private IDbConnection CreateConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }

        public void AddUser(UserModel user)
        {
            using var connection = CreateConnection();
            var sql = "INSERT INTO users (Username, Name, Email, Password, Notes) VALUES (@Username, @Name, @Email, @Password, @Notes);";
            connection.Execute(sql, user);
        }

        public IEnumerable<UserModel> GetUsers()
        {
            using var connection = CreateConnection();
            return connection.Query<UserModel>("SELECT * FROM users;");
        }

        public UserModel GetByUsername(string username)
        {
            using var connection = CreateConnection();
            return connection.QuerySingleOrDefault<UserModel>("SELECT * FROM users WHERE Username = @Username;", new { Username = username });
        }

        public void UpdateUser(UserModel user)
        {
            using var connection = CreateConnection();
            var sql = @"UPDATE users 
                SET Username = @Username, Name = @Name, Email = @Email, Password = @Password, Notes = @Notes 
                WHERE Id = @Id;";
            connection.Execute(sql, user);
        }

        public void DeleteUserByUsername(string username)
        {
            using var connection = CreateConnection();
            var sql = "DELETE FROM users WHERE Username = @Username;";
            connection.Execute(sql, new { Username = username });
        }
    }
}
