using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using CashLogLib.Models;
using Microsoft.VisualBasic.CompilerServices; // Install-Package Microsoft.VisualBasic

namespace CashLogLib.Repositories
{
    public partial class UserRepository : Repository
    {
        private readonly SqlConnection connection;

        public UserRepository(SqlConnection connection)
        {
            this.connection = connection;
        }

        public void Add(UserType user)
        {
            user.Role = UserRole.Employee;
            if (IsFirstUserInCompany(user.CompanyId))
                user.Role = UserRole.Administrator;
            var command = connection.CreateCommand();
            command.CommandText = GetAddQuery();
            PopulateQuery(command, user);
            command.ExecuteNonQuery();
        }

        public bool Exist(string user)
        {
            var command = connection.CreateCommand();
            command.CommandText = $"SELECT COUNT(username) FROM CompanyUser WHERE username=@username";
            command.Parameters.AddWithValue("@username", SqlDbType.VarChar).Value = user;
            if (Conversions.ToBoolean(Operators.ConditionalCompareObjectGreater(command.ExecuteScalar(), 0, false)))
                return true;
            return false;
        }

        public Guid Login(string user, string password)
        {
            var command = connection.CreateCommand();
            command.CommandText = $"SELECT token FROM CompanyUser WHERE username=@username AND password=@password";
            command.Parameters.AddWithValue("@username", SqlDbType.VarChar).Value = user;
            command.Parameters.AddWithValue("@password", SqlDbType.VarChar).Value = EncryptString(password);
            using (IDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    return (Guid)reader["token"];
                }
            }

            return default;
        }

        public List<UserType> GetUsersByCompanyId(int companyId)
        {
            var sql = new StringBuilder(GetFindQuery());
            sql.AppendLine("WHERE cus.company_id=@companyId");
            var result = new List<UserType>();
            var command = connection.CreateCommand();
            command.CommandText = sql.ToString();
            command.Parameters.AddWithValue("@companyId", SqlDbType.Int).Value = companyId;
            using (IDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var user = new UserType();
                    PopulateUser(user, reader);
                    result.Add(user);
                }
            }

            return result;
        }

        public UserType GetUser(string username)
        {
            var sql = new StringBuilder(GetFindQuery());
            sql.AppendLine("WHERE cus.username=@username");
            var command = connection.CreateCommand();
            command.CommandText = sql.ToString();
            command.Parameters.AddWithValue("@username", SqlDbType.VarChar).Value = username;
            using (IDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    var user = new UserType();
                    PopulateUser(user, reader);
                    return user;
                }
            }

            return default;
        }

        private bool IsFirstUserInCompany(int companyId)
        {
            var command = connection.CreateCommand();
            command.CommandText = $"SELECT COUNT(username) FROM CompanyUser WHERE company_id=@company_id";
            command.Parameters.AddWithValue("@company_id", SqlDbType.Int).Value = companyId;
            if (Conversions.ToBoolean(Operators.ConditionalCompareObjectGreater(command.ExecuteScalar(), 0, false)))
                return false;
            return true;
        }

        private static string GetFindQuery()
        {
            var sql = new StringBuilder();
            sql.AppendLine("SELECT");
            sql.AppendLine("cus.username");
            sql.AppendLine(",cus.cpf");
            sql.AppendLine(",cus.name");
            sql.AppendLine(",cus.email");
            sql.AppendLine(",cus.company_id");
            sql.AppendLine(",cus.type");
            sql.AppendLine("FROM CompanyUser cus");
            return sql.ToString();
        }

        private static void PopulateUser(UserType user, IDataReader reader)
        {
            user.User = (string)reader["username"];
            user.Cpf = (string)reader["cpf"];
            user.Name = (string)reader["name"];
            user.Email = (string)reader["email"];
            user.CompanyId = (int)reader["company_id"];
            user.Role = (UserRole)reader["type"];
        }

        private static void PopulateQuery(SqlCommand command, UserType user)
        {
            command.Parameters.AddWithValue("@username", user.User);
            command.Parameters.AddWithValue("@name", user.Name);
            command.Parameters.AddWithValue("@password", EncryptString(user.Password));
            command.Parameters.AddWithValue("@email", user.Email);
            command.Parameters.AddWithValue("@cpf", user.Cpf);
            command.Parameters.AddWithValue("@company_id", user.CompanyId);
            command.Parameters.AddWithValue("@type", user.Role);
        }

        private static string GetAddQuery()
        {
            var sql = new StringBuilder();
            sql.AppendLine("INSERT INTO CompanyUser");
            sql.AppendLine("(username,name,password,email,cpf,company_id,token,type)");
            sql.AppendLine("VALUES");
            sql.AppendLine("(@username,@name,@password,@email,@cpf,@company_id,NEWID(),@type)");
            return sql.ToString();
        }

        public override void Dispose()
        {
            connection.Close();
        }

        public static string EncryptString(string value)
        {
            string result = "";
            var cryptoService = new System.Security.Cryptography.MD5CryptoServiceProvider();
            var bytesToHash = Encoding.ASCII.GetBytes(value);
            bytesToHash = cryptoService.ComputeHash(bytesToHash);
            foreach (byte streamByte in bytesToHash)
                result += streamByte.ToString("x2");
            return result.ToUpper();
        }
    }
}
