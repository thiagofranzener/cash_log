Imports System.Data
Imports System.Data.SqlClient
Imports System.Text
Imports LazyDevUtils.Extensions

Public Class ExpenseCategoryRepository
    Inherits Repository

    Private ReadOnly connection As SqlConnection
    Sub New(connection As SqlConnection)
        Me.connection = connection
    End Sub

    Public Function Add(companyId As Integer, description As String) As Integer
        Dim nextId As Integer = GetNextId(companyId)
        Dim expenseCategory As New ExpenseCategory() With {
        .CompanyId = companyId,
        .CategoryId = nextId,
        .Description = description
        }
        Dim command As SqlCommand = connection.CreateCommand()
        command.CommandText = GetAddQuery()
        PopulateQuery(command, expenseCategory)
        command.ExecuteNonQuery()
        Return nextId
    End Function

    Public Sub Update(expenseCategory As ExpenseCategory)
        Dim command As SqlCommand = connection.CreateCommand()
        command.CommandText = GetUpdateQuery()
        PopulateQuery(command, expenseCategory)
        command.ExecuteNonQuery()
    End Sub

    Public Function GetAll(companyId As Integer) As HashSet(Of ExpenseCategory)
        Dim command = connection.CreateCommand()
        command.CommandText = GetFindQuery(companyId)
        Dim result As New HashSet(Of ExpenseCategory)
        Using reader As IDataReader = command.ExecuteReader()
            While reader.Read()
                Dim expenseCategory As New ExpenseCategory()
                Populate(reader, expenseCategory)
                result.Add(expenseCategory)
            End While
        End Using
        Return result
    End Function

    Public Function GetById(companyId As Integer, categoryId As Integer) As ExpenseCategory
        Dim sql As New StringBuilder(GetFindQuery(companyId))
        sql.AppendFormat("AND type_id={0}", categoryId)
        Dim command = connection.CreateCommand()
        command.CommandText = sql.ToString()
        Using reader As IDataReader = command.ExecuteReader()
            If reader.Read() Then
                Dim expenseCategory As New ExpenseCategory()
                Populate(reader, expenseCategory)
                Return expenseCategory
            End If
        End Using
        Return Nothing
    End Function

    Public Sub Delete(companyId As Integer, categoryId As Integer)
        Dim command = connection.CreateCommand()
        command.CommandText = GetDeleteQuery(companyId, categoryId)
        command.ExecuteNonQuery()
    End Sub

    Private Sub Populate(reader As IDataReader, expenseCategory As ExpenseCategory)
        expenseCategory.CompanyId = reader("company_id")
        expenseCategory.CategoryId = reader("type_id")
        expenseCategory.Description = reader("description")
    End Sub

    Public Function Exist(companyId As Integer, expenseCategoryId As Integer) As Boolean
        Dim command As SqlCommand = connection.CreateCommand()
        command.CommandText = $"SELECT COUNT(type_id) FROM ExpenseType WHERE company_id=@company_id AND type_id=@type_id"
        command.Parameters.AddWithValue("@company_id", SqlDbType.Int).Value = companyId
        command.Parameters.AddWithValue("@type_id", SqlDbType.Int).Value = expenseCategoryId
        If command.ExecuteScalar() > 0 Then Return True
        Return False
    End Function

    Private Function GetNextId(companyId As Integer) As Integer
        Dim command As SqlCommand = connection.CreateCommand()
        command.CommandText = "SELECT ISNULL(MAX(type_id),0) + 1 FROM ExpenseType WHERE company_id=@company_id"
        command.Parameters.AddWithValue("@company_id", SqlDbType.Int).Value = companyId
        Return command.ExecuteScalar()
    End Function

    Private Shared Function GetAddQuery() As String
        Dim sql As New StringBuilder()
        sql.AppendLine("INSERT INTO ExpenseType")
        sql.AppendLine("(company_id,type_id,description)")
        sql.AppendLine("VALUES")
        sql.AppendLine("(@company_id,@type_id,@description)")
        Return sql.ToString()
    End Function

    Private Shared Function GetUpdateQuery() As String
        Dim sql As New StringBuilder()
        sql.AppendLine("UPDATE ExpenseType SET")
        sql.AppendLine("description=@description")
        sql.AppendFormat("WHERE company_id=@company_id").AppendLine()
        sql.AppendFormat("AND type_id=@type_id")
        Return sql.ToString()
    End Function

    Private Shared Function GetDeleteQuery(companyId As Integer, categoryId As Integer) As String
        Dim sql As New StringBuilder()
        sql.AppendLine("DELETE FROM ExpenseType")
        sql.AppendFormat("WHERE company_Id={0}", companyId).AppendLine()
        sql.AppendFormat("AND type_id={0}", categoryId)
        Return sql.ToString()
    End Function


    Private Shared Function GetFindQuery(companyId As Integer) As String
        Dim sql As New StringBuilder()
        sql.AppendLine("SELECT")
        sql.AppendLine("company_id")
        sql.AppendLine(",type_id")
        sql.AppendLine(",description")
        sql.AppendLine("FROM ExpenseType")
        sql.AppendFormat("WHERE company_id={0}", companyId)
        Return sql.ToString()
    End Function

    Private Shared Sub PopulateQuery(command As SqlCommand, expenseCategory As ExpenseCategory)
        command.Parameters.AddWithValue("@description", expenseCategory.Description)
        command.Parameters.AddWithValue("@company_id", expenseCategory.CompanyId)
        command.Parameters.AddWithValue("@type_id", expenseCategory.CategoryId)
    End Sub

    Public Overrides Sub Dispose()
        connection.Close()
    End Sub

End Class
