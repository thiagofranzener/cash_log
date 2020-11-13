using System;
using System.Data;
using CashLogApi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using CashLogUtils;

namespace CashLogApi.Repositories
{
    class ExpenseCategoryRepository : Repository
    {
        private SqlConnection connection;
        Helpers helper = new Helpers();

        public ExpenseCategoryRepository(SqlConnection connection)
        {
            this.connection = connection;
        }

        public int Add(int companyId, string description)
        {
            int nextId = this.GetNextId(companyId);
            ExpenseCategory expenseCategory = new ExpenseCategory();
            expenseCategory.CompanyId = companyId;
            expenseCategory.CategoryId = nextId;
            expenseCategory.Description = description;
            SqlCommand command = this.connection.CreateCommand();
            command.CommandText = this.GetAddQuery();
            this.PopulateQuery(command, expenseCategory);
            command.ExecuteNonQuery();
            return nextId;
        }

        public void Update(ExpenseCategory expenseCategory)
        {
            SqlCommand command = this.connection.CreateCommand();
            command.CommandText = this.GetUpdateQuery();
            this.PopulateQuery(command, expenseCategory);
            command.ExecuteNonQuery();
        }

        public HashSet<ExpenseCategory> GetAll(int companyId)
        {
            HashSet<ExpenseCategory> result = new HashSet<ExpenseCategory>();
            SqlCommand command = this.connection.CreateCommand();
            command.CommandText = this.GetFindQuery(companyId);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    ExpenseCategory expenseCategory = new ExpenseCategory();
                    this.Populate(reader, expenseCategory);
                    result.Add(expenseCategory);
                }
            }
            return result;
        }

        public ExpenseCategory GetById(int companyId, int categoryId)
        {
            ExpenseCategory expenseCategory = new ExpenseCategory();
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("AND type_id={0}", categoryId);
            SqlCommand command = this.connection.CreateCommand();
            command.CommandText = sql.ToString();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        this.Populate(reader, expenseCategory);
                    }
                }
            }
            return expenseCategory;
        }

        public void Delete(int companyId, int categoryId)
        {
            SqlCommand command = this.connection.CreateCommand();
            command.CommandText = GetDeleteQuery(companyId, categoryId);
            command.ExecuteNonQuery();
        }

        public void Populate(SqlDataReader reader, ExpenseCategory expenseCategory)
        {
            expenseCategory.CompanyId = reader.GetInt32(0);
            expenseCategory.CategoryId = reader.GetInt32(1);
            expenseCategory.Description = reader.GetString(2);
        }

        public bool Exist(int companyId, int expenseCategoryId)
        {
            bool retorno = false;
            SqlCommand command = this.connection.CreateCommand();
            command.CommandText = $"SELECT COUNT(type_id) FROM ExpenseType WHERE company_id=@company_id AND type_id=@type_id";
            command.Parameters.AddWithValue("@company_id", SqlDbType.Int).Value = companyId;
            command.Parameters.AddWithValue("@type_id", SqlDbType.Int).Value = expenseCategoryId;
            if (helper.ValInt(command.ExecuteScalar().ToString()) > 0)
            {
                retorno = true;
            }
            return retorno;
        }

        public int GetNextId(int companyId)
        {
            SqlCommand command = this.connection.CreateCommand();
            command.CommandText = "SELECT ISNULL(MAX(type_id),0) + 1 FROM ExpenseType WHERE company_id=@company_id";
            command.Parameters.AddWithValue("@company_id", SqlDbType.Int).Value = companyId;
            return helper.ValInt(command.ExecuteScalar().ToString());
        }

        public string GetAddQuery()
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("INSERT INTO ExpenseType");
            sql.AppendLine("(company_id,type_id,description)");
            sql.AppendLine("VALUES");
            sql.AppendLine("(@company_id,@type_id,@description)");
            return sql.ToString();
        }

        public string GetUpdateQuery()
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("UPDATE ExpenseType SET");
            sql.AppendLine("description=@description");
            sql.AppendFormat("WHERE company_id=@company_id").AppendLine();
            sql.AppendFormat("AND type_id=@type_id");
            return sql.ToString();
        }

        public string GetDeleteQuery(int companyId, int categoryId)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("DELETE FROM ExpenseType");
            sql.AppendFormat("WHERE company_Id={0}", companyId).AppendLine();
            sql.AppendFormat("AND type_id={0}", categoryId);
            return sql.ToString();
        }

        public string GetFindQuery(int companyId)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT");
            sql.AppendLine("company_id");
            sql.AppendLine(",type_id");
            sql.AppendLine(",description");
            sql.AppendLine("FROM ExpenseType");
            sql.AppendFormat("WHERE company_id={0}", companyId);
            return sql.ToString();
        }

        public void PopulateQuery(SqlCommand command, ExpenseCategory expenseCategory)
        {
            command.Parameters.AddWithValue("@description", expenseCategory.Description);
            command.Parameters.AddWithValue("@company_id", expenseCategory.CompanyId);
            command.Parameters.AddWithValue("@type_id", expenseCategory.CategoryId);
        }

        protected override void Dispose(bool disposing)
        {
            this.connection.Close();
        }

    }
}
