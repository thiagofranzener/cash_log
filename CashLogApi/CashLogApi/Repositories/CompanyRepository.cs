using System;
using CashLogApi.Models;
using System.Data.SqlClient;
using CashLogUtils;
using System.Text;
using System.Data;
using System.Collections.Generic;

namespace CashLogApi.Repositories
{
    class CompanyRepository : Repository
    {
        private SqlConnection connection;
        CashLogUtils.Helpers helper = new Helpers();

        public CompanyRepository(SqlConnection connection)
        {
            this.connection = connection;
        }

        public int Add(Company company)
        {
            company.Id = this.GetNextId();
            SqlCommand command = this.connection.CreateCommand();
            command.CommandText = this.GetAddQuery();
            this.PopulateQuery(command, company);
            command.ExecuteNonQuery();

            ExpenseCategoryRepository expenseCategoryRepository = new ExpenseCategoryRepository(this.connection);
            expenseCategoryRepository.Add(company.Id, "Hospedagem");
            expenseCategoryRepository.Add(company.Id, "Transporte");
            expenseCategoryRepository.Add(company.Id, "Alimentação");
            expenseCategoryRepository.Add(company.Id, "Outros");

            return company.Id;
        }

        public bool Exist(int id)
        {
            bool retorno = false;
            SqlCommand command = new SqlCommand($"SELECT COUNT(company_id) FROM Company WHERE company_id={id}");
            if (helper.ValInt(command.ExecuteScalar().ToString()) > 0)
            {
                retorno = true;
            }
            return retorno;
        }

        public void Update(Company company)
        {
            StringBuilder sql = new StringBuilder(this.GetUpdateQuery(company.Id));
            SqlCommand command = connection.CreateCommand();
            command.CommandText = sql.ToString();
            this.PopulateQuery(command, company);
            command.ExecuteNonQuery();
        }

        public Company FindById(int id)
        {
            StringBuilder sql = new StringBuilder(this.GetFindQuery());
            Company company = new Company();
            SqlCommand command = this.connection.CreateCommand();
            command.CommandText = sql.ToString();
            using (SqlDataReader dr = command.ExecuteReader())
            {
                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        this.Populate(dr, company);
                    }
                }
            }
            return company;
        }

        public Company FindByUsername(string username)
        {
            StringBuilder sql = new StringBuilder(this.GetFindQuery());
            sql.AppendLine("JOIN CompanyUser cus ON");
            sql.AppendLine("com.company_id=cus.company_id");
            sql.AppendLine("WHERE cus.username=@userName");
            Company company = new Company();
            SqlCommand command = this.connection.CreateCommand();
            command.CommandText = sql.ToString();
            command.Parameters.AddWithValue("@userName", SqlDbType.VarChar).Value = username;
            using (SqlDataReader dr = command.ExecuteReader())
            {
                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        this.Populate(dr, company);
                    }
                }
            }
            return company;
        }

        public string FindManager(int companyId)
        {
            string result = string.Empty;
            SqlCommand command = this.connection.CreateCommand();
            command.CommandText = GetFindManagerQuery(companyId);
            using (SqlDataReader dr = command.ExecuteReader())
            {
                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        result = dr.GetString(0);
                    }
                }
            }
            return result;
        }

        public HashSet<Company> Find()
        {
            HashSet<Company> result = new HashSet<Company>();
            SqlCommand command = this.connection.CreateCommand();
            command.CommandText = this.GetFindQuery();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Company company = new Company();
                    this.Populate(reader, company);
                    result.Add(company);
                }
            }
            return result;
        }

        public Company FindByToken(Guid token)
        {
            Company result = new Company();
            StringBuilder query = new StringBuilder(this.GetFindQuery());
            query.AppendLine("WHERE token=@token");
            SqlCommand command = this.connection.CreateCommand();
            command.CommandText = query.ToString();
            command.Parameters.AddWithValue("@token", DbType.Guid).Value = token;
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        this.Populate(reader, result);
                    }
                }
            }
            return result;
        }

        public void Delete(int id)
        {
            SqlCommand command = this.connection.CreateCommand();
            command.CommandText = $"DELETE FROM User WHERE company_id={id}";
            command.ExecuteNonQuery();

            command.CommandText = $"DELETE FROM Company WHERE company_id={id}";
            command.ExecuteNonQuery();
        }

        public int GetNextId()
        {
            int nextId = 0;
            SqlCommand command = new SqlCommand("SELECT ISNULL(MAX(company_id),0) + 1 FROM Company", this.connection);
            using (SqlDataReader dr = command.ExecuteReader())
            {
                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        nextId = dr.GetInt32(0);
                    }
                }
            }
            return nextId;
        }

        public void PopulateQuery(SqlCommand command, Company company)
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

        public void Populate(SqlDataReader reader, Company company)
        {
            company.Id = reader.GetInt32(0);
            company.Name = reader.GetString(1);
            company.Cnpj = reader.GetString(2);
            company.State = reader.GetString(3);
            company.City = reader.GetString(4);
            company.Address = reader.GetString(5);
            company.Complement = reader.GetString(6);
            company.Number = reader.GetInt32(7);
            company.Cep = reader.GetString(8);
            company.Token = reader.GetGuid(9);
        }

        public string GetAddQuery()
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("INSERT INTO Company");
            sql.AppendLine("(company_id,name,cnpj,address,number,complement,city,state,cep,token)");
            sql.AppendLine("VALUES");
            sql.AppendLine("(@company_id,@name,@cnpj,@address,@number,@complement,@city,@state,@cep,NEWID())");
            return sql.ToString();
        }

        public string GetFindQuery()
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT");
            sql.AppendLine("com.company_id");
            sql.AppendLine(",com.name");
            sql.AppendLine(",com.cnpj");
            sql.AppendLine(",com.state");
            sql.AppendLine(",com.city");
            sql.AppendLine(",com.address");
            sql.AppendLine(",com.complement");
            sql.AppendLine(",com.number");
            sql.AppendLine(",com.cep");
            sql.AppendLine(",com.token");
            sql.AppendLine("FROM Company com");
            return sql.ToString();
        }

        public string GetFindManagerQuery(int companyId)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT username FROM CompanyUser");
            sql.AppendFormat("WHERE company_id={0}", companyId).AppendLine();
            sql.AppendFormat("AND type={0}", helper.ValInt(UserRole.Administrator.ToString()));
            return sql.ToString();
        }

        public string GetUpdateQuery(int companyId)
        {
            StringBuilder sql = new StringBuilder();
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

        protected override void Dispose(bool disposing)
        {
            this.connection.Close();
        }

    }
}
