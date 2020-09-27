using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using CashLogLib.Models;
using Microsoft.VisualBasic.CompilerServices; // Install-Package Microsoft.VisualBasic

namespace CashLogLib.Repositories
{
    public partial class CompanyRepository : Repository
    {
        private readonly SqlConnection connection;

        public CompanyRepository(SqlConnection connection)
        {
            this.connection = connection;
        }

        public int Add(Company company)
        {
            company.Id = GetNextId();
            var command = connection.CreateCommand();
            command.CommandText = GetAddQuery();
            PopulateQuery(command, company);
            command.ExecuteNonQuery();
            var expenseCategoryRepository = new ExpenseCategoryRepository(connection);
            expenseCategoryRepository.Add(company.Id, "Hospedagem");
            expenseCategoryRepository.Add(company.Id, "Transporte");
            expenseCategoryRepository.Add(company.Id, "Alimentação");
            expenseCategoryRepository.Add(company.Id, "Outros");
            return company.Id;
        }

        public bool Exist(int id)
        {
            var command = connection.CreateCommand();
            command.CommandText = $"SELECT COUNT(company_id) FROM Company WHERE company_id={id}";
            if (Conversions.ToBoolean(Operators.ConditionalCompareObjectGreater(command.ExecuteScalar(), 0, false)))
                return true;
            return false;
        }

        public void Update(Company company)
        {
            var sql = new StringBuilder(GetUpdateQuery(company.Id));
            var command = connection.CreateCommand();
            command.CommandText = sql.ToString();
            PopulateQuery(command, company);
            command.ExecuteNonQuery();
        }

        public Company FindById(int id)
        {
            var sql = new StringBuilder(GetFindQuery());
            sql.AppendFormat("WHERE com.company_id={0}", id);
            var command = connection.CreateCommand();
            command.CommandText = sql.ToString();
            using (IDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    var company = new Company();
                    Populate(reader, company);
                    return company;
                }
            }

            return default;
        }

        public Company FindByUsername(string username)
        {
            var sql = new StringBuilder(GetFindQuery());
            sql.AppendLine("JOIN CompanyUser cus ON");
            sql.AppendLine("com.company_id=cus.company_id");
            sql.AppendLine("WHERE cus.username=@userName");
            var command = connection.CreateCommand();
            command.CommandText = sql.ToString();
            command.Parameters.AddWithValue("@userName", SqlDbType.VarChar).Value = username;
            using (IDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    var company = new Company();
                    Populate(reader, company);
                    return company;
                }
            }

            return default;
        }

        public string FindManager(int companyId)
        {
            string result = null;
            var command = connection.CreateCommand();
            command.CommandText = GetFindManagerQuery(companyId);
            using (IDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    result = Conversions.ToString(reader[0]);
                }
            }

            return result;
        }

        public HashSet<Company> Find()
        {
            var result = new HashSet<Company>();
            var command = connection.CreateCommand();
            command.CommandText = GetFindQuery();
            using (IDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var company = new Company();
                    Populate(reader, company);
                    result.Add(company);
                }
            }

            return result;
        }

        public Company FindByToken(Guid token)
        {
            Company result = default;
            var query = new StringBuilder(GetFindQuery());
            query.AppendLine("WHERE token=@token");
            var command = connection.CreateCommand();
            command.CommandText = query.ToString();
            command.Parameters.AddWithValue("@token", DbType.Guid).Value = token;
            using (IDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    var company = new Company();
                    Populate(reader, company);
                    result = company;
                }
            }

            return result;
        }

        public void Delete(int id)
        {
            var command = connection.CreateCommand();
            command.CommandText = $"DELETE FROM User WHERE company_id={id}";
            command.ExecuteNonQuery();
            command.CommandText = $"DELETE FROM Company WHERE company_id={id}";
            command.ExecuteNonQuery();
        }

        private int GetNextId()
        {
            var command = connection.CreateCommand();
            command.CommandText = "SELECT ISNULL(MAX(company_id),0) + 1 FROM Company";
            return Conversions.ToInteger(command.ExecuteScalar());
        }

        private static void PopulateQuery(SqlCommand command, Company company)
        {
            command.Parameters.AddWithValue("@company_id", company.Id);
            command.Parameters.AddWithValue("@name", company.Name);
            command.Parameters.AddWithValue("@cnpj", company.Cnpj);
            command.Parameters.AddWithValue("@address", company.Address);
            command.Parameters.AddWithValue("@number", company.Number);
            command.Parameters.AddWithValue("@complement", company.Complement);
            command.Parameters.AddWithValue("@city", company.City);
            command.Parameters.AddWithValue("@state", company.State);
            command.Parameters.AddWithValue("@cep", company.Cep);
        }

        private static void Populate(IDataReader reader, Company company)
        {
            company.Id = (int)reader["company_id"];
            company.Name = (string)reader["name"];
            company.Cnpj = (string)reader["cnpj"];
            company.State = (string)reader["state"];
            company.City = (string)reader["city"];
            company.Address = (string)reader["address"];
            company.Complement = (string)reader["complement"];
            company.Number = (int)reader["number"];
            company.Cep = (string)reader["cep"];
            company.Token = (Guid)reader["token"];
        }

        private static string GetAddQuery()
        {
            var sql = new StringBuilder();
            sql.AppendLine("INSERT INTO Company");
            sql.AppendLine("(company_id,name,cnpj,address,number,complement,city,state,cep,token)");
            sql.AppendLine("VALUES");
            sql.AppendLine("(@company_id,@name,@cnpj,@address,@number,@complement,@city,@state,@cep,NEWID())");
            return sql.ToString();
        }

        private static string GetFindQuery()
        {
            var sql = new StringBuilder();
            sql.AppendLine("SELECT");
            sql.AppendLine("com.company_id");
            sql.AppendLine(",com.name");
            sql.AppendLine(",com.cnpj");
            sql.AppendLine(",com.address");
            sql.AppendLine(",com.number");
            sql.AppendLine(",com.complement");
            sql.AppendLine(",com.city");
            sql.AppendLine(",com.state");
            sql.AppendLine(",com.cep");
            sql.AppendLine(",com.token");
            sql.AppendLine("FROM Company com");
            return sql.ToString();
        }

        private static string GetFindManagerQuery(int companyId)
        {
            var sql = new StringBuilder();
            sql.AppendLine("SELECT username FROM CompanyUser");
            sql.AppendFormat("WHERE company_id={0}", companyId).AppendLine();
            sql.AppendFormat("AND type={0}", UserRole.Administrator);
            return sql.ToString();
        }

        private static string GetUpdateQuery(int companyId)
        {
            var sql = new StringBuilder();
            sql.AppendLine("UPDATE Company SET");
            sql.AppendLine("company_id=@company_id");
            sql.AppendLine(",name=@name");
            sql.AppendLine(",cnpj=@cnpj");
            sql.AppendLine(",address=@address");
            sql.AppendLine(",number=@number");
            sql.AppendLine(",complement=@complement");
            sql.AppendLine(",city=@city");
            sql.AppendLine(",state=@state");
            sql.AppendLine(",cep=@cep");
            sql.AppendFormat("WHERE company_id={0}", companyId);
            return sql.ToString();
        }

        public override void Dispose()
        {
            connection.Close();
        }
    }
}
