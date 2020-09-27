using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using CashLogLib.Models;
using Microsoft.VisualBasic.CompilerServices; // Install-Package Microsoft.VisualBasic

namespace CashLogLib.Repositories
{
    public partial class ExpenseRepository : Repository
    {
        private readonly SqlConnection connection;

        public ExpenseRepository(SqlConnection connection)
        {
            this.connection = connection;
        }

        public int AddEmployeeRequest(ExpenseEmployeeRequest request)
        {
            int nextId = GetNextId(request.CompanyId);
            var command = connection.CreateCommand();
            command.CommandText = GetAddEmployeeRequestQuery();
            PopulateEmployeeRequestQuery(command, request, nextId);
            command.ExecuteNonQuery();
            return nextId;
        }

        public void UpdateExpense(ExpenseAdministratorApprovation request)
        {
            var command = connection.CreateCommand();
            command.CommandText = GetUpdateEmployeeRequestQuery();
            PopulateAdministrationApprovationQuery(command, request);
            command.ExecuteNonQuery();
        }

        public HashSet<Expense> GetAll(int companyId, UserType user)
        {
            var result = new HashSet<Expense>();
            var sql = new StringBuilder(GetFindQuery(companyId));
            var command = connection.CreateCommand();
            if (user.Role == UserRole.Employee)
            {
                sql.AppendFormat("AND exp.user_request=@user_request");
                command.Parameters.AddWithValue("@user_request", DbType.String).Value = user.User;
            }

            command.CommandText = sql.ToString();
            using (IDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var expense = new Expense();
                    Populate(reader, expense);
                    result.Add(expense);
                }
            }

            return result;
        }

        public Expense GetById(int companyId, int expenseId, UserType user)
        {
            Expense result = default;
            var sql = new StringBuilder(GetFindQuery(companyId));
            sql.AppendLine().AppendFormat("AND expense_id={0}", expenseId);
            var command = connection.CreateCommand();
            if (user.Role == UserRole.Employee)
            {
                sql.AppendFormat("AND exp.user_request=@user_request");
                command.Parameters.AddWithValue("@user_request", DbType.String).Value = user.User;
            }

            command.CommandText = sql.ToString();
            using (IDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    var expense = new Expense();
                    Populate(reader, expense);
                    result = expense;
                }
            }

            return result;
        }

        private static string GetAddEmployeeRequestQuery()
        {
            var sql = new StringBuilder();
            sql.AppendLine("INSERT INTO Expense");
            sql.AppendLine("(expense_id,description,cost,date,user_request,user_manager,picture,type_id,situation,company_id)");
            sql.AppendLine("VALUES").AppendLine();
            sql.AppendLine("(@expense_id,@description,@cost,@date,@user_request,@user_manager,@picture,@type_id,@situation,@company_id)");
            return sql.ToString();
        }

        private static string GetUpdateEmployeeRequestQuery()
        {
            var sql = new StringBuilder();
            sql.AppendLine("UPDATE Expense");
            sql.AppendLine("SET cost_approved=@cost_approved");
            sql.AppendLine(",manager_motive=@manager_motive");
            sql.AppendLine(",situation=@situation");
            sql.AppendLine(",date_approved=@date_approved");
            sql.AppendLine("WHERE company_id=@company_id AND expense_id=@expense_id");
            return sql.ToString();
        }

        private static void PopulateEmployeeRequestQuery(SqlCommand command, ExpenseEmployeeRequest request, int expenseId)
        {
            command.Parameters.AddWithValue("@expense_id", SqlDbType.Int).Value = expenseId;
            command.Parameters.AddWithValue("@company_id", SqlDbType.Int).Value = request.CompanyId;
            command.Parameters.AddWithValue("@description", SqlDbType.VarChar).Value = request.Description;
            command.Parameters.AddWithValue("@date", SqlDbType.Date).Value = request.Date;
            command.Parameters.AddWithValue("@user_manager", SqlDbType.VarChar).Value = request.ManagerUser;
            command.Parameters.AddWithValue("@user_request", SqlDbType.VarChar).Value = request.RequestUser;
            command.Parameters.AddWithValue("@type_id", SqlDbType.Int).Value = request.TypeId;
            command.Parameters.AddWithValue("@situation", SqlDbType.Int).Value = request.Situation;
            command.Parameters.AddWithValue("@cost", SqlDbType.Float).Value = request.Cost;
            command.Parameters.AddWithValue("@picture", SqlDbType.Image).Value = request.Picture;
        }

        private static void PopulateAdministrationApprovationQuery(SqlCommand command, ExpenseAdministratorApprovation request)
        {
            if (request.Motive == (default))
                request.Motive = "";
            if (request.ApprovedCost == (default))
                request.ApprovedCost = 0;
            command.Parameters.AddWithValue("@expense_id", SqlDbType.Int).Value = request.ExpenseId;
            command.Parameters.AddWithValue("@company_id", SqlDbType.Int).Value = request.CompanyId;
            command.Parameters.AddWithValue("@cost_approved", SqlDbType.Float).Value = request.ApprovedCost;
            command.Parameters.AddWithValue("@manager_motive", SqlDbType.VarChar).Value = request.Motive;
            command.Parameters.AddWithValue("@situation", SqlDbType.VarChar).Value = (object)request.Situation;
            command.Parameters.AddWithValue("@date_approved", SqlDbType.Date).Value = DateTime.Now;
        }

        private int GetNextId(int companyId)
        {
            var command = connection.CreateCommand();
            command.CommandText = "SELECT ISNULL(MAX(expense_id),0) + 1 FROM Expense WHERE company_id=@company_id";
            command.Parameters.AddWithValue("@company_id", SqlDbType.Int).Value = companyId;
            return Conversions.ToInteger(command.ExecuteScalar());
        }

        public bool Exist(int companyId, int expenseId)
        {
            var command = connection.CreateCommand();
            command.CommandText = $"SELECT COUNT(expense_id) FROM Expense WHERE company_id={companyId} AND expense_id={expenseId}";
            if (Conversions.ToBoolean(Operators.ConditionalCompareObjectGreater(command.ExecuteScalar(), 0, false)))
                return true;
            return false;
        }

        // Public Sub Update(company As Company)
        // Dim sql As New StringBuilder(GetUpdateQuery())
        // sql.AppendFormat("WHERE company_id={0}", company.Id)
        // Dim command As SqlCommand = connection.CreateCommand()
        // command.CommandText = sql.ToString()
        // PopulateQuery(command, company)
        // command.ExecuteNonQuery()
        // End Sub

        // Public Function FindById(id As Integer) As Company
        // Dim sql As New StringBuilder(GetFindQuery())
        // sql.AppendFormat("WHERE com.company_id={0}", id)
        // Dim command = connection.CreateCommand()
        // command.CommandText = sql.ToString()
        // Using reader As IDataReader = command.ExecuteReader()
        // If reader.Read() Then
        // Dim company As New Company()
        // Populate(reader, company)
        // Return company
        // End If
        // End Using
        // Return Nothing
        // End Function

        // Public Function FindByUsername(username As String) As Company
        // Dim sql As New StringBuilder(GetFindQuery())
        // sql.AppendLine("JOIN CompanyUser cus ON")
        // sql.AppendLine("com.company_id=cus.company_id")
        // sql.AppendLine("WHERE cus.username=@userName")
        // Dim command = connection.CreateCommand()
        // command.CommandText = sql.ToString()
        // command.Parameters.AddWithValue("@userName", SqlDbType.VarChar).Value = username
        // Using reader As IDataReader = command.ExecuteReader()
        // If reader.Read() Then
        // Dim company As New Company()
        // Populate(reader, company)
        // Return company
        // End If
        // End Using
        // Return Nothing
        // End Function

        // Public Function Find() As HashSet(Of Company)
        // Dim result As New HashSet(Of Company)
        // Dim command = connection.CreateCommand()
        // command.CommandText = GetFindQuery()
        // Using reader As IDataReader = command.ExecuteReader()
        // While reader.Read()
        // Dim company As New Company()
        // Populate(reader, company)
        // result.Add(company)
        // End While
        // End Using
        // Return result
        // End Function

        // Public Sub Delete(id As Integer)
        // Dim command As SqlCommand = connection.CreateCommand()
        // command.CommandText = $"DELETE FROM User WHERE company_id={id}"
        // command.ExecuteNonQuery()

        // command.CommandText = $"DELETE FROM Company WHERE company_id={id}"
        // command.ExecuteNonQuery()
        // End Sub

        // Private Function GetNextId() As Integer
        // Dim command As SqlCommand = connection.CreateCommand()
        // command.CommandText = "SELECT ISNULL(MAX(company_id),0) + 1 FROM Company"
        // Return command.ExecuteScalar()
        // End Function

        // Private Shared Sub PopulateQuery(command As SqlCommand, company As Company)
        // command.Parameters.AddWithValue("@company_id", company.Id)
        // command.Parameters.AddWithValue("@name", company.Name)
        // command.Parameters.AddWithValue("@cnpj", company.Cnpj)
        // command.Parameters.AddWithValue("@address", company.Address)
        // command.Parameters.AddWithValue("@number", company.Number)
        // command.Parameters.AddWithValue("@complement", company.Complement)
        // command.Parameters.AddWithValue("@city", company.City)
        // command.Parameters.AddWithValue("@state", company.State)
        // command.Parameters.AddWithValue("@cep", company.Cep)
        // End Sub


        // Private Shared Function GetAddQuery() As String
        // Dim sql As New StringBuilder()
        // sql.AppendLine("INSERT INTO Company")
        // sql.AppendLine("(company_id,name,cnpj,address,number,complement,city,state,cep)")
        // sql.AppendLine("VALUES")
        // sql.AppendLine("(@company_id,@name,@cnpj,@address,@number,@complement,@city,@state,@cep)")
        // Return sql.ToString()
        // End Function

        // Private Shared Function GetFindQuery() As String
        // Dim sql As New StringBuilder()
        // sql.AppendLine("SELECT")
        // sql.AppendLine("com.company_id")
        // sql.AppendLine(",com.name")
        // sql.AppendLine(",com.cnpj")
        // sql.AppendLine(",com.address")
        // sql.AppendLine(",com.number")
        // sql.AppendLine(",com.complement")
        // sql.AppendLine(",com.city")
        // sql.AppendLine(",com.state")
        // sql.AppendLine(",com.cep")
        // sql.AppendLine("FROM Company com")
        // Return sql.ToString()
        // End Function

        private static string GetFindQuery(int companyId)
        {
            var sql = new StringBuilder();
            sql.AppendLine("SELECT");
            sql.AppendLine("exp.company_id");
            sql.AppendLine(",exp.expense_id");
            sql.AppendLine(",exp.description");
            sql.AppendLine(",exp.cost");
            sql.AppendLine(",exp.date");
            sql.AppendLine(",exp.user_request");
            sql.AppendLine(",exp.user_manager");
            sql.AppendLine(",exp.cost_approved");
            sql.AppendLine(",exp.date_approved");
            sql.AppendLine(",exp.picture");
            sql.AppendLine(",exp.type_id");
            sql.AppendLine(",exp.situation");
            sql.AppendLine(",expTy.description as type_description");
            sql.AppendLine(",exp.manager_motive").AppendLine();
            sql.AppendLine("FROM Expense exp");
            sql.AppendLine("JOIN ExpenseType expTy ON");
            sql.AppendLine("expTy.company_id=exp.company_id");
            sql.AppendLine("AND expTy.type_id=exp.type_id").AppendLine();
            sql.AppendFormat("WHERE exp.company_id={0}", companyId).AppendLine();
            return sql.ToString();
        }

        private static void Populate(IDataReader reader, Expense expense)
        {
            expense.CompanyId = (int)reader["company_id"];
            expense.ExpenseId = (int)reader["expense_id"];
            expense.Description = (string)reader["description"];
            expense.Cost = (double)reader["cost"];
            expense.Date = (DateTime)reader["date"];
            expense.RequestUser = (string)reader["user_request"];
            expense.ManagerUser = (string)reader["user_manager"];
            var costApproved = reader["cost_approved"];
            if (!ReferenceEquals(costApproved, DBNull.Value))
            {
                expense.ApprovedCost = (double)reader["cost_approved"];
            }
            else
            {
                expense.ApprovedCost = null;
            }

            var dateApproved = reader["date_approved"];
            if (!ReferenceEquals(costApproved, DBNull.Value))
            {
                expense.ApprovedDate = (DateTime)reader["date_approved"];
            }
            else
            {
                expense.ApprovedDate = null;
            }

            var managerMotive = reader["manager_motive"];
            if (!ReferenceEquals(managerMotive, DBNull.Value))
            {
                expense.Motive = (string)reader["manager_motive"];
            }
            else
            {
                expense.Motive = null;
            }

            expense.Picture = (string)reader["picture"];
            expense.Category = new ExpenseCategory()
            {
                CompanyId = expense.CompanyId,
                CategoryId = (int)reader["type_id"],
                Description = (string)reader["type_description"]
            };
            expense.Situation = (ExpenseSituation)reader["situation"];
        }


        // Private Shared Function GetUpdateQuery() As String
        // Dim sql As New StringBuilder()
        // sql.AppendLine("UPDATE Company SET")
        // sql.AppendLine("company_id=@company_id")
        // sql.AppendLine(",name=@name")
        // sql.AppendLine(",cnpj=@cnpj")
        // sql.AppendLine(",address=@address")
        // sql.AppendLine(",number=@number")
        // sql.AppendLine(",complement=@complement")
        // sql.AppendLine(",city=@city")
        // sql.AppendLine(",state=@state")
        // sql.AppendLine(",cep=@cep")
        // Return sql.ToString()
        // End Function

        public override void Dispose()
        {
            connection.Close();
        }
    }
}
