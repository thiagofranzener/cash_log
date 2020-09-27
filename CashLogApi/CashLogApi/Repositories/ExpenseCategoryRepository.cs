using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using CashLogLib.Models;
using Microsoft.VisualBasic.CompilerServices; // Install-Package Microsoft.VisualBasic

namespace CashLogLib.Repositories
{
    public partial class ExpenseCategoryRepository : Repository
    {
        private readonly SqlConnection connection;

        public ExpenseCategoryRepository(SqlConnection connection)
        {
            this.connection = connection;
        }

        public int Add(int companyId, string description)
        {
            int nextId = GetNextId(companyId);
            var expenseCategory = new ExpenseCategory()
            {
                CompanyId = companyId,
                CategoryId = nextId,
                Description = description
            };
            var command = connection.CreateCommand();
            command.CommandText = GetAddQuery();
            PopulateQuery(command, expenseCategory);
            command.ExecuteNonQuery();
            return nextId;
        }

        public void Update(ExpenseCategory expenseCategory)
        {
            var command = connection.CreateCommand();
            command.CommandText = GetUpdateQuery();
            PopulateQuery(command, expenseCategory);
            command.ExecuteNonQuery();
        }

        public HashSet<ExpenseCategory> GetAll(int companyId)
        {
            var command = connection.CreateCommand();
            command.CommandText = GetFindQuery(companyId);
            var result = new HashSet<ExpenseCategory>();
            using (IDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var expenseCategory = new ExpenseCategory();
                    Populate(reader, expenseCategory);
                    result.Add(expenseCategory);
                }
            }

            return result;
        }

        public ExpenseCategory GetById(int companyId, int categoryId)
        {
            var sql = new StringBuilder(GetFindQuery(companyId));
            sql.AppendFormat("AND type_id={0}", categoryId);
            var command = connection.CreateCommand();
            command.CommandText = sql.ToString();
            using (IDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    var expenseCategory = new ExpenseCategory();
                    Populate(reader, expenseCategory);
                    return expenseCategory;
                }
            }

            return default;
        }

        public void Delete(int companyId, int categoryId)
        {
            var command = connection.CreateCommand();
            command.CommandText = GetDeleteQuery(companyId, categoryId);
            command.ExecuteNonQuery();
        }

        private void Populate(IDataReader reader, ExpenseCategory expenseCategory)
        {
            expenseCategory.CompanyId = (int)reader["company_id"];
            expenseCategory.CategoryId = (int)reader["type_id"];
            expenseCategory.Description = (string)reader["description"];
        }

        public bool Exist(int companyId, int expenseCategoryId)
        {
            var command = connection.CreateCommand();
            command.CommandText = $"SELECT COUNT(type_id) FROM ExpenseType WHERE company_id=@company_id AND type_id=@type_id";
            command.Parameters.AddWithValue("@company_id", SqlDbType.Int).Value = companyId;
            command.Parameters.AddWithValue("@type_id", SqlDbType.Int).Value = expenseCategoryId;
            if (Conversions.ToBoolean(Operators.ConditionalCompareObjectGreater(command.ExecuteScalar(), 0, false)))
                return true;
            return false;
        }

        private int GetNextId(int companyId)
        {
            var command = connection.CreateCommand();
            command.CommandText = "SELECT ISNULL(MAX(type_id),0) + 1 FROM ExpenseType WHERE company_id=@company_id";
            command.Parameters.AddWithValue("@company_id", SqlDbType.Int).Value = companyId;
            return Conversions.ToInteger(command.ExecuteScalar());
        }

        private static string GetAddQuery()
        {
            var sql = new StringBuilder();
            sql.AppendLine("INSERT INTO ExpenseType");
            sql.AppendLine("(company_id,type_id,description)");
            sql.AppendLine("VALUES");
            sql.AppendLine("(@company_id,@type_id,@description)");
            return sql.ToString();
        }

        private static string GetUpdateQuery()
        {
            var sql = new StringBuilder();
            sql.AppendLine("UPDATE ExpenseType SET");
            sql.AppendLine("description=@description");
            sql.AppendFormat("WHERE company_id=@company_id").AppendLine();
            sql.AppendFormat("AND type_id=@type_id");
            return sql.ToString();
        }

        private static string GetDeleteQuery(int companyId, int categoryId)
        {
            var sql = new StringBuilder();
            sql.AppendLine("DELETE FROM ExpenseType");
            sql.AppendFormat("WHERE company_Id={0}", companyId).AppendLine();
            sql.AppendFormat("AND type_id={0}", categoryId);
            return sql.ToString();
        }

        private static string GetFindQuery(int companyId)
        {
            var sql = new StringBuilder();
            sql.AppendLine("SELECT");
            sql.AppendLine("company_id");
            sql.AppendLine(",type_id");
            sql.AppendLine(",description");
            sql.AppendLine("FROM ExpenseType");
            sql.AppendFormat("WHERE company_id={0}", companyId);
            return sql.ToString();
        }

        private static void PopulateQuery(SqlCommand command, ExpenseCategory expenseCategory)
        {
            command.Parameters.AddWithValue("@description", expenseCategory.Description);
            command.Parameters.AddWithValue("@company_id", expenseCategory.CompanyId);
            command.Parameters.AddWithValue("@type_id", expenseCategory.CategoryId);
        }

        public override void Dispose()
        {
            connection.Close();
        }
    }
}