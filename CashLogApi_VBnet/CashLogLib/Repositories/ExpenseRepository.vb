Imports System.Data
Imports System.Data.SqlClient
Imports System.Text

Public Class ExpenseRepository
    Inherits Repository

    Private ReadOnly connection As SqlConnection

    Sub New(connection As SqlConnection)
        Me.connection = connection
    End Sub

    Public Function AddEmployeeRequest(request As ExpenseEmployeeRequest) As Integer
        Dim nextId As Integer = GetNextId(request.CompanyId)
        Dim command As SqlCommand = connection.CreateCommand()
        command.CommandText = GetAddEmployeeRequestQuery()
        PopulateEmployeeRequestQuery(command, request, nextId)
        command.ExecuteNonQuery()
        Return nextId
    End Function


    Public Sub UpdateExpense(request As ExpenseAdministratorApprovation)
        Dim command As SqlCommand = connection.CreateCommand()
        command.CommandText = GetUpdateEmployeeRequestQuery()
        PopulateAdministrationApprovationQuery(command, request)
        command.ExecuteNonQuery()
    End Sub

    Public Function GetAll(companyId As Integer, user As User) As HashSet(Of Expense)
        Dim result As New HashSet(Of Expense)
        Dim sql As New StringBuilder(GetFindQuery(companyId))
        Dim command = connection.CreateCommand()
        If user.Role = UserRole.Employee Then
            sql.AppendFormat("AND exp.user_request=@user_request")
            command.Parameters.AddWithValue("@user_request", DbType.String).Value = user.User
        End If
        command.CommandText = sql.ToString()
        Using reader As IDataReader = command.ExecuteReader()
            While reader.Read()
                Dim expense As New Expense()
                Populate(reader, expense)
                result.Add(expense)
            End While
        End Using
        Return result
    End Function

    Public Function GetById(companyId As Integer, expenseId As Integer, user As User) As Expense
        Dim result As Expense = Nothing
        Dim sql As New StringBuilder(GetFindQuery(companyId))
        sql.AppendLine().AppendFormat("AND expense_id={0}", expenseId)
        Dim command = connection.CreateCommand()
        If user.Role = UserRole.Employee Then
            sql.AppendFormat("AND exp.user_request=@user_request")
            command.Parameters.AddWithValue("@user_request", DbType.String).Value = user.User
        End If
        command.CommandText = sql.ToString()
        Using reader As IDataReader = command.ExecuteReader()
            If reader.Read() Then
                Dim expense As New Expense()
                Populate(reader, expense)
                result = expense
            End If
        End Using
        Return result
    End Function

    Private Shared Function GetAddEmployeeRequestQuery() As String
        Dim sql As New StringBuilder()
        sql.AppendLine("INSERT INTO Expense")
        sql.AppendLine("(expense_id,description,cost,date,user_request,user_manager,picture,type_id,situation,company_id)")
        sql.AppendLine("VALUES").AppendLine()
        sql.AppendLine("(@expense_id,@description,@cost,@date,@user_request,@user_manager,@picture,@type_id,@situation,@company_id)")
        Return sql.ToString()
    End Function

    Private Shared Function GetUpdateEmployeeRequestQuery() As String
        Dim sql As New StringBuilder()
        sql.AppendLine("UPDATE Expense")
        sql.AppendLine("SET cost_approved=@cost_approved")
        sql.AppendLine(",manager_motive=@manager_motive")
        sql.AppendLine(",situation=@situation")
        sql.AppendLine(",date_approved=@date_approved")
        sql.AppendLine("WHERE company_id=@company_id AND expense_id=@expense_id")
        Return sql.ToString()
    End Function

    Private Shared Sub PopulateEmployeeRequestQuery(command As SqlCommand, request As ExpenseEmployeeRequest, expenseId As Integer)
        command.Parameters.AddWithValue("@expense_id", SqlDbType.Int).Value = expenseId
        command.Parameters.AddWithValue("@company_id", SqlDbType.Int).Value = request.CompanyId
        command.Parameters.AddWithValue("@description", SqlDbType.VarChar).Value = request.Description
        command.Parameters.AddWithValue("@date", SqlDbType.Date).Value = request.Date
        command.Parameters.AddWithValue("@user_manager", SqlDbType.VarChar).Value = request.ManagerUser
        command.Parameters.AddWithValue("@user_request", SqlDbType.VarChar).Value = request.RequestUser
        command.Parameters.AddWithValue("@type_id", SqlDbType.Int).Value = request.TypeId
        command.Parameters.AddWithValue("@situation", SqlDbType.Int).Value = request.Situation
        command.Parameters.AddWithValue("@cost", SqlDbType.Float).Value = request.Cost
        command.Parameters.AddWithValue("@picture", SqlDbType.Image).Value = request.Picture
    End Sub

    Private Shared Sub PopulateAdministrationApprovationQuery(command As SqlCommand, request As ExpenseAdministratorApprovation)
        If request.Motive = Nothing Then request.Motive = ""
        If request.ApprovedCost = Nothing Then request.ApprovedCost = 0
        command.Parameters.AddWithValue("@expense_id", SqlDbType.Int).Value = request.ExpenseId
        command.Parameters.AddWithValue("@company_id", SqlDbType.Int).Value = request.CompanyId
        command.Parameters.AddWithValue("@cost_approved", SqlDbType.Float).Value = request.ApprovedCost
        command.Parameters.AddWithValue("@manager_motive", SqlDbType.VarChar).Value = request.Motive
        command.Parameters.AddWithValue("@situation", SqlDbType.VarChar).Value = CInt(request.Situation)
        command.Parameters.AddWithValue("@date_approved", SqlDbType.Date).Value = Date.Now
    End Sub


    Private Function GetNextId(companyId As Integer) As Integer
        Dim command As SqlCommand = connection.CreateCommand()
        command.CommandText = "SELECT ISNULL(MAX(expense_id),0) + 1 FROM Expense WHERE company_id=@company_id"
        command.Parameters.AddWithValue("@company_id", SqlDbType.Int).Value = companyId
        Return command.ExecuteScalar()
    End Function

    Public Function Exist(companyId As Integer, expenseId As Integer) As Boolean
        Dim command As SqlCommand = connection.CreateCommand()
        command.CommandText = $"SELECT COUNT(expense_id) FROM Expense WHERE company_id={companyId} AND expense_id={expenseId}"
        If command.ExecuteScalar() > 0 Then Return True
        Return False
    End Function

    Private Shared Function GetFindQuery(companyId As Integer) As String
        Dim sql As New StringBuilder()
        sql.AppendLine("SELECT")
        sql.AppendLine("exp.company_id")
        sql.AppendLine(",exp.expense_id")
        sql.AppendLine(",exp.description")
        sql.AppendLine(",exp.cost")
        sql.AppendLine(",exp.date")
        sql.AppendLine(",exp.user_request")
        sql.AppendLine(",exp.user_manager")
        sql.AppendLine(",exp.cost_approved")
        sql.AppendLine(",exp.date_approved")
        sql.AppendLine(",exp.picture")
        sql.AppendLine(",exp.type_id")
        sql.AppendLine(",exp.situation")
        sql.AppendLine(",expTy.description as type_description")
        sql.AppendLine(",exp.manager_motive").AppendLine()
        sql.AppendLine("FROM Expense exp")
        sql.AppendLine("JOIN ExpenseType expTy ON")
        sql.AppendLine("expTy.company_id=exp.company_id")
        sql.AppendLine("AND expTy.type_id=exp.type_id").AppendLine()
        sql.AppendFormat("WHERE exp.company_id={0}", companyId).AppendLine()
        Return sql.ToString()
    End Function

    Private Shared Sub Populate(reader As IDataReader, expense As Expense)
        expense.CompanyId = reader("company_id")
        expense.ExpenseId = reader("expense_id")
        expense.Description = reader("description")
        expense.Cost = reader("cost")
        expense.Date = reader("date")
        expense.RequestUser = reader("user_request")
        expense.ManagerUser = reader("user_manager")

        Dim costApproved = reader("cost_approved")
        If costApproved IsNot DBNull.Value Then
            expense.ApprovedCost = reader("cost_approved")
        Else
            expense.ApprovedCost = Nothing
        End If

        Dim dateApproved = reader("date_approved")
        If costApproved IsNot DBNull.Value Then
            expense.ApprovedDate = reader("date_approved")
        Else
            expense.ApprovedDate = Nothing
        End If

        Dim managerMotive = reader("manager_motive")
        If managerMotive IsNot DBNull.Value Then
            expense.Motive = reader("manager_motive")
        Else
            expense.Motive = Nothing
        End If

        expense.Picture = reader("picture")
        expense.Category = New ExpenseCategory With {
        .CompanyId = expense.CompanyId,
        .CategoryId = reader("type_id"),
        .Description = reader("type_description")
        }
        expense.Situation = CInt(reader("situation"))

    End Sub

    Public Overrides Sub Dispose()
        connection.Close()
    End Sub

End Class